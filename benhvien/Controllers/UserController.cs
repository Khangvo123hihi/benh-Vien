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

     
    }
}