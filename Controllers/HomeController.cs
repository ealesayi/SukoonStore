using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace MyWebProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                // التحقق من دور المستخدم
                if (User.IsInRole("Customer"))
                {
                    return RedirectToAction("Index", "CustomerProducts");
                }
                else // Admin أو User (بائع)
                {
                    return RedirectToAction("Index", "Products");
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
                Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, cookieValue);
            }
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}