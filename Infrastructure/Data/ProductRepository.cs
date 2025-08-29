using System.IO.Compression;
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
    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort)
    {
        var query = storeContext.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(x => x.Brand == brand);
        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(x => x.Type == type);
        
        query = sort switch
        {
           "priceAsc" => query.OrderBy(x => x.Price),
            "priceDesc" => query.OrderByDescending(x => x.Price),
            _ => query.OrderBy(x => x.Name),
        };  
        return await query.ToListAsync();   
    }
    public async Task<IReadOnlyList<string>> GetProductBrands()
    {
        return await storeContext.Products.Select(x => x.Brand).Distinct().ToListAsync();
    }
    public async Task<IReadOnlyList<string>> GetProductTypes()
    {
        return await storeContext.Products.Select(x => x.Type).Distinct().ToListAsync();
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
    public async Task<bool> IsTableEmpty()
    {
        return !await storeContext.Products.AnyAsync();
    }
}
