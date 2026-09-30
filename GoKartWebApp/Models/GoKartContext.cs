using System.Data.Entity;

namespace GoKartWebApp.Models
{
    public class GoKartContext : DbContext
    {
        public GoKartContext() : base("name=GoKartContext") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
    }
}