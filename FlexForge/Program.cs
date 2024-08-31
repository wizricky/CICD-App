using FlexForge.Domain.Identity;
using FlexForge.Repository;
using FlexForge.Repository.Implementation;
using FlexForge.Repository.Interface;
using FlexForge.Service.Interface;
using FlexForge.Service.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlexForge.Domain.Domain;
using FlexForge.Services.Interface;
using FlexForge.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Retrieve connection string from environment variables
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<FlexForgeApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<IFavoriteProductsService, FavoriteProductsService>();
builder.Services.AddTransient<ICategoriesService, CategoriesService>();

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<FlexForgeApplicationUser>>();
    string email = "admin@gmail.com";
    string password = "Ristematej123!";
    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new FlexForgeApplicationUser
        {
            Email = email,
            UserName = "Admin",
            FirstName = "Admin",
            LastName = "Flexforge",
            Address = "Admin address 7",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }

    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!context.Sizes.Any())
    {
        context.Sizes.AddRange(
            new Size { SizeType = "XS" },
            new Size { SizeType = "S" },
            new Size { SizeType = "M" },
            new Size { SizeType = "L" },
            new Size { SizeType = "XL" },
            new Size { SizeType = "XXL" },
            new Size { SizeType = "6-8y" },
            new Size { SizeType = "8-10y" },
            new Size { SizeType = "10-12y" },
            new Size { SizeType = "12-14y" }
        );
        await context.SaveChangesAsync();
    }
}

app.Run();
