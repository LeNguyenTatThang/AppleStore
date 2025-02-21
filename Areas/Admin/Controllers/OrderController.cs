using AppleStore.Data;
using AppleStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        
        private readonly ApplicationDbContext _dbContext;

        public OrderController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Admin/Order/Detail")]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            var orders = _dbContext.Orders.ToList();
            return View(orders);
        }

        [HttpGet]
        [Route("Admin/Order/Detail/{orderId?}")]
        public async Task<IActionResult> Detail(string orderId)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("Mã đơn hàng không hợp lệ.");
            }

            var order = await _dbContext.Orders
                .Include(o => o.OrderDetails) 
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound(new { message = $"Không tìm thấy đơn hàng với mã {orderId}." });
            }

            return View(order);
        }


        [HttpPost]
        [Route("Admin/Order/UpdateStatus")]
        public IActionResult UpdateStatus(int id, string status)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            var order = _dbContext.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = status;
            _dbContext.SaveChanges();

            TempData["success"] = "Trạng thái đơn hàng đã được cập nhật.";
            return RedirectToAction("Index");
        }

        [HttpPost("Admin/Order/Delete")]
        public IActionResult Delete(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            var order = _dbContext.Orders.Find(id);
            
            _dbContext.Orders.Remove(order);
            _dbContext.SaveChanges();

            TempData["success"] = "Đã xóa đơn hàng.";
            return RedirectToAction("Index");
        }
    }
}
