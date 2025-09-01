using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IGenericRepository<Product> repository) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery] string? brand,string? type,string? sort)
        {
            IReadOnlyList<Product> products = await repository.GetAllAsync();
            return products.Count == 0 ? NotFound() : Ok(products);
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
            //ToDO:
            return Ok();
        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>>GetProductTypes()
        {
            //ToDO:  
            return Ok();
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
