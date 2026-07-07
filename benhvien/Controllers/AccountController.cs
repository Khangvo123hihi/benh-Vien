using benhvien.Data;
using benhvien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace benhvien.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // REGISTER GET
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER POST
        [HttpPost]
        // REGISTER POST
        [HttpPost]
        public IActionResult Register(User user, int? bloodTypeId) // 🟢 1. THÊM THAM SỐ TÁCH BIỆT Ở ĐÂY
        {
            // Loại bỏ kiểm tra hợp lệ cho các Object liên kết phức tạp
            ModelState.Remove("Role");
            ModelState.Remove("Hospital");
            ModelState.Remove("BloodTypeNavigation");
            ModelState.Remove("BloodType");
            ModelState.Remove("BloodTypeId"); // Cho phép bỏ qua kiểm tra tự động của hệ thống

            if (ModelState.IsValid)
            {
                var checkEmail = _context.Users.FirstOrDefault(x => x.Email == user.Email);
                if (checkEmail != null)
                {
                    ViewBag.Error = "Email đã tồn tại";
                    return View(user);
                }

                // 🟢 2. ÉP GÁN TRỰC TIẾP: Ưu tiên lấy từ tham số form truyền thẳng vào
                if (bloodTypeId.HasValue)
                {
                    user.BloodTypeId = bloodTypeId.Value;
                }
                else
                {
                    // Dự phòng: Nếu vì lý do nào đó tham số bị hụt, đọc trực tiếp qua Request.Form
                    var rawBloodTypeId = Request.Form["BloodTypeId"].ToString();
                    if (!string.IsNullOrEmpty(rawBloodTypeId) && int.TryParse(rawBloodTypeId, out int bloodId))
                    {
                        user.BloodTypeId = bloodId;
                    }
                }

                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                user.RoleId = 4; // Mặc định quyền Tình nguyện viên (User)

                _context.Users.Add(user);
                _context.SaveChanges();

                // Đăng ký thành công thì chuyển hướng sang trang Đăng nhập
                return RedirectToAction("Login");
            }

            // 🟢 3. ĐOẠN CODE KIỂM TRA LỖI ẨN (Bẫy lỗi nếu ModelState bị False mà ko rõ lý do)
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            if (errors.Any())
            {
                ViewBag.Error = "Lỗi dữ liệu Form: " + string.Join(", ", errors);
            }

            return View(user);
        }
        // LOGIN GET
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            // Đăng nhập User
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetInt32("HospitalId", user.HospitalId ?? 0);
                HttpContext.Session.SetString("FullName", user.FullName ?? "");
                HttpContext.Session.SetString("Role", user.Role?.RoleName ?? "");

                switch (user.Role?.RoleName)
                {
                    case "Admin":
                        return RedirectToAction("Index", "Admin");

                    case "Staff":
                        return RedirectToAction("Index", "Staff");

                    case "User":
                        return RedirectToAction("Index", "User");
                }
            }

            // Đăng nhập Hospital
            var hospital = _context.Hospitals
                .FirstOrDefault(h => h.Email == email
                                  && h.Password == password
                                  && h.IsActive);

            if (hospital != null)
            {
                HttpContext.Session.SetInt32("HospitalId", hospital.Id);
                HttpContext.Session.SetString("HospitalName", hospital.Name);
                HttpContext.Session.SetString("Role", "Hospital");

                return RedirectToAction("Index", "Hospital");
            }

            ViewBag.Error = "Sai email hoặc mật khẩu";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
        [HttpGet]
        [HttpGet]
        public IActionResult Profile()
        {
            // Lấy UserId của người dùng đang đăng nhập từ Session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 🟢 CẦN SỬA: Thêm .Include(u => u.BloodType) để nạp bảng nhóm máu đi kèm
            var user = _context.Users
                .Include(u => u.BloodType)
                .FirstOrDefault(u => u.Id == userId.Value);

            if (user == null)
            {
                return NotFound();
            }

            return View(user); // Truyền dữ liệu User qua View để hiển thị
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}