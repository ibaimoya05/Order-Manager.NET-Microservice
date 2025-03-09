using Microsoft.EntityFrameworkCore;
using ProductCrudBlazor.Models;

namespace ProductCrudBlazor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
