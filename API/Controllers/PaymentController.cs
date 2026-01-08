using System;
using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;

public class PaymentController(IPaymentService paymentService,IUnitOfWork unitOfWork,ILogger<PaymentController> logger,IConfiguration config,IHubContext<NotificationHub> hubContext):BaseApiController
{
    private readonly string _whSecret = config["StripeSettings:WhSecret"]!;
    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var shoppingCart = await paymentService.CreateOrUpdatePaymentIntent(cartId);
        return shoppingCart == null ? BadRequest("Problem with your cart!") : Ok(shoppingCart);
    }
    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        return Ok(await unitOfWork.Repository<DeliveryMethod>().ListAllAsync());
    }
    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebHook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _whSecret
            );
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Invalid Stripe signature");
            return BadRequest(); // ✅ NOT 500
        }

        // ✅ Only handle what you care about
        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var intent = stripeEvent.Data.Object as PaymentIntent;
            if (intent != null)
            {
                await HandlePaymentIntentSucceeded(intent);
            }
        }
        // ✅ Always ACK Stripe
        return Ok();
    }

    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        var spec = new OrderSpecification(intent.Id, true);
        var order = await unitOfWork.Repository<Order>()
            .GetEntityWithSpec(spec);

        if (order == null)
        {
            logger.LogWarning("Order not found for PaymentIntent {IntentId}", intent.Id);
            return; // ✅ do NOT throw
        }
        var expectedAmount = (long)Math.Round(order.GetTotal() * 100, MidpointRounding.AwayFromZero);
        if (expectedAmount != intent.Amount)
        {
            order.OrderStatus = OrderStatus.PaymentMismatch;
        }
        else
        {
            order.OrderStatus = OrderStatus.PaymentReceived;
        }

        await unitOfWork.Complete();
        var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
        if (!string.IsNullOrEmpty(connectionId))
        {
            await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification",order.ToDTO());
        }
    }
}
