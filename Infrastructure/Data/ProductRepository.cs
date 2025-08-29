using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ProductRepository(StoreContext storeContext) : IProductRepository
{
    public void CreateProduct(Product product)
    {
        storeContext.Products.Add(product);
    }
    public void DeleteProduct(Product product)
    {
        storeContext.Products.Remove(product);
    }
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        Product? product = await storeContext.Products.FindAsync(id);
        return product;
    }
    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
        IReadOnlyList<Product> products = await storeContext.Products.ToListAsync();
        return products;
    }
    public async Task<bool> ProductExists(int id)
    {
        return await storeContext.Products.AnyAsync(x => x.Id == id);
    }
    public async Task<bool> SaveChangesAsync()
    {
        return await storeContext.SaveChangesAsync() > 0;
    }
    public void UpdateProduct(Product product)
    {
        storeContext.Entry(product).State = EntityState.Modified;
    }
}
