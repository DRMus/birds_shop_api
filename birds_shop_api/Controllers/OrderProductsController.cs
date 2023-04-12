using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using birds_shop_api.Models.BirdsShop;

namespace birds_shop_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderProductsController : ControllerBase
    {
        private readonly BirdsContext _context;

        public OrderProductsController(BirdsContext context)
        {
            _context = context;
        }

        // GET: api/OrderProducts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderProducts>>> GetordersProducts()
        {
          if (_context.ordersProducts == null)
          {
              return NotFound();
          }
            return await _context.ordersProducts.ToListAsync();
        }

        // GET: api/OrderProducts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderProducts>> GetOrderProducts(long id)
        {
          if (_context.ordersProducts == null)
          {
              return NotFound();
          }
            var orderProducts = await _context.ordersProducts.FindAsync(id);

            if (orderProducts == null)
            {
                return NotFound();
            }

            return orderProducts;
        }

        // PUT: api/OrderProducts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderProducts(long id, OrderProducts orderProducts)
        {
            if (id != orderProducts.id)
            {
                return BadRequest();
            }

            _context.Entry(orderProducts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderProductsExists(id))
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

        // POST: api/OrderProducts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderProducts>> PostOrderProducts(OrderProducts orderProducts)
        {
          if (_context.ordersProducts == null)
          {
              return Problem("Entity set 'BirdsContext.ordersProducts'  is null.");
          }
            _context.ordersProducts.Add(orderProducts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderProducts", new { id = orderProducts.id }, orderProducts);
        }

        // DELETE: api/OrderProducts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderProducts(long id)
        {
            if (_context.ordersProducts == null)
            {
                return NotFound();
            }
            var orderProducts = await _context.ordersProducts.FindAsync(id);
            if (orderProducts == null)
            {
                return NotFound();
            }

            _context.ordersProducts.Remove(orderProducts);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderProductsExists(long id)
        {
            return (_context.ordersProducts?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
