using benhvien.Data;
using benhvien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                 .Include(x => x.User)
                .Where(x => bloodRequestIds.Contains(x.BloodRequestId))
                .OrderByDescending(x => x.DonateDate)
                .ToList();

            return View(appointments);
        }

        // =========================
        // DUYỆT
        // =========================
        private void SendNotification(int userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
        }
        public IActionResult Approve(int id)
        {
            var appointment = _context.DonationAppointments
                .FirstOrDefault(x => x.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Approved";

            SendNotification(
                appointment.UserId,
                "Lịch hẹn được duyệt",
                "Lịch hẹn hiến máu của bạn đã được bệnh viện duyệt. Vui lòng đến đúng thời gian đã đăng ký."
            );

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

            SendNotification(
                appointment.UserId,
                "Lịch hẹn bị từ chối",
                "Lịch hẹn hiến máu của bạn đã bị từ chối. Vui lòng liên hệ bệnh viện để biết thêm chi tiết."
            );

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

            SendNotification(
                appointment.UserId,
                "Hiến máu thành công",
                "Cảm ơn bạn đã tham gia hiến máu. Chúc bạn nhiều sức khỏe!"
            );

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

            SendNotification(
                appointment.UserId,
                "Hiến máu không thành công",
                "Buổi hiến máu của bạn chưa thể hoàn thành. Vui lòng liên hệ bệnh viện để biết thêm thông tin."
            );

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

            user.BloodTypeId = model.BloodTypeId;
            user.DateOfBirth = model.DateOfBirth;
            user.Address = model.Address;

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật hồ sơ thành công";

            return RedirectToAction(nameof(UserDetail),
                new { id = user.Id });
        }
        public IActionResult HealthDetail(int userId)
        {
            if (!IsStaff())
                return RedirectToAction("Login", "Account");

            var health = _context.HealthDeclarations
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault(x => x.UserId == userId);

            if (health == null)
            {
                TempData["Error"] = "Người dùng chưa khai báo sức khỏe.";
                return RedirectToAction(nameof(Appointments));
            }

            return View(health);
        }
    }
}