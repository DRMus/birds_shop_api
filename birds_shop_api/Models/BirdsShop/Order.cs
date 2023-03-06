using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace birds_shop_api.Models.BirdsShop
{
    [PrimaryKey(nameof(order_id))]
    public class Order
    {
        public long order_id { get; set; }
        public long user_id { get; set; }
        public int state { get; set; }
        public DateTime delivery_date { get; set; }
        public int sum_cost { get; set; }

    }
}
