using benhvien.Data; // 🟢 Cần thêm dòng này để nhận diện ApplicationDbContext
using benhvien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace benhvien.Controllers
{
    public class UserController : Controller
    {
        // 🟢 1. KHAI BÁO BIẾN KẾT NỐI DATABASE
        private readonly ApplicationDbContext _context;

        // 🟢 2. KHỞI TẠO CONSTRUCTOR ĐỂ TIÊM ĐỐI TƯỢNG ĐOẠN ĐƯỢC CHÈN
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "User")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        [HttpGet]
        public IActionResult MyNotification()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // 🟢 SỬA LẠI: Bỏ điều kiện lọc IsRead, lấy hết sạch thông báo của User này ra xem
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId.Value)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(notifications);
        }
        [HttpGet]
        public IActionResult GetLatestUnreadNotification()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { hasNew = false });

            // Tìm thông báo mới nhất của người này mà đang ở trạng thái chưa đọc (IsRead == false)
            var latestNoti = _context.Notifications
                .Where(n => n.UserId == userId.Value && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefault();

            if (latestNoti != null)
            {
                // Đánh dấu đã đọc hoặc gán tạm để lần sau quét không bị trùng lặp nổ thông báo lại
                latestNoti.IsRead = true;
                _context.SaveChanges();

                return Json(new
                {
                    hasNew = true,
                    title = latestNoti.Title,
                    message = latestNoti.Message
                });
            }

            return Json(new { hasNew = false });
        }
    }
}