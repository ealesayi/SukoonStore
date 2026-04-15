using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWebProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyWebProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // ===================== LOGIN =====================
        [HttpGet]
        public IActionResult Login(string? returnUrl = "/")
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    var lang = Request.Cookies["UserLanguage"] ?? "ar";

    var user = await _userManager.FindByNameAsync(model.Username!);
    if (user == null && model.Username!.Contains("@"))
        user = await _userManager.FindByEmailAsync(model.Username!);

    if (user == null)
    {
        ModelState.AddModelError("", lang == "ar" ? "❌ لا يوجد حساب بهذا الاسم أو البريد الإلكتروني." : "❌ No account found.");
        return View(model);
    }

    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password!, model.RememberMe, lockoutOnFailure: false);

    if (result.Succeeded)
    {
        // تسجيل الأدوار في Terminal
        var roles = await _userManager.GetRolesAsync(user);
        Console.WriteLine($"✅ {model.Username} logged in with roles: {string.Join(", ", roles)}");

        // توجيه المستخدم حسب دوره
        if (await _userManager.IsInRoleAsync(user, "Customer"))
        {
            return RedirectToAction("Index", "CustomerProducts");
        }
        else
        {
            return RedirectToAction("Index", "Products");
        }
    }

    if (result.IsLockedOut)
        ModelState.AddModelError("", lang == "ar" ? "الحساب مقفل مؤقتاً." : "Account locked out.");
    else
        ModelState.AddModelError("", lang == "ar" ? "❌ كلمة المرور غير صحيحة." : "❌ Incorrect password.");

    return View(model);
}

        // ===================== REGISTER =====================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    RegisteredAt = DateTime.Now
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "CustomerProducts");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // ===================== LOGOUT & ACCESS DENIED =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Products");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

    // ===================== VIEW MODELS =====================
    public class LoginViewModel
    {
        [Required(ErrorMessage = "اسم المستخدم أو البريد الإلكتروني مطلوب")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress]
        public string Email { get; set; }

        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "كلمة المرور غير متطابقة")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}