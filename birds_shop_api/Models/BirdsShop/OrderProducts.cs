using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace birds_shop_api.Models.BirdsShop
{
    public class OrderProducts
    {
        public long id { get; set; }
        [ForeignKey("Order")]
        public long order_id { get; set; }

        [ForeignKey("Product")]
        public long product_id { get; set;}
        public float total_cost { get; set; }
        public int? weight { get; set; }
        public int count { get; set; }


        [NotMapped]
        public Product? product { get; set; }
        [NotMapped]
        public Order? Order { get; set; }
    }
}
