using System;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class OrdersController(ICartService cartService,IUnitOfWork unitOfWork):BaseApiController
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(OrderDTO orderDTO)
    {
        var email = User.GetEmail();
        var cart = await cartService.GetCartAsync(orderDTO.CartId);
        if (cart == null) return BadRequest("Cart not found!");
        if (cart.PaymentIntentId == null) return BadRequest("No payment intent for this order!");
        var orderItems = new List<OrderItem>();
        foreach (var item in cart.Items)
        {
            var productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
            if (productItem == null) return BadRequest("Problem with the order");
            var itemOrdered = new ProductItemOrdered
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                PictureURL = item.PictureURL,
            };
            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quantity = item.Quantity
            };
            orderItems.Add(orderItem);
        }
        var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(orderDTO.DeliveryMethodId);
        if (deliveryMethod == null) return BadRequest("No delivery method selected");
        var order = new Order
        {
            OrderItems = orderItems,
            DeliveryMethod = deliveryMethod,
            ShippingAddress = orderDTO.ShippingAddress,
            SubTotal = orderItems.Sum(x => x.Price * x.Quantity),
            PaymentSummary = orderDTO.PaymentSummary,
            PaymentIntentId = cart.PaymentIntentId,
            BuyerEmail = email,
        };
        unitOfWork.Repository<Order>().Create(order);
        if (await unitOfWork.Complete()) return order;
        return BadRequest("Problem creating order!");
    }
}
