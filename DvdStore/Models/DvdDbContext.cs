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





    }
}
