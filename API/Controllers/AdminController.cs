using API.Controllers;
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
    public async Task<ActionResult<Order>> GetOrders([FromQuery]OrderSpecParams orderParams)
    {
        var spec = new OrderSpecification(orderParams);
        return await CreatePagedResult(unitOfWork.Repository<Order>(),spec,orderParams.PageIndex,orderParams.PageSize);
    }
}