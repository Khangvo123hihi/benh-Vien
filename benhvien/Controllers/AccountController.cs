using benhvien.Data;
using benhvien.Models;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // CHECK EMAIL EXIST
                var checkEmail = _context.Users
                    .FirstOrDefault(x => x.Email == user.Email);

                if (checkEmail != null)
                {
                    ViewBag.Error = "Email đã tồn tại";

                    return View(user);
                }

                user.CreatedAt = DateTime.Now;

                user.IsActive = true;

                _context.Users.Add(user);

                _context.SaveChanges();

                return RedirectToAction("Login");
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
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            var user = _context.Users
                .FirstOrDefault(x => x.Email == email && x.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Sai email hoặc mật khẩu";
                return View();
            }

            // SET SESSION
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetInt32("HospitalId", user.HospitalId ?? 0);

            HttpContext.Session.SetString("FullName", user.FullName ?? "");
            HttpContext.Session.SetString("Role", user.Role ?? "User");

            // REDIRECT THEO ROLE
            switch (user.Role)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");

                case "Hospital":
                    return RedirectToAction("Index", "Hospital");

                case "Staff":
                    return RedirectToAction("Index", "Staff");

                case "User":
                    return RedirectToAction("Index", "User");

                default:
                    HttpContext.Session.Clear();
                    ViewBag.Error = "Tài khoản không hợp lệ";
                    return View();
            }
        }
        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Profile()
        {
            // Lấy UserId của người dùng đang đăng nhập từ Session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Tìm thông tin người dùng trong Database (giả sử bảng là Users)
            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
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