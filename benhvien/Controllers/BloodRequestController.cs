using benhvien.Data;
using benhvien.Models;
using Microsoft.AspNetCore.Mvc;

namespace benhvien.Controllers
{
    public class BloodRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LIST ALL REQUESTS
        // =========================
        [HttpGet]
        public IActionResult Index()
        {
            var requests = _context.BloodRequests
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(requests);
        }

        // =========================
        // MY REQUESTS
        // =========================
        [HttpGet]
        public IActionResult MyRequests()
        {
            var hospitalId =
                HttpContext.Session.GetInt32("UserId");

            if (hospitalId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var data = _context.BloodRequests
                .Where(x => x.HospitalId == hospitalId.Value)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(data);
        }

        // =========================
        // CREATE GET
        // =========================
        [HttpGet]
        public IActionResult Create()
        {
            var role =
                HttpContext.Session.GetString("Role");

            if (role != "Hospital")
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            return View();
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BloodRequest request)
        {
            var hospitalId =
                HttpContext.Session.GetInt32("UserId");

            if (hospitalId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            request.HospitalId = hospitalId.Value;

            request.CreatedAt = DateTime.Now;

            _context.BloodRequests.Add(request);

            _context.SaveChanges();

            return RedirectToAction("MyRequest");
        }

        // =========================
        // EDIT GET
        // =========================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var hospitalId =
                HttpContext.Session.GetInt32("UserId");

            if (hospitalId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var request = _context.BloodRequests
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.HospitalId == hospitalId.Value);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BloodRequest request)
        {
            var hospitalId =
                HttpContext.Session.GetInt32("UserId");

            if (hospitalId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var oldRequest = _context.BloodRequests
                .FirstOrDefault(x =>
                    x.Id == request.Id &&
                    x.HospitalId == hospitalId.Value);

            if (oldRequest == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            oldRequest.BloodType =
                request.BloodType;

            oldRequest.QuantityNeeded =
                request.QuantityNeeded;

            oldRequest.Description =
                request.Description;

            oldRequest.IsUrgent =
                request.IsUrgent;

            _context.SaveChanges();

            return RedirectToAction("MyRequest");
        }

        // =========================
        // DELETE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var hospitalId =
                HttpContext.Session.GetInt32("UserId");

            if (hospitalId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var request = _context.BloodRequests
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.HospitalId == hospitalId.Value);

            if (request == null)
            {
                return NotFound();
            }

            _context.BloodRequests.Remove(request);

            _context.SaveChanges();

            return RedirectToAction("MyRequest");
        }
    }
}