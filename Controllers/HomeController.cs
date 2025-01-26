using System.Diagnostics;
using AppleStore.Data;
using AppleStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? categoryId)
        {
            var categories = _context.Category
                             .Where(c => c.Id != 21 && c.Id != 22 && c.Id != 23)
                             .ToList();
            var products = _context.Product.Include(p => p.Category).ToList();

            ViewBag.Categories = categories;
            ViewBag.Products = products;

            return View(products);
        }

        public IActionResult GetProductsByCategory(int categoryId)
        {
            var products = _context.Product
                                   .Where(p => p.CategoryId == categoryId)
                                   .Include(p => p.Category)
                                   .ToList();
            return View("Index", products);
        }

        public IActionResult Cart()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Search(string keyword)
        {
            var products = _context.Product
                .Include(p => p.Category) 
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                products = products.Where(p => p.Name.Contains(keyword) || p.Category.Name.Contains(keyword));
            }

            ViewBag.Keyword = keyword; 
            return View("Search", products.ToList());
        }

        public IActionResult Filter(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";

            var products = from p in _context.Product
                           select p;

            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    break;
            }

            return View(products.ToList());
        }
        public IActionResult GetProductsByCategory(int categoryId, int limit = 4)
        {
            var products = _context.Product
                .Where(p => p.CategoryId == categoryId)
                .Take(limit)
                .Include(p => p.Category)
                .ToList();

            return Json(products);
        }
    }
}
