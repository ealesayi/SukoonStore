using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebProject.Data;
using MyWebProject.Models;

namespace MyWebProject.Controllers
{
    [Authorize]  // لا يمكن الوصول إلا بعد تسجيل الدخول
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // عرض المنتجات الخاصة بالمستخدم الحالي فقط مع إضافة معلومات الشركة
        public async Task<IActionResult> Index(string searchString)
        {
            var userId = _userManager.GetUserId(User);
            ViewData["CurrentFilter"] = searchString;

            var products = _context.Products.Where(p => p.UserId == userId);
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            // جلب معلومات الشركة للمستخدم الحالي (البائع)
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CompanyName = currentUser?.CompanyName;
            ViewBag.CompanyLogoUrl = currentUser?.CompanyLogoUrl;

            return View(await products.ToListAsync());
        }

        // عرض نموذج الإضافة
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.UserId = _userManager.GetUserId(User);
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // عرض نموذج التعديل
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.UserId == _userManager.GetUserId(User));
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.UserId == _userManager.GetUserId(User));
                if (existing == null) return NotFound();
                existing.Name = product.Name;
                existing.Price = product.Price;
                existing.Description = product.Description;
                _context.Update(existing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // عرض تأكيد الحذف
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id && m.UserId == _userManager.GetUserId(User));
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.UserId == _userManager.GetUserId(User));
            if (product != null) _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // AJAX للبحث المباشر
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string searchString)
        {
            var userId = _userManager.GetUserId(User);
            var products = _context.Products.Where(p => p.UserId == userId);
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }
            var result = await products.ToListAsync();
            return PartialView("_ProductCards", result);
        }
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UpdateLogo(string logoUrl)
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return Unauthorized();

    user.CompanyLogoUrl = logoUrl;
    var result = await _userManager.UpdateAsync(user);
    if (result.Succeeded)
    {
        return Json(new { success = true, newLogoUrl = logoUrl });
    }
    return Json(new { success = false, message = "حدث خطأ أثناء تحديث الشعار." });
}
    }
    
}