using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed de datos inicial (opcional)
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "PS5 Pro", Price = 799.99m, Stock = 100 }
            );
        }
    }
}