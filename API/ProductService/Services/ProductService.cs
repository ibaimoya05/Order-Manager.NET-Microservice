using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;


namespace ProductService.Services
{
    public class ProductService{
        private readonly AppDbContext _context;

        public ProductService(AppDbContext  context){
            _context = context;
        }

        // Get all products
        public async Task<List<Product>> GetProductsAsync(){
            return await _context.Products.ToListAsync();
        }

        // Get product by id
        public async Task<Product> GetProductByIdAsync(int id){
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return product;
        }

        // Create a product
        public async Task<Product> CreateProductAsync(Product product){
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }


        // Update a product
        public async Task<Product> UpdateProductAsync(Product product){
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return product;
        }


        // Delete a product
        public async Task DeleteProductAsync(int id){
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
        }
    }
}