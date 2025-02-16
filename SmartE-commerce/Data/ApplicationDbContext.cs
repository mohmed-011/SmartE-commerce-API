using Microsoft.EntityFrameworkCore;

namespace SmartE_commerce.Data
{
    public class ApplicationDbContext : DbContext
    {   
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemImage> ItemImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ItemImage>()
            //.HasKey(ii => new { ii.Item_Image, ii.Item_ID });

            //modelBuilder.Entity<ItemImage>()
            //    .HasOne<Item>()
            //    .WithMany()
            //    .HasForeignKey(ii => ii.Item_ID);
            base.OnModelCreating(modelBuilder);

           // modelBuilder.Entity<Product>().ToTable("Item").HasKey("Item_ID");
            modelBuilder.Entity<Category>().ToTable("Category").HasKey("Category_ID");
            modelBuilder.Entity<Cart>().ToTable("Cart").HasKey("Item_ID", "Buyer_ID");
            modelBuilder.Entity<Buyer>().ToTable("Buyer").HasKey("Buyer_ID");
            modelBuilder.Entity<Seller>().ToTable("Seller").HasKey("Seller_ID");



        }
    }
}
