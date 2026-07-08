using benhvien.Data;
using benhvien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace benhvien.Controllers
{
    public class HospitalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HospitalController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetHospitalId()
        {
            return HttpContext.Session.GetInt32("HospitalId") ?? 0;
        }
        // Dashboard
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Hospital")
            {
                return RedirectToAction("Login", "Account");
            }

            int hospitalId = GetHospitalId();

            var data = _context.BloodRequests
                .Where(x => x.HospitalId == hospitalId)
                .ToList();

            return View(data);
        }

        // =========================
        // CREATE
        // =========================

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BloodRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            // 1. Lấy ID của người dùng đang đăng nhập (ví dụ từ Claims/Session)
            // Tùy thuộc vào cách bạn làm Authentication, ví dụ:
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            // 2. Gán ID đó vào request trước khi lưu
            // request.UserId = userId; // (Thay 'UserId' bằng tên thuộc tính thực tế trong model của bạn)

            request.HospitalId = GetHospitalId();
            request.CreatedAt = DateTime.Now;

            _context.BloodRequests.Add(request);
            _context.SaveChanges();

            // Tìm người dùng cùng nhóm máu
            var users = _context.Users
                .Include(u => u.BloodType)
                .Where(u => u.IsActive
                    && u.BloodType != null
                    && (u.BloodType.ABO + u.BloodType.Rh) == request.BloodType)
                .ToList();

            foreach (var user in users)
            {
                Notification notification = new Notification
                {
                    UserId = user.Id,
                    HospitalId = request.HospitalId,
                    Title = "🩸 Có yêu cầu hiến máu mới",

                    Message =
                        $"Bệnh viện đang cần nhóm máu {request.BloodType}. " +
                        $"Số lượng: {request.QuantityNeeded}. " +
                        $"Vui lòng đăng ký nếu bạn có thể hỗ trợ.",

                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                _context.Notifications.Add(notification);
            }

            _context.SaveChanges();
            TempData["Success"] = "Đăng yêu cầu máu thành công";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================

        public IActionResult Details(int id)
        {
            int hospitalId = GetHospitalId();

            var request = _context.BloodRequests
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.HospitalId == hospitalId);

            if (request == null)
                return NotFound();

            return View(request);
        }

        // =========================
        // EDIT
        // =========================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            int hospitalId = GetHospitalId();

            var request = _context.BloodRequests
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.HospitalId == hospitalId);

            if (request == null)
                return NotFound();

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BloodRequest model)
        {
            int hospitalId = GetHospitalId();

            var request = _context.BloodRequests
                .FirstOrDefault(x =>
                    x.Id == model.Id &&
                    x.HospitalId == hospitalId);

            if (request == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            request.BloodType = model.BloodType;
            request.QuantityNeeded = model.QuantityNeeded;
            request.Description = model.Description;
            request.IsUrgent = model.IsUrgent;

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật thành công";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================

        [HttpGet]
        public IActionResult Delete(int id)
        {
            int hospitalId = GetHospitalId();

            var request = _context.BloodRequests
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.HospitalId == hospitalId);

            if (request == null)
                return NotFound();

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            int hospitalId = GetHospitalId();

            // 1. Tìm request và đồng thời "lôi" luôn danh sách Appointment đi kèm lên bộ nhớ
            var request = _context.BloodRequests
                .FirstOrDefault(x => x.Id == id && x.HospitalId == hospitalId);

            if (request == null)
                return NotFound();

            // 2. Tìm chính xác lại tất cả các Appointment dựa vào Id của Request
            var relatedAppointments = _context.DonationAppointments
                .Where(a => a.BloodRequestId == id)
                .ToList();

            // 3. Xóa sạch đống Appointment này trước 
            if (relatedAppointments.Any())
            {
                _context.DonationAppointments.RemoveRange(relatedAppointments);

                // Ép Entity Framework lưu đợt 1 để cắt đứt hoàn toàn khóa ngoại dưới DB
                _context.SaveChanges();
            }

            // 4. Giờ thì thoải mái xóa Request gốc mà không sợ bất kỳ ràng buộc nào nữa
            _context.BloodRequests.Remove(request);
            _context.SaveChanges(); // Lưu đợt 2 để hoàn tất xóa bài đăng

            TempData["Success"] = "Xóa thành công";

            return RedirectToAction(nameof(Index));
        }
        public IActionResult StaffList()
        {
            int hospitalId = GetHospitalId();

            var staffs = _context.Users
                .Where(x => x.Role.RoleName == "Staff" && x.HospitalId == hospitalId)
                .ToList();

            return View(staffs);
        }
        [HttpGet]
        public IActionResult CreateStaff()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateStaff(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int hospitalId = GetHospitalId();

            var staffRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Staff");
            if (staffRole != null)
            {
                model.Role = staffRole; // Gán đối tượng cho đối tượng
            }

            model.HospitalId = hospitalId;

            model.CreatedAt = DateTime.Now;

            model.IsActive = true;

            _context.Users.Add(model);

            _context.SaveChanges();

            TempData["Success"] = "Tạo Staff thành công";

            return RedirectToAction(nameof(StaffList));
        }

    }
}