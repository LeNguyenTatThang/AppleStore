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

        // Hiển thị danh sách đơn hàng
        [HttpGet]
        [Route("Admin/Order/Detail")]
        public IActionResult Index()
        {
            var orders = _dbContext.Orders.ToList();
            return View(orders);
        }

        [HttpGet]
        [Route("Admin/Order/Detail/{orderId?}")]
        public async Task<IActionResult> Detail(string orderId)
        {
            // Kiểm tra xem orderId có được truyền vào không
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("Mã đơn hàng không hợp lệ.");
            }

            // Debug: In orderId ra log
            Console.WriteLine($"orderId received: {orderId}");

            // Tìm đơn hàng và chi tiết đơn hàng
            var order = await _dbContext.Orders
                .Include(o => o.OrderDetails) // Bao gồm thông tin OrderDetails
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound(new { message = $"Không tìm thấy đơn hàng với mã {orderId}." });
            }

            // Trả về View với model là đơn hàng
            return View(order);
        }


        // Cập nhật trạng thái đơn hàng
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var order = _dbContext.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = status;
            _dbContext.SaveChanges();

            TempData["Message"] = "Trạng thái đơn hàng đã được cập nhật.";
            return RedirectToAction("Index");
        }
    }
}
