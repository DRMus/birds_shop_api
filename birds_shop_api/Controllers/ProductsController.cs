using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using birds_shop_api.Models.BirdsShop;
using birds_shop_api.Utils;
using Microsoft.AspNetCore.WebUtilities;

namespace birds_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly BirdsContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(BirdsContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _hostEnvironment = environment;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Getproducts()
        {
            if (_context.products == null)
            {
                return NotFound();
            }

            return await _context.products.ToListAsync();
        }

        [HttpGet("page/{category_id}/{page}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductPages(int category_id, int page)
        {
            if (_context.products == null)
            {
                return NotFound();
            }
            List<Product> products = await _context.products.ToListAsync();
            int pageLength = products.Count / 8 + 1;

            products = products
                .FindAll(item => item.category_id == category_id)
                .OrderBy(item => item.product_id)
                .Skip(8 * (page - 1))
                .Take(8)
                .ToList();

            return Ok(new { pages = pageLength, products });
        }

        [HttpGet("limit/{category_id}/{count}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductLimit(int category_id, int count)
        {
            if (_context.products == null)
            {
                return NotFound();
            }
            List<Product> products = await _context.products.ToListAsync();
            products = products
                .FindAll(item => item.category_id == category_id)
                .Take(count)
                .ToList();

            return products;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            if (_context.products == null)
            {
                return NotFound();
            }
            Product? product = await _context.products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product product)
        {
            if (id != product.product_id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (_context.products == null)
            {
                return Problem("Entity set 'BirdsContext.products'  is null.");
            }
            _context.products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.product_id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            if (_context.products == null)
            {
                return NotFound();
            }
            var product = await _context.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(long id)
        {
            return (_context.products?.Any(e => e.product_id == id)).GetValueOrDefault();
        }
    }
}
