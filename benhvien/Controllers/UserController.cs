using Microsoft.AspNetCore.Mvc;

namespace benhvien.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            var role =
                HttpContext.Session.GetString("Role");

            if (role != "User")
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            return View();
        }
    }
}