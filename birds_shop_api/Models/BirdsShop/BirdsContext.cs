using Microsoft.EntityFrameworkCore;

namespace birds_shop_api.Models.BirdsShop
{
    public class BirdsContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Order> orders { get; set; }


        public BirdsContext(DbContextOptions<BirdsContext> options)
       : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=birdsShop;Username=postgres;Password=12345678");
        }
    }
}
