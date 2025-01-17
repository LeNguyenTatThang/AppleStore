using Microsoft.AspNetCore.Mvc;
using AppleStore.Data;
using AppleStore.Models;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Product
                           .Include(p => p.Category) 
                           .ToList();
            return View(products);
        }

        public IActionResult Detail(int id)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            return View(product);  
        }

    }
}
