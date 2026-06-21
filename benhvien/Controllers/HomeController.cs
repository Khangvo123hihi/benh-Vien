using benhvien.Data;
using benhvien.Models;
using Microsoft.AspNetCore.Mvc;

namespace benhvien.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

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
            var hospitals = _context.Hospitals.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                hospitals = hospitals.Where(h => h.Name.Contains(keyword));
            }

            return View(hospitals.ToList());
        }
    }
}