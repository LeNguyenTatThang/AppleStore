using Microsoft.EntityFrameworkCore;
using AppleStore.Models;

namespace AppleStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Account> Account { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ giữa Product và Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Product)
                .HasForeignKey(p => p.CategoryId);
        }

        // Phương thức này để enable sensitive data logging (cho việc debug, không dùng trong production)
        public static void EnableSensitiveLogging(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        // Đảm bảo chỉ có một instance của một entity với cùng key được attach vào DbContext
        public void EnsureSingleEntityTracking(Product product)
        {
            var trackedProduct = this.ChangeTracker.Entries<Product>()
                .FirstOrDefault(e => e.Entity.Id == product.Id);

            if (trackedProduct != null)
            {
                // Nếu entity đã được track, detach entity cũ
                trackedProduct.State = EntityState.Detached;
            }

            // Thực hiện cập nhật entity hoặc thêm mới
            this.Product.Update(product);
        }
    }
}
