using Microsoft.AspNetCore.Mvc;

namespace benhvien.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DonationGuide()
        {
            return View();
        }
        public IActionResult Search(string keyword)
        {
            ViewBag.Keyword = keyword;

            return View();
        }
    }
}
