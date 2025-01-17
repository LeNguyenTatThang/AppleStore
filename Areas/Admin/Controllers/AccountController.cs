using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AppleStore.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AppleStore.Data;

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
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            Console.WriteLine(role);

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

                var accounts = _context.Account.ToList();
                return View(accounts);
        }
    }
}
