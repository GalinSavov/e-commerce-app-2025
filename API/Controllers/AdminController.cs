using API.Controllers;
using API.DTOs;
using API.Extensions;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;
[Authorize(Roles =  "Admin")]
public class AdminController(IUnitOfWork unitOfWork,IPaymentService paymentService) : BaseApiController
{
    [HttpGet("orders")]
    public async Task<ActionResult<IReadOnlyList<OrderDTO>>> GetOrders([FromQuery]OrderSpecParams orderParams)
    {
        var spec = new OrderSpecification(orderParams);
        return await CreatePagedResult(unitOfWork.Repository<Order>(),spec,orderParams.PageIndex,orderParams.PageSize, o => o.ToDTO());
    }
    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(int id)
    {
        var spec = new OrderSpecification(id);
        return await unitOfWork.Repository<Order>().GetEntityWithSpec(spec) is Order order ? Ok(order.ToDTO()) : NotFound();
    }
    [HttpPost("orders/refund/{id:int}")]
    public async Task<ActionResult<OrderDTO>> RefundOrder(int id)
    {
        var spec = new OrderSpecification(id);
        var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        if(order == null) return NotFound();
        if(order.OrderStatus == OrderStatus.Pending) return BadRequest("Payment has not been completed for this order.");
        var result = await paymentService.RefundPayment(order.PaymentIntentId);
        if(result == "succeeded")
        {
            order.OrderStatus = OrderStatus.Refunded;
            await unitOfWork.Complete();
            return Ok(order.ToDTO());
        }
        return BadRequest("Problem refunding order");
    }
}