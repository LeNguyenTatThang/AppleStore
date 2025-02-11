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
                    Password = a.Password ?? "Trống", 
                    FullName = a.FullName ?? "Trống", 
                    PhoneNumber = a.PhoneNumber ?? "Trống",
                    a.Role,
                    a.Status,
                    CreatedAt = a.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), 
                    UpdatedAt = a.UpdatedAt.HasValue ? a.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Trống",
                    ResetPasswordToken = a.ResetPasswordToken ?? "Trống",
                    ResetTokenExpires = a.ResetTokenExpires.HasValue ? a.ResetTokenExpires.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Trống"
                })
                .ToListAsync();

            return View(accounts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string username, string fullname, string password, string email)
        {
            var existingUser = await _context.Account
                                              .FirstOrDefaultAsync(u => u.Username == username);

            if (existingUser != null)
            {
                TempData["error"] = "Tên tài khoản đã tồn tại.";
                ModelState.AddModelError("Username", "Tên tài khoản đã tồn tại.");
                return View();
            }

            var existingEmail = await _context.Account
                                               .FirstOrDefaultAsync(u => u.Email == email);

            if (existingEmail != null)
            {
                TempData["error"] = "Email đã được đăng ký.";
                ModelState.AddModelError("Email", "Email đã được đăng ký.");
                return View();
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var newAccount = new Account
            {
                Username = username,
                FullName = fullname,
                Password = passwordHash,
                Email = email,
                CreatedAt = DateTime.Now,
                Status = "Active",
                Role = "Admin"
            };

            _context.Account.Add(newAccount);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Account");
        }
    }
}
