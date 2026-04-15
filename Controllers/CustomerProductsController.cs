using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebProject.Data;
using MyWebProject.Models;

namespace MyWebProject.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var products = _context.Products.Include(p => p.User).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
                products = products.Where(p => p.Name.Contains(searchString));
            return View(await products.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string searchString)
        {
            var products = _context.Products.Include(p => p.User).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
                products = products.Where(p => p.Name.Contains(searchString));
            var result = await products.ToListAsync();
            return PartialView("_ProductCards", result);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, IsActive = true };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem { CartId = cart.Id, ProductId = productId, Quantity = quantity };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }
            await _context.SaveChangesAsync();

            var totalQuantity = await _context.CartItems.Where(ci => ci.CartId == cart.Id).SumAsync(ci => ci.Quantity);
            var lang = Request.Cookies["UserLanguage"] ?? "ar";
            string message = lang == "ar" ? "تمت إضافة المنتج إلى السلة!" : "Product added to cart!";

            return Json(new { success = true, message = message, cartCount = totalQuantity });
        }
    }
}