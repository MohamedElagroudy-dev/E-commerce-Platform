using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IproductRepository repo) : ControllerBase
    {
        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type,string? sort)
        {
            return Ok(await repo.GetProductsAsync(brand, type, sort));
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repo.GetProductByIdAsync(id);

            if (product == null)
                return NotFound();

            return product;
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repo.AddProduct(product);

            if (await repo.SaveChangesAsync())
            {
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            return product;
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product updatedProduct)
        {
            if (updatedProduct == null || updatedProduct.Id != id || !repo.ProductExists(updatedProduct.Id))
                return BadRequest("Cannot update this product");

            repo.UpdateProduct(updatedProduct);

            if (await repo.SaveChangesAsync())
            {
                return Ok();
            }

           
            return BadRequest();
        }


        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await repo.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            repo.DeleteProduct(product);
            if (await repo.SaveChangesAsync())
            {
                return Ok();
            }
            return BadRequest("Problem in deleteing the product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await repo.GetBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await repo.GetTypesAsync());
        }
    }
}
