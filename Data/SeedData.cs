using Microsoft.AspNetCore.Identity;
using MyWebProject.Models;

namespace MyWebProject.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. إنشاء الأدوار
            string adminRole = "Admin";
            string userRole = "User";
            string customerRole = "Customer";

            if (!await roleManager.RoleExistsAsync(adminRole))
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            if (!await roleManager.RoleExistsAsync(userRole))
                await roleManager.CreateAsync(new IdentityRole(userRole));
            if (!await roleManager.RoleExistsAsync(customerRole))
                await roleManager.CreateAsync(new IdentityRole(customerRole));

            // 2. إنشاء مستخدم Admin إذا لم يكن موجوداً
            string adminEmail = "admin@example.com";
            string adminUser = "admin";
            if (await userManager.FindByNameAsync(adminUser) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminUser,
                    Email = adminEmail,
                    FullName = "مدير النظام",
                    RegisteredAt = DateTime.Now
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, adminRole);
                }
            }

            // 3. (اختياري) إنشاء مستخدم تجريبي من نوع Customer
            if (await userManager.FindByNameAsync("customer1") == null)
            {
                var customer = new ApplicationUser
                {
                    UserName = "customer1",
                    Email = "customer1@example.com",
                    FullName = "زبون تجريبي",
                    RegisteredAt = DateTime.Now
                };
                await userManager.CreateAsync(customer, "Customer@123");
                await userManager.AddToRoleAsync(customer, customerRole);
            }
        }
    }
}