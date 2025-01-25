using Microsoft.AspNetCore.Mvc;
using AppleStore.Data;
using AppleStore.Models;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;  // Số lượng sản phẩm mỗi trang

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

        // Action phân trang
        public IActionResult Index(int page = 1, string keyword = "")
        {
            // Lấy tổng số sản phẩm
            var totalProducts = _context.Product
                .Where(p => string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword))
                .Count();

            // Tính toán số trang
            var totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);

            // Lấy dữ liệu sản phẩm cho trang hiện tại
            var products = _context.Product
                .Where(p => string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword))
                .Skip((page - 1) * PageSize)  // Bỏ qua số lượng sản phẩm của các trang trước
                .Take(PageSize)  // Lấy PageSize sản phẩm
                .Include(p => p.Category) // Lấy thông tin danh mục (nếu cần)
                .ToList();

            // Tạo ViewModel với sản phẩm và thông tin phân trang
            var model = new ProductViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                Keyword = keyword
            };

            return View(model);
        }
    }

}
