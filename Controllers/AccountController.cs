using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleStore.Data;
using AppleStore.Models;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // Kiểm tra tài khoản và mật khẩu từ cơ sở dữ liệu
        var user = await _context.Account
                                  .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            // Lưu thông tin vào session
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role);

            // Kiểm tra vai trò của người dùng
            if (user.Role == "Admin")
            {
                // Chuyển hướng tới Admin Dashboard nếu là Admin
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            else if (user.Role == "User")
            {
                // Chuyển hướng tới Home nếu là User
                return RedirectToAction("Index", "Home");
            }
        }
        else
        {
            // Nếu đăng nhập không thành công, thêm lỗi vào model
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        // Mặc định trả về view login nếu không thành công
        return View();
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // Xóa toàn bộ dữ liệu Session
        return RedirectToAction("Login", "Account");
    }
}
