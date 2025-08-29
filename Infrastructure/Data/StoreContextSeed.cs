using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext storeContext)
    {
        if (!storeContext.Products.Any()) // check if the database is empty
        {
            var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);
            if (products == null) return;
            storeContext.Products.AddRange(products);
            await storeContext.SaveChangesAsync();
        }
    }
}
