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
        if (!storeContext.DeliveryMethods.Any()) // check if the database is empty
        {
            var deliveryMethodsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/delivery.json");
            var deliveries = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
            if (deliveries == null) return;
            storeContext.DeliveryMethods.AddRange(deliveries);
            await storeContext.SaveChangesAsync();
        }
    }
}
