using benhvien.Data;
using Microsoft.AspNetCore.Mvc;
using benhvien.Models;
using Microsoft.EntityFrameworkCore;

namespace benhvien.Controllers
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🟢 FIX GET: Lấy thông tin hiển thị lên Form đăng ký hiến máu
        [HttpGet]
        public IActionResult BloodRegister(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Thêm .Include(x => x.BloodType) để nạp object nhóm máu của user vào bộ nhớ
            var user = _context.Users
                .Include(x => x.BloodType)
                .FirstOrDefault(x => x.Id == userId);

            if (user == null)
                return NotFound();

            // Ghép chuỗi ABO và Rh (Ví dụ: "O" + "+" = "O+") để gán vào thuộc tính string của model
            string userBloodStr = user.BloodType != null ? (user.BloodType.ABO + user.BloodType.Rh) : "";

            var model = new DonationAppointment
            {
                BloodType = userBloodStr // 🟢 Hết lỗi: Đã truyền chuỗi text sạch thay vì Object
            };

            ViewBag.BloodRequestId = id;

            return View(model);
        }

        // 🟢 FIX POST: Xử lý lưu lịch hẹn đăng ký hiến máu khi submit form
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

            // Thêm .Include(x => x.BloodType) ở đây nữa nhé ní
            var user = _context.Users
                .Include(x => x.BloodType)
                .FirstOrDefault(x => x.Id == userId.Value);

            var request = _context.BloodRequests
                .FirstOrDefault(x => x.Id == BloodRequestId);

            if (user == null || request == null)
                return NotFound();

            // Ghép chuỗi nhóm máu của User (Ví dụ từ Object dịch ra thành chữ "O+")
            string userBloodStr = user.BloodType != null ? (user.BloodType.ABO + user.BloodType.Rh) : "";

            // Giả định request.BloodType dưới database của bạn đang lưu dạng chuỗi chữ (Ví dụ: "O+")
            if (userBloodStr != request.BloodType)
            {
                ModelState.AddModelError("",
                    $"Yêu cầu cần nhóm máu {request.BloodType}, nhóm máu của bạn là {userBloodStr}");

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
                BloodType = userBloodStr, // 🟢 Hết lỗi: Gán chuỗi string kết hợp hoàn chỉnh vào
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
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var history = _context.DonationAppointments
                .Where(x => x.UserId == userId.Value)
                .OrderByDescending(x => x.DonateDate)
                .ToList();

            return View(history);
        }
    }
}