using AppleStore.Controllers;
using AppleStore.Data;
using AppleStore.Libraries;
using AppleStore.Models.Momo;
using AppleStore.Services;
using AppleStore.Services.Momo;
using AppleStore.Services.Vnpay;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:44379")
                .AllowAnyHeader()
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });
});
builder.Services.AddScoped<EmailService>();
//Connect MomoAPI
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<VnPayLibrary>();
// Cấu hình DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDb"));
});

// Cấu hình session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Cấu hình Controllers và Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

// Cấu hình middleware
app.UseRouting(); // Đặt UseRouting trước UseAuthentication và UseAuthorization


app.UseSession(); // Sử dụng session

// Cấu hình endpoint
app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "Order",
    pattern: "{controller=Order}/{action=Index}/{OrderId?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
