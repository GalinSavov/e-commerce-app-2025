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
public class AdminController(IUnitOfWork unitOfWork) : BaseApiController
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
}