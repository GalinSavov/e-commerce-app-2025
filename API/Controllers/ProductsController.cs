using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IGenericRepository<Product> repository) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            var specification = new ProductSpecification(brand, type,sort);
            var products = await repository.GetAllAsync(specification);
            return products == null ? NotFound() : Ok(products);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Product? product = await repository.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetProductBrands()
        {
            var specification = new BrandListSpecification();
            var brands = await repository.GetAllAsync<string>(specification);
            return brands == null ? NotFound() : Ok(brands);
        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>>GetProductTypes()
        {
            var specification = new TypeListSpecification();
            var types = await repository.GetAllAsync<string>(specification);
            return types == null ? NotFound() : Ok(types);
        }
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repository.Create(product);
            var result = await repository.SaveAllAsync();
            return result ? CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product) : BadRequest(); // returns a 201 Created if true, a Location Route where the object can be found, and the body of the object
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (!await repository.ProductExists(id)) return NotFound();
            if (product.Id != id) return BadRequest("ID in route does not match product ID");
            repository.Update(product);
            await repository.SaveAllAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            Product? product = await repository.GetByIdAsync(id);
            if (product == null) return NotFound();
            repository.Delete(product);
            await repository.SaveAllAsync();
            return NoContent();
        }
    }
}
