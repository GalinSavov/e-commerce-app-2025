using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected async Task<ActionResult> CreatePagedResult<T>(IGenericRepository<T> repository,
        ISpecification<T> specification, int pageIndex, int pageSize) where T : BaseEntity
        {
            var items = await repository.GetAllAsync(specification);
            var count = await repository.CountAsync(specification);
            var pagination = new Pagination<T>(pageIndex, pageSize, count, items);
            return Ok(pagination);
        }
        protected async Task<ActionResult> CreatePagedResult<T,TDto>(IGenericRepository<T> repository,
        ISpecification<T> specification, int pageIndex, int pageSize,Func<T,TDto> toDto) where T : BaseEntity,IDTOConvertible where TDto : class
        {
            var items = await repository.GetAllAsync(specification);
            var count = await repository.CountAsync(specification);
            var dtoItems = items.Select(toDto).ToList();
            var pagination = new Pagination<TDto>(pageIndex, pageSize, count, dtoItems);
            return Ok(pagination);
        }
    }
}
