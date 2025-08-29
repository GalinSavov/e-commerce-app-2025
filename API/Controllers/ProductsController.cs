using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductRepository productRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
        {
            IReadOnlyList<Product> products = await productRepository.GetProductsAsync();
            return products.Count == 0 ? NotFound() : Ok(products);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Product? product = await productRepository.GetProductByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            productRepository.CreateProduct(product);
            var result = await productRepository.SaveChangesAsync();
            return result ? CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product) : BadRequest(); // returns a 201 Created if true, a Location Route where the object can be found, and the body of the object
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (!await productRepository.ProductExists(id)) return NotFound();
            if (product.Id != id) return BadRequest("ID in route does not match product ID");
            productRepository.UpdateProduct(product);
            await productRepository.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            Product? product = await productRepository.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            productRepository.DeleteProduct(product);
            await productRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
