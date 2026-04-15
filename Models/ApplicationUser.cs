using Microsoft.AspNetCore.Identity;

namespace MyWebProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        // حقول الشركة
        public string? CompanyName { get; set; }
        public string? CompanyLogoUrl { get; set; }
    }
}