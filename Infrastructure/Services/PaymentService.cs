using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService: IPaymentService
{
    private readonly ICartService cartService;
    private readonly IUnitOfWork unitOfWork;

    public PaymentService(IConfiguration config,ICartService cartService,IUnitOfWork unitOfWork)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        this.cartService = cartService;
        this.unitOfWork = unitOfWork;
    }
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        var shoppingCart = await cartService.GetCartAsync(cartId) ?? throw new NullReferenceException("Cart Unavailable!"); // get cart and check if its null

        // All calculations in domain are done in dollars
        var shippingPriceDollars = await GetShippingPriceAsync(shoppingCart);

        await ValidateItemsInShoppingCartAsync(shoppingCart);

        var subtotalDollars = CalculateSubtotal(shoppingCart);

        if (shoppingCart.Coupon != null)
        {
            subtotalDollars = await ApplyDiscountAsync(shoppingCart, subtotalDollars);
        }

        var totalDollars = subtotalDollars + shippingPriceDollars;

        // Convert once to cents at the Stripe boundary
        var totalInCents = (long)Math.Round(totalDollars * 100m, MidpointRounding.AwayFromZero);

        await CreateUpdatePaymentIntentAsync(shoppingCart, totalInCents);

        await cartService.SetCartAsync(shoppingCart);
        
        return shoppingCart;
    }
    public async Task<string> RefundPayment(string paymentIntentId)
    {
        var refundOptions = new RefundCreateOptions
        {
            PaymentIntent = paymentIntentId
        };
        var refundService = new RefundService();
        var result = await refundService.CreateAsync(refundOptions);
        return result.Status; // Return the status of the refund (e.g., "succeeded", "pending", "failed")
    }
    // Returns shipping price in dollars
    private async Task<decimal> GetShippingPriceAsync(ShoppingCart shoppingCart)
    {
        decimal shippingPrice = 0;
        if (shoppingCart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync((int)shoppingCart.DeliveryMethodId) 
                ?? throw new NullReferenceException("Delivery Method Not Found!");
            shippingPrice = deliveryMethod.Price;
        }
        return shippingPrice;
    }
    private async Task ValidateItemsInShoppingCartAsync(ShoppingCart shoppingCart)
    {
        foreach (var item in shoppingCart.Items)
        {
            var productItem = await unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId) ?? throw new NullReferenceException("Product Item in Cart Not Found!");
            if (item.Price != productItem.Price) item.Price = productItem.Price;
        }
    }
    // Subtotal in dollars
    private static decimal CalculateSubtotal(ShoppingCart shoppingCart)
    {
        var subtotal = shoppingCart.Items.Sum(i => i.Price * i.Quantity); // decimal
        return subtotal;
    }
    // Apply discount to an amount in dollars
    private static async Task<decimal> ApplyDiscountAsync(ShoppingCart shoppingCart, decimal amountDollars)
    {
        var couponId = shoppingCart.Coupon!.CouponId;
        var coupon = await new Stripe.CouponService().GetAsync(couponId)
                    ?? throw new NullReferenceException("Coupon Not Found!");

        if (coupon.PercentOff is not null)
        {
            var discount = amountDollars * (coupon.PercentOff.Value / 100m);
            // Round to cents in dollars
            return amountDollars - Math.Round(discount, 2, MidpointRounding.AwayFromZero);
        }

        if (coupon.AmountOff is not null)
        {
            // Stripe AmountOff is in the smallest currency unit (cents)
            var amountOffDollars = coupon.AmountOff.Value / 100m;
            return Math.Max(0, amountDollars - amountOffDollars);
        }
            return amountDollars;
        }
    private static async Task CreateUpdatePaymentIntentAsync(ShoppingCart shoppingCart,long amount)
    {
        var paymentIntentService = new PaymentIntentService();
        PaymentIntent? paymentIntent = null;
        if (string.IsNullOrEmpty(shoppingCart.PaymentIntentId)) // if no payment intent exists, create one, assign the client secret and paymentIntentId to the cart properties
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                PaymentMethodTypes = ["card"]
            };
            paymentIntent = await paymentIntentService.CreateAsync(options);
            shoppingCart.PaymentIntentId = paymentIntent.Id;
            shoppingCart.ClientSecret = paymentIntent.ClientSecret;
        }
        else // update the payment intent otherwise
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = amount
            };
            paymentIntent = await paymentIntentService.UpdateAsync(shoppingCart.PaymentIntentId, options);
        }
    }

    
}
