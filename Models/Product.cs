using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebProject.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(100, MinimumLength = 3)]
        [Display(Name = "اسم المنتج")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000000)]
        [DataType(DataType.Currency)]
        [Display(Name = "السعر")]
        // [Column(TypeName = "decimal(18,2)")]  // يمكنك استخدام هذا بدلاً من HasPrecision
        public decimal Price { get; set; }

        [StringLength(500)]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "تاريخ الإضافة")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // المفتاح الأجنبي إلى المستخدم
        public string? UserId { get; set; }

        // خاصية الملاحة (Navigation Property)
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}