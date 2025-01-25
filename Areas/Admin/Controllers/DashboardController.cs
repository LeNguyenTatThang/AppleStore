using AppleStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Dashboard")]
    public class DashboardController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            Console.WriteLine(role); 

            if (role == "Admin")
            {
                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet("GetChartData")]
        public JsonResult GetChartData()
        {
            var data = new
            {
                labels = new string[] { "January", "February", "March", "April", "May", "June" },
                datasets = new[]
                {
            new
            {
                label = "Sales Data",
                data = new int[] { 65, 59, 80, 81, 56, 55 },
                backgroundColor = "rgba(255, 99, 132, 0.2)",
                borderColor = "rgba(255, 99, 132, 1)",
                borderWidth = 1
            }
        }
            };

            return Json(data);
        }

        [HttpGet("GetAnotherChartData")]
        public JsonResult GetAnotherChartData()
        {
            var data = new
            {
                labels = new string[] { "Q1", "Q2", "Q3", "Q4" },
                datasets = new[]
                {
            new
            {
                label = "Revenue Data",
                data = new int[] { 150, 200, 170, 220 },
                backgroundColor = "rgba(54, 162, 235, 0.2)",
                borderColor = "rgba(54, 162, 235, 1)",
                borderWidth = 1
            }
        }
            };

            return Json(data);
        }

        public IActionResult Product()
        {
            return View();
        }

        public IActionResult Category()
        {
            return View();
        }

        public IActionResult Account()
        {
            return View();
        }

    }
}
