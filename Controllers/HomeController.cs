using System.Diagnostics;
using AppleStore.Data;
using AppleStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
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
        public IActionResult Iphone(int page = 1, string keyword = "")
        {
            const int PageSize = 16;
            var totalProducts = _context.Product
                .Where(p => p.CategoryId == 2 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
                .Count();

            var totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);

            var products = _context.Product
                .Where(p => p.CategoryId == 2 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
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

            return View(model);
        }

        public IActionResult ipad(int page = 1, string keyword = "")
        {
            const int PageSize = 16;
            var totalProducts = _context.Product
            .Where(p => p.CategoryId == 2 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
            .Count();

            var totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            var products = _context.Product
            .Where(p => p.CategoryId == 2 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
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

            return View(model);
        }
        public IActionResult mac(int page = 1, string keyword = "")
        {
            const int PageSize = 16;
            var totalProducts = _context.Product
            .Where(p => p.CategoryId == 3 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
            .Count();

            var totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            var products = _context.Product
            .Where(p => p.CategoryId == 3 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
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

            return View(model);
        }
        public IActionResult watch(int page = 1, string keyword = "")
        {
            const int PageSize = 16;
            var totalProducts = _context.Product
            .Where(p => p.CategoryId == 4 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
            .Count();

            var totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            var products = _context.Product
            .Where(p => p.CategoryId == 4 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
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

            return View(model);
        }
        public IActionResult am_thanh(int page = 1, string keyword = "")
        {
            const int PageSize = 16;
            var totalProducts = _context.Product
            .Where(p => p.CategoryId == 19 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
            .Count();

            var totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            var products = _context.Product
            .Where(p => p.CategoryId == 19 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
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

            return View("am-thanh", model);
        }
        public IActionResult phu_kien(int page = 1, string keyword = "")
        {
            const int PageSize = 16;
            var totalProducts = _context.Product
            .Where(p => p.CategoryId == 20 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
            .Count();

            var totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            var products = _context.Product
            .Where(p => p.CategoryId == 20 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
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

            return View("phu-kien", model);
        }
        public IActionResult may_cu(int page = 1, string keyword = "")
        {
            const int PageSize = 16;
            var totalProducts = _context.Product
            .Where(p => p.CategoryId == 21 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
            .Count();

            var totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            var products = _context.Product
            .Where(p => p.CategoryId == 21 && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword)))
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

            return View("may-cu", model);
        }
        public IActionResult news()
        {
            return View();
        }
        public IActionResult bao_hanh()
        {
            return View("bao-hanh");
        }
        public IActionResult sua_chua()
        {
            return View("sua-chua");
        }
      
    }
}
