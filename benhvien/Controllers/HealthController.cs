using benhvien.Data;
using benhvien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace benhvien.Controllers
{
    public class HealthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HealthController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Create(int bloodRequestId)
        {
            ViewBag.BloodRequestId = bloodRequestId;
            return View();
        }
        [HttpPost]
        public IActionResult Create(HealthDeclaration model, int bloodRequestId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            model.UserId = userId.Value;
            model.CreatedAt = DateTime.Now;

            _context.HealthDeclarations.Add(model);

            _context.SaveChanges();

            return RedirectToAction("BloodRegister", "Donation",
                new { id = bloodRequestId });
        }
    }
}
