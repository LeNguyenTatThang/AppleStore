using AppleStore.Models.Vnpay;
using Microsoft.AspNetCore.Mvc;
using AppleStore.Services.Vnpay;
using AppleStore.Services.Momo;
using AppleStore.Models;
using AppleStore.Data;
using AppleStore.Extensions;

namespace AppleStore.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly ApplicationDbContext _dbContext;
        private IMomoService _momoService;

        public PaymentController(IMomoService momoService, IVnPayService vnPayService, ApplicationDbContext dbContext)
        {
            _momoService = momoService;
            _vnPayService = vnPayService;
            _dbContext = dbContext;
        }

        // Thêm từ khóa async để hỗ trợ các thao tác bất đồng bộ
        public async Task<IActionResult> Checkout(OrderInfo model, PaymentInformationModel model1, string customerName, string address, string phone, decimal Amount, string paymentMethod)
        {
            // Lấy giá trị paymentMethod từ form (được gửi khi submit form)
            if (string.IsNullOrEmpty(paymentMethod))
            {
                TempData["Error"] = "Vui lòng chọn phương thức thanh toán.";
                return RedirectToAction("Index", "Cart");
            }
            HttpContext.Session.SetString("CustomerName", customerName);
            HttpContext.Session.SetString("Address", address);
            HttpContext.Session.SetString("Phone", phone);
            HttpContext.Session.SetDecimal("TotalAmount", Amount);
            // Sử dụng switch-case để xử lý các phương thức thanh toán
            switch (paymentMethod)
            {
                case "Momo":
                    var response = await _momoService.CreatePaymentAsync(model);
                    return Redirect(response.PayUrl);

                case "Vnpay":
                    var url = _vnPayService.CreatePaymentUrl(model1, HttpContext);
                    return Redirect(url);

                case "COD":
                    var order = new Order
                    {
                        CustomerName = customerName,
                        Address = address,
                        Phone = phone,
                        TotalAmount = Amount,
                        PaymentMethod = "COD",
                        PaymentStatus = "Pending",
                        OrderStatus = "Processing",
                        OrderDate = DateTime.Now
                    };

                    _dbContext.Orders.Add(order);
                    await _dbContext.SaveChangesAsync();

                    TempData["PaymentSuccess"] = "Thanh toán COD thành công!";
                    return RedirectToAction("PaymentSuccess", new { orderId = order.Id });

                default:
                    // Nếu không phải là phương thức thanh toán hợp lệ, chuyển hướng về giỏ hàng
                    TempData["Error"] = "Phương thức thanh toán không hợp lệ.";
                    return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost("payment/momo")]
        public async Task<IActionResult> CreatePaymentMomo(OrderInfo model, string customerName, string address, string phone, decimal totalAmount)
        {
            HttpContext.Session.SetString("CustomerName", customerName);
            HttpContext.Session.SetString("Address", address);
            HttpContext.Session.SetString("Phone", phone);
            HttpContext.Session.SetDecimal("TotalAmount", totalAmount);

            // Gọi dịch vụ để tạo payment qua Momo
            var response = await _momoService.CreatePaymentAsync(model);

            TempData["PaymentSuccess"] = "Thanh toán qua Momo thành công!";
            return Redirect(response.PayUrl);
        }

        [HttpPost("payment/vnpay")]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model, string customerName, string address, string phone, decimal totalAmount)
        {
            HttpContext.Session.SetString("CustomerName", customerName);
            HttpContext.Session.SetString("Address", address);
            HttpContext.Session.SetString("Phone", phone);
            HttpContext.Session.SetDecimal("TotalAmount", totalAmount);

            // Tạo URL thanh toán VnPay
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            TempData["PaymentSuccess"] = "Thanh toán qua VnPay thành công!";
            return Redirect(url);
        }

        public IActionResult PaymentCOD(string customerName, string address, string phone, decimal totalAmount)
        {
            var order = new Order
            {
                CustomerName = customerName,
                Address = address,
                Phone = phone,
                TotalAmount = totalAmount,
                PaymentMethod = "COD",
                PaymentStatus = "Pending",
                OrderStatus = "Processing",
                OrderDate = DateTime.Now
            };

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            TempData["PaymentSuccess"] = "Thanh toán COD thành công!";
            return RedirectToAction("PaymentSuccess", new { orderId = order.Id });
        }
    }
}
