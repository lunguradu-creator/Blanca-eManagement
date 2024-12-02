using Blanca_eManagement.Data;
using Blanca_eManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurarea conexiunii la baza de date
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Configurarea Identity cu opțiuni personalizate
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Dezactivează confirmarea contului
    options.User.RequireUniqueEmail = false;        // Permite emailuri ne-unice
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // Setează caracterele permise pentru numele de utilizator
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders(); // Activează token-urile pentru resetarea parolei, confirmarea emailului etc.

// 3. Adăugarea suportului pentru controlere și vizualizări
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Permite suport pentru Razor Pages (opțional)

// 4. Crearea aplicației
var app = builder.Build();


// 5. Seed pentru roluri la pornirea aplicației
await SeedRoles(app.Services);

// 6. Configurarea pipeline-ului aplicației
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware-ul aplicației
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Middleware pentru autentificare
app.UseAuthorization();  // Middleware pentru autorizare

// Rutele aplicației
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Ruta implicită

app.MapControllerRoute(
    name: "products",
    pattern: "{controller=Products}/{action=ProductsManagement}/{id?}"); // Ruta pentru produse

app.MapControllerRoute(
    name: "admin_categories",
    pattern: "Admin/Categories/{action=CategoriesManagement}/{id?}",
    defaults: new { controller = "Categories" });

app.MapRazorPages(); // Rute pentru Razor Pages (opțional)

// Rularea aplicației
app.Run();

// 7. Seed pentru roluri
async Task SeedRoles(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Administrator", "Ospatar", "Analist" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    var app = builder.Build();

}
