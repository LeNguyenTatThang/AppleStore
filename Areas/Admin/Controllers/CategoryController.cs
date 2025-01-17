using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleStore.Models;
using System.Threading.Tasks;
using AppleStore.Data;
using Microsoft.AspNetCore.Authorization;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Category/[action]/{id?}")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách thể loại
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            Console.WriteLine(role);

            if (role == "Admin")
            {
                var categories = _context.Category.ToList();
                return View(categories);
            }
           return RedirectToAction("Login", "Account");
                
        }

        // Hiển thị form thêm thể loại mới
        public IActionResult Create()
        {
            return View();
        }

        // Thêm thể loại mới
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            
            _context.Add(category);
            await _context.SaveChangesAsync();
            if(category.Name == null)
            {
                TempData["error"] = "Vui lòng nhập đầy đủ thông tin";
                return View(category);
            }
            TempData["success"] = "Thêm danh mục thành công";
            return RedirectToAction(nameof(Index));
        }

        // Sửa thể loại
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Cập nhật thể loại
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Category.Any(e => e.Id == category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            if(category.Name == null)
            {
                TempData["error"] = "Vui lòng nhập đầy đủ thông tin";
                return View(category);
            }
            TempData["success"] = "Cập nhật danh mục thành công";
            return RedirectToAction("Index");
        }

        // Xóa thể loại
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            bool hasRelatedProducts = await _context.Product.AnyAsync(p => p.CategoryId == id);

            if (hasRelatedProducts)
            {
                TempData["error"] = "Không thể xóa danh mục vì còn sản phẩm liên quan.";
                return RedirectToAction(nameof(Index));
            }
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

            TempData["success"] = "Xóa danh mục thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
