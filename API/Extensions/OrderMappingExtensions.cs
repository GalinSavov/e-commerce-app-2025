using System;
using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions;

public static class OrderMappingExtensions
{
    public static OrderDTO ToDTO(this Order order)
    {
        return new OrderDTO
        {
            Id = order.Id,
            BuyerEmail = order.BuyerEmail,
            OrderDate = order.OrderDate,
            ShippingAddress = order.ShippingAddress,
            PaymentSummary = order.PaymentSummary,
            DeliveryMethod = order.DeliveryMethod.Description,
            ShippingPrice = order.DeliveryMethod.Price,
            OrderItems = order.OrderItems.Select(x => x.ToDTO()).ToList(),
            SubTotal = order.SubTotal,
            Total = order.GetTotal(),
            OrderStatus = order.OrderStatus.ToString(),
            PaymentIntentId = order.PaymentIntentId,
        };
    }
    public static OrderItemDTO ToDTO(this OrderItem orderItem)
    {
        return new OrderItemDTO
        {
            ProductId = orderItem.ItemOrdered.ProductId,
            ProductName = orderItem.ItemOrdered.ProductName,
            PictureURL = orderItem.ItemOrdered.PictureURL,
            Price = orderItem.Price,
            Quantity = orderItem.Quantity,

        };
    }
}
