using Microsoft.AspNetCore.Mvc;
using AppleStore.Data;
using AppleStore.Models;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;  

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Detail(int id)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == id); 
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Index(int CategoryId, int page = 1, string keyword = "")
        {
            const int PageSize = 16;
            var totalProducts = _context.Product
            .Where(p => p.CategoryId == CategoryId && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
            .Count();

            var totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            var products = _context.Product
            .Where(p => p.CategoryId == CategoryId && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
            .OrderBy(p => p.Id)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Include(p => p.Category)
                .ToList();

            var model = new ProductViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                Keyword = keyword
            };

            return View( model);
        }
    }

}
