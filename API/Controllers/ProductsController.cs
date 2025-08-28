using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(StoreContext storeContext) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            List<Product> products = await storeContext.Products.ToListAsync();
            return products.Count == 0 ? NotFound() : Ok(products);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Product? product = await storeContext.Products.FindAsync(id);
            return product == null ? NotFound() : Ok(product);
        }
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            await storeContext.Products.AddAsync(product);
            var result = await storeContext.SaveChangesAsync();
            if (result <= 0) return BadRequest();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product); // returns a 201 Created, a Location Route where the object can be found, and the body of the object
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (!await storeContext.Products.AnyAsync(x => x.Id == id)) return NotFound();
            if (product.Id != id) return BadRequest("ID in route does not match product ID");
            storeContext.Entry(product).State = EntityState.Modified;
            await storeContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            Product? product = await storeContext.Products.FindAsync(id);
            if (product == null) return NotFound();
            storeContext.Products.Remove(product);
            await storeContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
