using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(IConfiguration config,
ICartService cartService,
IGenericRepository<Core.Entities.Product> productRepo,
IGenericRepository<DeliveryMethod> deliveryMethodRepo) : IPaymentService
{
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        var shoppingCart = await cartService.GetCartAsync(cartId); // get cart and check if its null
        if (shoppingCart == null) return null;
        var shippingPrice = 0m;
        if (shoppingCart.DeliveryMethodId.HasValue) // fetch deliveryMethod from database, if not null - assign its value to shipping price
        {
            var deliveryMethod = await deliveryMethodRepo.GetByIdAsync((int)shoppingCart.DeliveryMethodId);
            if (deliveryMethod == null) return null;
            shippingPrice = deliveryMethod.Price;
        }
        foreach (var item in shoppingCart.Items) // check that product exists and if the price of the items in the client match the price of the products on the database
        {
            var productItem = await productRepo.GetByIdAsync(item.ProductId);
            if (productItem == null) return null;
            if (item.Price != productItem.Price) item.Price = productItem.Price;
        }
        var paymentIntentService = new PaymentIntentService();
        PaymentIntent? paymentIntent = null;
        if (string.IsNullOrEmpty(shoppingCart.PaymentIntentId)) // if no payment intent exists, create one, assign the client secret and paymentIntentId to the cart properties
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)shoppingCart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice * 100,
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
                Amount = (long)shoppingCart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice * 100
            };
            paymentIntent = await paymentIntentService.UpdateAsync(shoppingCart.PaymentIntentId, options);
        }
        await cartService.SetCartAsync(shoppingCart);
        return shoppingCart;
    }
}
