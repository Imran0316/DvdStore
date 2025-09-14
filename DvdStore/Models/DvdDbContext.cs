using Microsoft.EntityFrameworkCore;

namespace DvdStore.Models
{
    public class DvdDbContext : DbContext
    {
        public DvdDbContext(DbContextOptions<DvdDbContext> options) : base(options)
        {
        }
        public DbSet<Users> tbl_Users { get; set; }
        public DbSet<Artists> tbl_Artists { get; set; }
        public DbSet<Category> tbl_Category { get; set; }
        public DbSet<Albums> tbl_Albums { get; set; }
        public DbSet<Orders> tbl_Orders { get; set; }
        public DbSet<OrderDetails> tbl_OrderDetails { get; set; }
        public DbSet<Producers> tbl_Producers { get; set; }
        public DbSet<Suppliers> tbl_Suppliers { get; set; }
        public DbSet<Payments> tbl_Payments { get; set; }
        public DbSet<Products> tbl_Products { get; set; }
        public DbSet<Cart> tbl_Carts { get; set; }
        public DbSet<CartItems> tbl_CartItems { get; set; }
        public DbSet<PurchaseInvoice> tbl_PurchaseInvoices { get; set; }
        public DbSet<PurchaseInvoiceDetail> tbl_PurchaseInvoiceDetails { get; set; }
        public DbSet<Songs> tbl_Songs { get; set; }
        // Hero Image
        public DbSet<HeroImage> tbl_HeroImages { get; set; }
        public DbSet<ProductReviews> tbl_ProductReviews { get; set; }
        public DbSet<NewsPromotion> tbl_NewsPromotions { get; set; }
        public DbSet<Wishlist> tbl_Wishlists { get; set; }
        public DbSet<Feedback> tbl_Feedbacks { get; set; }
        public DbSet<DownloadHistory> tbl_Downloads { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the relationship between Product and ProductReview
            modelBuilder.Entity<ProductReviews>()
                .HasOne(pr => pr.Product)
                .WithMany(p => p.tbl_ProductReviews)
                .HasForeignKey(pr => pr.ProductID)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false); // ADD THIS LINE

            modelBuilder.Entity<ProductReviews>()
                .HasOne(pr => pr.User)
                .WithMany()
                .HasForeignKey(pr => pr.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Add this to make the relationship optional
            modelBuilder.Entity<Products>()
                .HasMany(p => p.tbl_ProductReviews)
                .WithOne(pr => pr.Product)
                .HasForeignKey(pr => pr.ProductID)
                .IsRequired(false);

            modelBuilder.Entity<NewsPromotion>()
    .HasOne(np => np.Product)
    .WithMany()
    .HasForeignKey(np => np.ProductID)
    .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
