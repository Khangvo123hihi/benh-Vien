using benhvien.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace benhvien.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách thông báo của User đang đăng nhập
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var notifications = _context.Notifications
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt)
                .Where(x => x.UserId == userId)
                .ToList();

            return View(notifications);
        }

        // Đánh dấu đã đọc
        public IActionResult Read(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            var notification = _context.Notifications
                .FirstOrDefault(x => x.Id == id && x.UserId == userId);

            if (notification == null)
                return NotFound();
            if (notification == null)
                return NotFound();

            notification.IsRead = true;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // Xóa thông báo
        public IActionResult Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            var notification = _context.Notifications
                .FirstOrDefault(x => x.Id == id && x.UserId == userId);

            if (notification == null)
                return NotFound();
            if (notification == null)
                return NotFound();

            _context.Notifications.Remove(notification);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}