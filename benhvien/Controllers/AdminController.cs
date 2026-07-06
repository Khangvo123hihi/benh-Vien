using benhvien.Data;
using benhvien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Thêm dòng này để dùng Include nếu cần

namespace benhvien.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Gộp trang Report vào Index để vừa vào Admin là có luôn Dashboard thống kê
        public IActionResult Index()
        {
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalRequests = _context.BloodRequests.Count();

            // Sửa lại cho đúng tên bảng: _context.DonationAppointments chứ không phải Donations
            ViewBag.TotalAppointments = _context.DonationAppointments.Count();

            // 🟢 THÊM DÒNG NÀY: Lấy dữ liệu đếm từ SQL View lên để truyền ra giao diện
            ViewBag.BloodStats = _context.BloodTypeStatistics.ToList();

            return View(); // Trả về Admin/Index.cshtml
        }

        // 2. SỬA TẠI ĐÂY: Đổi tên từ Hospitals thành Hospital (Bỏ chữ s) cho khớp với file View của ní
        public IActionResult Hospital()
        {
            // Tận dụng nạp kèm Users luôn để trang danh sách hiển thị số tài khoản đã cấp
            var data = _context.Hospitals.Include(h => h.Users).ToList();

            return View(data); // Trả về Admin/Hospital.cshtml
        }

        // 3. Giữ nguyên trang quản lý Users
        // 3. Giữ nguyên trang quản lý Users
        public IActionResult Users()
        {
            // 🟢 SỬA TẠI ĐÂY: Thêm .Include(u => u.Role) để giao diện hiển thị đúng vai trò
            var users = _context.Users
                                .Include(u => u.Hospital)
                                .Include(u => u.Role) // <--- Chèn thêm dòng này vào đây nhe ní
                                .ToList();

            return View(users); // Trả về Admin/Users.cshtml và kết thúc hàm tại đây
        }

        public IActionResult Donations()
        {
            return View(
                _context.DonationAppointments
                .OrderByDescending(x => x.CreatedAt)
                .ToList());
        }


        // ================= CREATE =================

        // ================= CREATE =================

        [HttpGet]
        public IActionResult CreateHospital()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateHospital(Hospital model) // Sửa từ User sang Hospital
        {
            ModelState.Remove("Description");

            ModelState.Remove("Users");
            ModelState.Remove("BloodRequests");
            if (!ModelState.IsValid)
                return View(model);

            // Thêm các logic tương ứng của bảng Hospital nếu có (ví dụ: ngày tạo...)
            // model.CreatedAt = DateTime.Now; 
            model.CreatedAt = DateTime.Now;
            _context.Hospitals.Add(model); // Thêm vào bảng Hospitals
            _context.SaveChanges();

            TempData["Success"] = "Tạo Hospital thành công";
            return RedirectToAction(nameof(Hospital));
        }


        // ================= EDIT =================
        [HttpGet]
        public IActionResult EditHospital(int id)
        {
            // Tìm kiếm trực tiếp trong bảng danh sách bệnh viện (Hospitals) bằng ID công việc
            var hospital = _context.Hospitals.FirstOrDefault(x => x.Id == id);

            if (hospital == null)
                return NotFound(); // Nếu ID = 1 thực sự không tồn tại trong bảng Hospitals mới lỗi 404

            return View(hospital); // Truyền model Hospital sang cho View
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditHospital(Hospital model) // Sửa từ User sang Hospital
        {
            ModelState.Remove("Description");

            ModelState.Remove("Users");
            ModelState.Remove("BloodRequests");
            if (!ModelState.IsValid)
                return View(model);

            var hospital = _context.Hospitals.Find(model.Id); // Tìm trong bảng Hospitals

            if (hospital == null)
                return NotFound();

            // Cập nhật các trường thông tin dựa theo các thuộc tính có trong class Hospital của bạn
            hospital.Name = model.Name;   // Giả định bảng Hospital dùng trường Name thay vì FullName
            hospital.Email = model.Email;
            hospital.Phone = model.Phone;
            hospital.Address = model.Address; // Cập nhật địa chỉ bệnh viện nếu có

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật Hospital thành công";
            return RedirectToAction(nameof(Hospital));
        }
    }
}
