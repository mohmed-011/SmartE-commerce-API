using Microsoft.EntityFrameworkCore;

namespace SmartE_commerce.Data
{
    public class ApplicationDbContext : DbContext
    {   
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToTable("Item").HasKey("Item_ID");
            modelBuilder.Entity<Category>().ToTable("Category").HasKey("Category_ID");
            modelBuilder.Entity<Cart>().ToTable("Cart").HasKey("Item_ID", "Buyer_ID");

        }
    }
}
