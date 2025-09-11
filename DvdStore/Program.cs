using DvdStore.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register the DbContext with Dependency Injection container
builder.Services.AddDbContext<DvdDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "products",
    pattern: "products",
    defaults: new { controller = "Home", action = "Products" });

app.MapControllerRoute(
    name: "productDetail",
    pattern: "product/{id}",
    defaults: new { controller = "ProductDetail", action = "Index" });

app.MapControllerRoute(
    name: "news",
    pattern: "news",
    defaults: new { controller = "News", action = "Index" });

app.Run();
