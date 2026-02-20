using System.Reflection;
using System.Text.Json;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext storeContext,UserManager<AppUser> userManager)
    {
        if(!userManager.Users.Any(x => x.UserName == "admin@test.com"))
        {
            var adminUser = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com"
            };
           
            await userManager.CreateAsync(adminUser, "Pa$$w0rd");
            await userManager.AddToRoleAsync(adminUser,"Admin");
        }
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!storeContext.Products.Any()) // check if the database is empty
        {
            var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);
            if (products == null) return;
            storeContext.Products.AddRange(products);
            await storeContext.SaveChangesAsync();
        }
        if (!storeContext.DeliveryMethods.Any()) // check if the database is empty
        {
            var deliveryMethodsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");
            var deliveries = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
            if (deliveries == null) return;
            storeContext.DeliveryMethods.AddRange(deliveries);
            await storeContext.SaveChangesAsync();
        }
    }
}
