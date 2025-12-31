using Microsoft.EntityFrameworkCore;

namespace Lab02_Bartczak_Rafal
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
            modelBuilder.Entity<CartItem>().ToTable("CartItems");

            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(12, 2);
            modelBuilder.Entity<OrderItem>().Property(p => p.Price).HasPrecision(12, 2);
        }
    }
}
