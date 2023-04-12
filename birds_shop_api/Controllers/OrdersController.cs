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
    public class OrdersController : ControllerBase
    {
        private readonly BirdsContext _context;

        public OrdersController(BirdsContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Getorders()
        {
            if (_context.orders == null)
            {
                return NotFound();
            }
            var orders = await _context.orders.ToListAsync();
            orders.ForEach(item =>
            {
                var query = from t in _context.orders
                            join t2 in _context.users on t.user_id equals t2.id
                            select t2;

                item.User = query.First();
            });

            return orders;
        }

        // GET: api/Orders/5
        [HttpGet("{user_id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(long user_id)
        {
            if (_context.orders == null)
            {
                return NotFound();
            }

            var query = from order in _context.orders
                        where order.user_id == user_id
                        select order;

            List<Order> orders = new List<Order>();
            query.ToList().ForEach(order =>
            {
                List<OrderProducts> newProducts = new List<OrderProducts>();
                var relations = from relation in _context.ordersProducts
                                where order.order_id == relation.order_id
                                select relation;
                relations.ToList().ForEach(relation =>
                {
                    var products = from product in _context.products
                                   where relation.product_id == product.product_id
                                   select product;

                    products.ToList().ForEach(product =>
                    {
                        newProducts.Add(new OrderProducts()
                        {
                            product_id = product.product_id,
                            order_id = order.order_id,
                            count = relation.count,
                            id = relation.id,
                            product = product,
                            total_cost = relation.total_cost,
                            weight = relation.weight
                        });

                    });
                });
                orders.Add(new Order()
                {
                    delivery_date = order.delivery_date,
                    order_id = order.order_id,
                    state = order.state,
                    sum_cost = order.sum_cost,
                    user_id = user_id,
                    OrderProducts = newProducts
                });
            });

            return Ok(orders.AsEnumerable());
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(long id, Order order)
        {
            if (id != order.order_id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (_context.orders == null)
            {
                return Problem("Entity set 'BirdsContext.orders'  is null.");
            }

            _context.orders.Add(order);
            await _context.SaveChangesAsync();

            if (order.OrderProducts != null)
            {
                var newProductList = from item in order.OrderProducts

                                     select new OrderProducts()
                                     {
                                         order_id = order.order_id,
                                         product_id = item.product.product_id,
                                         total_cost = item.total_cost,
                                         weight = item.weight,
                                         count = item.count,
                                         Order = null,
                                         product = item.product,
                                     };
                order.OrderProducts = null;
                _context.ordersProducts.AddRange(newProductList);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(PostOrder), new { id = order.order_id }, order);
            }

            return BadRequest();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            if (_context.orders == null)
            {
                return NotFound();
            }
            var order = await _context.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(long id)
        {
            return (_context.orders?.Any(e => e.order_id == id)).GetValueOrDefault();
        }
    }
}
