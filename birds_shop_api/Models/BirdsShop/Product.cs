using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace birds_shop_api.Models.BirdsShop
{
    [PrimaryKey(nameof(product_id))]
    public class Product
    {
        public long product_id { get; set; }
        public long category_id { get; set; }
        public string name { get; set; } = null!;
        public string? second_name { get; set; }
        public string? description { get; set;}
        public int cost { get; set;}
        public string image { get; set; } = null!;
    }
}
