using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AppleStore.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AppleStore.Data;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Account/[action]/{id?}")]

    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách thể loại
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var accounts = await _context.Account
                .Select(a => new
                {
                    a.AccountId,
                    a.Username,
                    a.Email,
                    Password = a.Password ?? "Trống", // Thay NULL bằng "Trống"
                    FullName = a.FullName ?? "Trống", // Thay NULL bằng "Trống"
                    PhoneNumber = a.PhoneNumber ?? "Trống", // Thay NULL bằng "Trống"
                    a.Role,
                    a.Status,
                    CreatedAt = a.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), // Format ngày giờ
                    UpdatedAt = a.UpdatedAt.HasValue ? a.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Trống",
                    ResetPasswordToken = a.ResetPasswordToken ?? "Trống",
                    ResetTokenExpires = a.ResetTokenExpires.HasValue ? a.ResetTokenExpires.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Trống"
                })
                .ToListAsync();

            return View(accounts);
        }


    }
}
