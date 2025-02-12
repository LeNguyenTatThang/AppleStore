using AppleStore.Models.Vnpay;
using Microsoft.AspNetCore.Mvc;
using AppleStore.Services.Vnpay;
using AppleStore.Services.Momo;
using AppleStore.Models;
using AppleStore.Data;
using AppleStore.Extensions;
using Azure;
using System.Net;
using System.Numerics;

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
        public async Task<IActionResult> Checkout(OrderInfo model, PaymentInformationModel model1, string customerName, string address, string phone, decimal Amount, string paymentMethod)
        {
            if (string.IsNullOrEmpty(paymentMethod))
            {
                TempData["Error"] = "Vui lòng chọn phương thức thanh toán.";
                return RedirectToAction("Index", "Cart");
            }
            HttpContext.Session.SetString("CustomerName", customerName);
            HttpContext.Session.SetString("Address", address);
            HttpContext.Session.SetString("Phone", phone);
            HttpContext.Session.SetDecimal("TotalAmount", Amount);
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
                        OrderId = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        CustomerName = customerName,
                        Address = address,
                        Phone = phone,
                        TotalAmount = Amount,
                        PaymentMethod = "COD",
                        PaymentStatus = "Success",
                        OrderStatus = "Processing",
                        OrderDate = DateTime.Now
                    };

                    _dbContext.Orders.Add(order);
                    await _dbContext.SaveChangesAsync();

                    var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");


                    if (cart == null || !cart.Any())
                    {
                        throw new ArgumentException("Cart không hợp lệ!");
                    }

                    var orderDetails = cart.Select(item => new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Total = item.Quantity * item.Price
                    }).ToList();


                    _dbContext.OrderDetails.AddRange(orderDetails);
                    _dbContext.SaveChanges();

                    HttpContext.Session.Remove("cart");

                    ViewBag.Message = "Thanh toán thành công. Cảm ơn bạn đã mua hàng tại Apple Store.";
                    TempData["PaymentSuccess"] = "Thanh toán thành công!";
                    return RedirectToAction("Index", "Home");

                default:
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

            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            TempData["PaymentSuccess"] = "Thanh toán qua VnPay thành công!";
            return Redirect(url);
        }

    }
}
