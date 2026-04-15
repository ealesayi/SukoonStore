using System.ComponentModel.DataAnnotations;

namespace MyWebProject.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [StringLength(50, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress]
        public required string Email { get; set; }

        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }

        [Compare("Password", ErrorMessage = "كلمة المرور غير متطابقة")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }
    }
}