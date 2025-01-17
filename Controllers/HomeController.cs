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
            var categories = _context.Category.ToList();

            var products = categoryId.HasValue
                           ? _context.Product
                               .Where(p => p.CategoryId == categoryId.Value)
                               .Include(p => p.Category)
                               .ToList() 
                           : _context.Product
                               .Include(p => p.Category)
                               .Take(6) 
                               .ToList();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId; 

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
    }
}
