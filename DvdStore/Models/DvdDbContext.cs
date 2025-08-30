using Microsoft.EntityFrameworkCore;

namespace DvdStore.Models
{
    public class DvdDbContext : DbContext
    {
        public DvdDbContext(DbContextOptions<DvdDbContext> options) : base(options)
        {
        }
        public DbSet<Users> UsersTable { get; set; }
        public DbSet<Products> ProductsTable { get; set; }
    }
}
