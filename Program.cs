using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MyWebProject.Data;
using MyWebProject.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. taskkill /PID 8828 /F
builder.Services.AddControllersWithViews();

// Register DbContext with increased timeout (120 seconds) - مرة واحدة فقط
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.CommandTimeout(120)));

// Add Identity services with relaxed password policy
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;          // لا يشترط أرقام
    options.Password.RequiredLength = 6;            // الحد الأدنى 6 حروف/أرقام
    options.Password.RequireNonAlphanumeric = false; // لا يشترط رموز خاصة
    options.Password.RequireUppercase = false;      // لا يشترط حروف كبيرة
    options.Password.RequireLowercase = false;      // لا يشترط حروف صغيرة
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// إعداد الترجمة
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("ar"),
        new CultureInfo("en")
    };

    options.DefaultRequestCulture = new RequestCulture("ar");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

var app = builder.Build();

var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();   // مهم جداً: تم إضافته
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Seed Data (إنشاء الأدوار والمستخدم Admin تلقائياً) - بشكل متزامن
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services).Wait();   // استخدام .Wait() بدلاً من await
}

app.Run(); 