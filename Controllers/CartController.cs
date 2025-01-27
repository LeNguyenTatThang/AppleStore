using AppleStore.Models;
using Microsoft.AspNetCore.Mvc;
using AppleStore.Extensions;
using System.Text;
using AppleStore.Services.Vnpay;
using AppleStore.Libraries;
using AppleStore.Services.Momo;
using Azure;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppleStore.Data;
using AppleStore.Models.Momo;
using AppleStore.Models.Vnpay;
using System.Net;

namespace AppleStore.Controllers
{
    public class CartController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private const string CartSessionKey = "Cart";
        private readonly VnPayLibrary _vnPayLibrary;
        private readonly IMomoService _momoService;
        private readonly ApplicationDbContext _dbContext;

        public CartController(IMomoService momoService, VnPayLibrary vnPayLibrary, IVnPayService vnPayService, ApplicationDbContext context)
        {
            _momoService = momoService;
            _vnPayLibrary = vnPayLibrary;
            _vnPayService = vnPayService;
            _dbContext = context; 
        }
        public IActionResult ResultCallbackVnpay()
        {
            return View();
        }
        public IActionResult Index()

        {

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");
            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            return View(cart);
        }

        public IActionResult AddToCart(int productId, string productName, decimal price, string imageUrl)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");

            if (cart == null)
            {
                cart = new List<CartItem>();
            }
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    Price = price,
                    ImageUrl = imageUrl,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }

            HttpContext.Session.SetObjectAsJson("cart", cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, string action)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");

            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                if (action == "increase")
                {
                    item.Quantity++;
                }
                else if (action == "decrease" && item.Quantity > 1)
                {
                    item.Quantity--;
                }
            }

            HttpContext.Session.SetObjectAsJson("cart", cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");

            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
            }

            HttpContext.Session.SetObjectAsJson("cart", cart);

            return RedirectToAction("Index");
        }
        private List<OrderDetail> SaveOrderDetails(List<CartItem> cart, string orderId)
        {
            if (cart == null || !cart.Any())
            {
                throw new ArgumentException("Cart không hợp lệ!");
            }

            var orderDetails = cart.Select(item => new OrderDetail
            {
                OrderId = orderId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price,
                Total = item.Quantity * item.Price
            }).ToList();

            
            _dbContext.OrderDetails.AddRange(orderDetails);
            _dbContext.SaveChanges();

            cart.Clear();
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return orderDetails;
        }


        [Route("Cart/PaymentCallbackVnpay")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            var CustomerName = HttpContext.Session.GetString("CustomerName");
                var Address = HttpContext.Session.GetString("Address");
                var Phone = HttpContext.Session.GetString("Phone");
                var TotalAmount = HttpContext.Session.GetDecimal("TotalAmount");
            if (response.VnPayResponseCode == "00")
            {
                
                var checkOrder = new Order
                {
                    CustomerName = CustomerName,
                    Address = Address,
                    Phone = Phone,
                    TotalAmount = TotalAmount,
                    PaymentMethod = response.PaymentMethod,
                    OrderId = response.OrderId,
                    PaymentStatus = "Success",
                    OrderStatus = "Processing",
                    OrderDate = DateTime.Now
                };

                _dbContext.Orders.Add(checkOrder);
                await _dbContext.SaveChangesAsync();
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");

                if (cart == null || !cart.Any())
                {
                    ViewBag.Message = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Index", "Home");
                }

                SaveOrderDetails(cart, checkOrder.OrderId);
                ViewBag.Message = "Thanh toán thành công. Cảm ơn bạn đã mua hàng tại Apple Store.";
            }
            else
            {
                var checkOrder = new Order
                {
                    CustomerName = CustomerName,
                    Address = Address,
                    Phone = Phone,
                    TotalAmount = TotalAmount,
                    PaymentMethod = response.PaymentMethod,
                    OrderId = response.OrderId,
                    PaymentStatus = "Fail",
                    OrderStatus = "Processing",
                    OrderDate = DateTime.Now
                };

                _dbContext.Orders.Add(checkOrder);
                await _dbContext.SaveChangesAsync();
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");

                if (cart == null || !cart.Any())
                {
                    ViewBag.Message = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Index", "Home");
                }

                SaveOrderDetails(cart, checkOrder.OrderId);
                ViewBag.Message = "Thanh toán thất bại. Vui lòng thử lại hoặc liên hệ hỗ trợ.";
            }

            return View("ResultCallbackVnpay", response);

        }


        [HttpGet]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = await _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;

            var CustomerName = HttpContext.Session.GetString("CustomerName");
            var Address = HttpContext.Session.GetString("Address");
            var Phone = HttpContext.Session.GetString("Phone");
            var TotalAmount = HttpContext.Session.GetDecimal("TotalAmount");

            if (response.IsSuccess)
            {
                var checkOrder = new Order
                {
                    CustomerName = CustomerName,
                    Address = Address,
                    Phone = Phone,
                    TotalAmount = TotalAmount,
                    PaymentMethod = "Momo",
                    OrderId = requestQuery["orderId"],
                    PaymentStatus = "Success",
                    OrderStatus = "Procesing",
                    OrderDate = DateTime.Now
                };

                _dbContext.Orders.Add(checkOrder);
                await _dbContext.SaveChangesAsync();
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");

                if (cart == null || !cart.Any())
                {
                    ViewBag.Message = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Index", "Home");
                }

                SaveOrderDetails(cart, checkOrder.OrderId);
                ViewBag.Message = "Thanh toán thành công. Cảm ơn bạn đã mua hàng tại Apple Store.";
            }
            else
            {
                var checkOrder = new Order
                {
                    CustomerName = CustomerName,
                    Address = Address,
                    Phone = Phone,
                    TotalAmount = TotalAmount,
                    PaymentMethod = "Momo",
                    OrderId = requestQuery["orderId"],
                    PaymentStatus = "Fail",
                    OrderStatus = "Procesing",
                    OrderDate = DateTime.Now
                };
                _dbContext.Orders.Add(checkOrder);
                await _dbContext.SaveChangesAsync();
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");

                if (cart == null || !cart.Any())
                {
                    ViewBag.Message = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Index", "Home");
                }

                SaveOrderDetails(cart, checkOrder.OrderId);
                ViewBag.Message = "Thanh toán thất bại. Vui lòng thử lại hoặc liên hệ hỗ trợ.";
            }

            return View("PaymentCallback", response);
        }

    }
}
