using System;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class OrdersController(ICartService cartService,IUnitOfWork unitOfWork):BaseApiController
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDTO createOrderDTO)
    {
        var email = User.GetEmail();
        var cart = await cartService.GetCartAsync(createOrderDTO.CartId);
        if (cart == null) return BadRequest("Cart not found!");
        if (cart.PaymentIntentId == null) return BadRequest("No payment intent for this order!");
        var orderItems = new List<OrderItem>();
        foreach (var item in cart.Items)
        {
            var productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
            if (productItem == null) return BadRequest("Could not find the product in our system!");
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
        var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(createOrderDTO.DeliveryMethodId);
        if (deliveryMethod == null) return BadRequest("No delivery method selected");
        var order = new Order
        {
            OrderItems = orderItems,
            DeliveryMethod = deliveryMethod,
            ShippingAddress = createOrderDTO.ShippingAddress,
            SubTotal = orderItems.Sum(x => x.Price * x.Quantity),
            PaymentSummary = createOrderDTO.PaymentSummary,
            PaymentIntentId = cart.PaymentIntentId,
            BuyerEmail = email,
        };
        unitOfWork.Repository<Order>().Create(order);
        if (await unitOfWork.Complete()) return Ok(order);
        return BadRequest("Problem creating order!");
    }
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(int id)
    {
        var specification = new OrderSpecification(User.GetEmail(), id);
        var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(specification);
        return order == null ? BadRequest("Could not find the order by id!") : Ok(order.ToDTO());
    }
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDTO>>> GetOrders()
    {
        var specification = new OrderSpecification(User.GetEmail());
        var orders = await unitOfWork.Repository<Order>().GetAllAsync(specification);
        var ordersToReturn = orders.Select(x => x.ToDTO()).ToList();
        return orders == null ? BadRequest("Could not fetch orders for user!") : Ok(ordersToReturn);
    }
}
