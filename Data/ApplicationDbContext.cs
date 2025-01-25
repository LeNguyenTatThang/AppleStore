using Microsoft.EntityFrameworkCore;
using AppleStore.Models;

namespace AppleStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Account> Account { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Product)
                .HasForeignKey(p => p.CategoryId);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)  // Mỗi OrderDetail có 1 Order
                .WithMany(o => o.OrderDetails)  // Mỗi Order có nhiều OrderDetail
                .HasForeignKey(od => od.OrderId)  // Sử dụng OrderId kiểu string làm khóa ngoại
                .HasPrincipalKey(o => o.OrderId);
        }
        public static void EnableSensitiveLogging(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public void EnsureSingleEntityTracking(Product product)
        {
            var trackedProduct = this.ChangeTracker.Entries<Product>()
                .FirstOrDefault(e => e.Entity.Id == product.Id);

            if (trackedProduct != null)
            {
                trackedProduct.State = EntityState.Detached;
            }

            this.Product.Update(product);
        }
    }
}
