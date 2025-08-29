using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IProductRepository,ProductRepository>(); // this service only exists for the lifetime of a HTTP request

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapControllers();
try
{
    //apply any pending ef migrations
    using var scope = app.Services.CreateScope(); //scope that is disposed of after it finishes executing
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    //seed data from a JSON file after applying pending migrations
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}
app.Run();
