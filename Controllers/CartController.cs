using AppleStore.Models;
using Microsoft.AspNetCore.Mvc;
using AppleStore.Extensions;
using System.Text;
using AppleStore.Services.Vnpay;
using AppleStore.Libraries;
using AppleStore.Services.Momo;
using Azure;
using Microsoft.AspNetCore.Http;
namespace AppleStore.Controllers
{
    public class CartController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private const string CartSessionKey = "Cart";
        private readonly VnPayLibrary _vnPayLibrary;
        private readonly IMomoService _momoService;

        public CartController(IMomoService momoService, VnPayLibrary vnPayLibrary)
        {
            _momoService = momoService;
            _vnPayLibrary = vnPayLibrary;
        }
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");

            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
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

        //public IActionResult PaymentCallbackVnpay()
        //{
        //    return View("PaymentCallbackVnpay");
        //}

        [Route("Cart/PaymentCallbackVnpay")]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if(response.Success == true)
            TempData["Success"] = "Thanh toán thành công!";
            return View("PaymentSuccess", response);
        }
        public IActionResult PaymentCallBack()
        {
            // Lấy thông tin giao dịch từ Momo
            var response =  _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            Console.WriteLine("check res :", response);
            // Kiểm tra nếu không có mã đơn hàng
            if (Response.StatusCode == 0)
            {
                
                TempData["Success"] = "Thanh toán thành công!";
                return View("PaymentSuccess", response);
             }
            else 
            {
                TempData["Error"] = "Giao dịch đã bị hủy.";
                return View("PaymentSuccess");
            }
        }


    }

}
