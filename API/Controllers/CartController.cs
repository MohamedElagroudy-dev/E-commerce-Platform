using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Cart ID is required");

            var cart = await cartService.GetCartAsync(id);

            return Ok(cart ?? new ShoppingCart { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateCart([FromBody] ShoppingCart cart)
        {
            if (cart == null || string.IsNullOrWhiteSpace(cart.Id))
                return BadRequest("Cart data is invalid");

            var updatedCart = await cartService.SetCartAsync(cart);

            if (updatedCart == null)
                return StatusCode(500, "Problem saving the cart");

            return Ok(updatedCart);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCart(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Cart ID is required");

            var deleted = await cartService.DeleteCartAsync(id);

            if (!deleted)
                return NotFound($"Cart with id '{id}' not found");

            return NoContent();
        }
    }
}
