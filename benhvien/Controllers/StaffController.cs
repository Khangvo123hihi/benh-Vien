using benhvien.Data;
using benhvien.Models;
using Microsoft.AspNetCore.Mvc;

namespace benhvien.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetHospitalId()
        {
            return HttpContext.Session.GetInt32("HospitalId") ?? 0;
        }

        private bool IsStaff()
        {
            return HttpContext.Session.GetString("Role") == "Staff";
        }

        // =========================
        // DASHBOARD
        // =========================

        public IActionResult Index()
        {
            if (!IsStaff())
                return RedirectToAction("Login", "Account");

            int hospitalId = GetHospitalId();

            ViewBag.TotalRequests = _context.BloodRequests
                .Count(x => x.HospitalId == hospitalId);

            ViewBag.TotalAppointments =
                (from a in _context.DonationAppointments
                 join b in _context.BloodRequests
                 on a.BloodRequestId equals b.Id
                 where b.HospitalId == hospitalId
                 select a).Count();

            ViewBag.PendingAppointments =
                (from a in _context.DonationAppointments
                 join b in _context.BloodRequests
                 on a.BloodRequestId equals b.Id
                 where b.HospitalId == hospitalId
                 && a.Status == "Pending"
                 select a).Count();

            return View();
        }

        // =========================
        // DANH SÁCH YÊU CẦU MÁU
        // =========================

        public IActionResult BloodRequests()
        {
            if (!IsStaff())
                return RedirectToAction("Login", "Account");

            int hospitalId = GetHospitalId();

            var requests = _context.BloodRequests
                .Where(x => x.HospitalId == hospitalId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(requests);
        }

        // =========================
        // DANH SÁCH LỊCH HẸN
        // =========================

        public IActionResult Appointments()
        {
            if (!IsStaff())
                return RedirectToAction("Login", "Account");

            int hospitalId = GetHospitalId();

            var bloodRequestIds = _context.BloodRequests
                .Where(x => x.HospitalId == hospitalId)
                .Select(x => x.Id)
                .ToList();

            var appointments = _context.DonationAppointments
                .Where(x => bloodRequestIds.Contains(x.BloodRequestId))
                .OrderByDescending(x => x.DonateDate)
                .ToList();

            return View(appointments);
        }

        // =========================
        // DUYỆT
        // =========================

        public IActionResult Approve(int id)
        {
            var appointment = _context.DonationAppointments
                .FirstOrDefault(x => x.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Approved";

            _context.SaveChanges();

            TempData["Success"] = "Đã duyệt lịch hẹn";

            return RedirectToAction(nameof(Appointments));
        }

        // =========================
        // TỪ CHỐI
        // =========================

        public IActionResult Reject(int id)
        {
            var appointment = _context.DonationAppointments
                .FirstOrDefault(x => x.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Rejected";

            _context.SaveChanges();

            TempData["Success"] = "Đã từ chối lịch hẹn";

            return RedirectToAction(nameof(Appointments));
        }

        // =========================
        // HIẾN THÀNH CÔNG
        // =========================

        public IActionResult Complete(int id)
        {
            var appointment = _context.DonationAppointments
                .FirstOrDefault(x => x.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Completed";

            _context.SaveChanges();

            TempData["Success"] = "Hiến máu thành công";

            return RedirectToAction(nameof(Appointments));
        }

        // =========================
        // HIẾN THẤT BẠI
        // =========================

        public IActionResult Failed(int id)
        {
            var appointment = _context.DonationAppointments
                .FirstOrDefault(x => x.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Failed";

            _context.SaveChanges();

            TempData["Success"] = "Hiến máu thất bại";

            return RedirectToAction(nameof(Appointments));
        }

        // =========================
        // CHI TIẾT USER
        // =========================

        public IActionResult UserDetail(int id)
        {
            if (!IsStaff())
                return RedirectToAction("Login", "Account");

            var user = _context.Users
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // =========================
        // UPDATE HEALTH
        // =========================

        [HttpGet]
        public IActionResult UpdateHealth(int id)
        {
            if (!IsStaff())
                return RedirectToAction("Login", "Account");

            var user = _context.Users
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateHealth(User model)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Id == model.Id);

            if (user == null)
                return NotFound();

            user.BloodType = model.BloodType;
            user.Age = model.Age;
            user.Address = model.Address;

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật hồ sơ thành công";

            return RedirectToAction(nameof(UserDetail),
                new { id = user.Id });
        }
    }
}