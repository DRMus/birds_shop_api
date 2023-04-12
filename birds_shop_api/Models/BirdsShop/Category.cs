using Microsoft.EntityFrameworkCore;

namespace birds_shop_api.Models.BirdsShop
{
    [PrimaryKey(nameof(category_id))]
    public class Category
    {
        public long category_id { get; set; }
        public string? category { get; set; }
        public bool has_weight { get; set; }
    }
}
