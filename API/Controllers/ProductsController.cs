using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers
{
    public class ProductsController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams productParams)
        {
            var specification = new ProductSpecification(productParams);
            return await CreatePagedResult(unitOfWork.Repository<Product>(), specification, productParams.PageIndex, productParams.PageSize);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Product? product = await unitOfWork.Repository<Product>().GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetProductBrands()
        {
            var specification = new BrandListSpecification();
            var brands = await unitOfWork.Repository<Product>().GetAllAsync<string>(specification);
            return brands == null ? NotFound() : Ok(brands);
        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>>GetProductTypes()
        {
            var specification = new TypeListSpecification();
            var types = await unitOfWork.Repository<Product>().GetAllAsync<string>(specification);
            return types == null ? NotFound() : Ok(types);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            unitOfWork.Repository<Product>().Create(product);
            var result = await unitOfWork.Complete();
            return result ? CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product) : BadRequest(); // returns a 201 Created if true, a Location Route where the object can be found, and the body of the object
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (!await unitOfWork.Repository<Product>().ProductExists(id)) return NotFound();
            if (product.Id != id) return BadRequest("ID in route does not match product ID");
            unitOfWork.Repository<Product>().Update(product);
            await unitOfWork.Complete();
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            Product? product = await unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();
            unitOfWork.Repository<Product>().Delete(product);
            await unitOfWork.Complete();
            return NoContent();
        }
    }
}
