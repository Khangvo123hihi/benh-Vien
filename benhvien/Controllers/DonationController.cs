using benhvien.Data;
using Microsoft.AspNetCore.Mvc;
using benhvien.Models;
namespace benhvien.Controllers
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]


        public IActionResult BloodRegister(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var user = _context.Users
                .FirstOrDefault(x => x.Id == userId);

            var model = new DonationAppointment
            {
                BloodType = user.BloodType
            };

            ViewBag.BloodRequestId = id;

            return View(model);
        }

        [HttpPost]
        public IActionResult BloodRegister(
            int BloodRequestId,
            DateTime DonateDate,
            string Location,
            string HospitalName,
            string Note)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users
                .FirstOrDefault(x => x.Id == userId.Value);

            var request = _context.BloodRequests
                .FirstOrDefault(x => x.Id == BloodRequestId);

            if (user == null || request == null)
                return NotFound();

            if (user.BloodType != request.BloodType)
            {
                ModelState.AddModelError("",
                    $"Yêu cầu cần nhóm máu {request.BloodType}, nhóm máu của bạn là {user.BloodType}");

                ViewBag.BloodRequestId = BloodRequestId;

                return View();
            }

            var appointment = new DonationAppointment
            {
                UserId = user.Id,
                BloodRequestId = BloodRequestId,
                DonateDate = DonateDate,
                Location = Location,
                HospitalName = HospitalName,
                BloodType = user.BloodType,
                Note = Note,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.DonationAppointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("History");
        }
        [HttpGet]
        public IActionResult History()
        {
            var userId =
                HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var history = _context.DonationAppointments
                .Where(x => x.UserId == userId.Value)
                .OrderByDescending(x => x.DonateDate)
                .ToList();

            return View(history);
        }
    }
}