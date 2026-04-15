using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyWebProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    CompanyName = model.CompanyName,
                    CompanyLogoUrl = model.CompanyLogoUrl,
                    RegisteredAt = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password ?? string.Empty);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    TempData["Success"] = "تم إنشاء المستخدم بنجاح!";
                    return RedirectToAction(nameof(Users));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userToDelete = await _userManager.FindByIdAsync(id);

            if (userToDelete == null)
            {
                TempData["Error"] = "المستخدم غير موجود.";
                return RedirectToAction(nameof(Users));
            }

            if (userToDelete.UserName == "admin")
            {
                TempData["Error"] = "لا يمكن حذف حساب الأدمن الأساسي (admin).";
                return RedirectToAction(nameof(Users));
            }

            if (currentUser != null && currentUser.Id == id)
            {
                TempData["Error"] = "لا يمكنك حذف حسابك الخاص.";
                return RedirectToAction(nameof(Users));
            }

            var result = await _userManager.DeleteAsync(userToDelete);
            if (result.Succeeded)
            {
                TempData["Success"] = "تم حذف المستخدم بنجاح!";
            }
            else
            {
                TempData["Error"] = "حدث خطأ أثناء حذف المستخدم.";
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                CompanyName = user.CompanyName,
                CompanyLogoUrl = user.CompanyLogoUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Id))
                    return NotFound();

                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                user.Email = model.Email;
                user.FullName = model.FullName;
                user.UserName = model.UserName;
                user.PhoneNumber = model.PhoneNumber;
                user.CompanyName = model.CompanyName;
                user.CompanyLogoUrl = model.CompanyLogoUrl;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "تم تحديث بيانات المستخدم بنجاح!";
                    return RedirectToAction(nameof(Users));
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "تم إعادة تعيين كلمة المرور بنجاح!";
            }
            else
            {
                TempData["Error"] = "فشل إعادة تعيين كلمة المرور. تأكد من أن الكلمة الجديدة تلبي المتطلبات.";
            }
            return RedirectToAction(nameof(Users));
        }
    }

    // ========== ViewModels خارج الـ Controller ==========
    public class EditUserViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        public required string Email { get; set; }

        public string? FullName { get; set; }

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        // حقول الشركة
        public string? CompanyName { get; set; }
        public string? CompanyLogoUrl { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "اسم المستخدم يجب أن يكون بين 3 و 50 حرف")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        public string? FullName { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        // حقول الشركة
        public string? CompanyName { get; set; }
        public string? CompanyLogoUrl { get; set; }
    }
}