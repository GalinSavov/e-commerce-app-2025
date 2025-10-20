using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CartController(ICartService cartService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetShoppingCartAsync(string id)
        {
            var cart = await cartService.GetCartAsync(id);
            return  Ok(cart ?? new ShoppingCart { Id = id });
        }
        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> SetShoppingCartAsync(ShoppingCart shoppingCart)
        {
            var cart = await cartService.SetCartAsync(shoppingCart);
            return cart == null ? BadRequest("Problem with cart") : Ok(cart);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteShoppingCartAsync(string id)
        {
            var cart = await cartService.GetCartAsync(id);
            if (cart == null) return BadRequest("Could not find cart");
            return await cartService.DeleteCartAsync(id)  == true ? NoContent() : BadRequest("Error deleting cart");
        }
    }
}
