using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyWebProject.Models;

namespace MyWebProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // تكوين علاقة Product مع ApplicationUser (مفتاح أجنبي)
            builder.Entity<Product>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull); // اختياري: عند حذف المستخدم، لا تحذف منتجاته

            // تحديد دقة الـ decimal (لإزالة التحذير)
            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); // 18 خانة إجمالية، 2 للكسور
        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
    }
}