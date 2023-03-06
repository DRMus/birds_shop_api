namespace birds_shop_api.Models.BirdsShop
{
    public class User
    {
        public long id { get; set; }
        public string? fullname { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }
        public string phone_number { get; set; } = null!;
        public string password { get; set; } = null!;
    }
}
