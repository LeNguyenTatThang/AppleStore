using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleStore.Models;
using System.Threading.Tasks;
using AppleStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Product/[action]/{id?}")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnviroment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            Console.WriteLine(role);

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            var products = await _context.Product.Include(p => p.Category).ToListAsync();
            return View(products);
           
        }


        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Category = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Category = new SelectList(_context.Category, "Id", "Name", product.CategoryId);

            

            if (product.Image != null)
            {
                string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "images");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                string imageName = Guid.NewGuid().ToString() + "_" + product.Image.FileName;
                string filePath = Path.Combine(uploadsDir, imageName);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(fs);
                }

                product.Img = imageName;
            }
            if (string.IsNullOrEmpty(product.Name) || string.IsNullOrEmpty(product.Information) || product.Price == 0 || product.CategoryId == 0 || product.Img == null || product.Image == null)
            {
                TempData["error"] = "Vui lòng nhập đầy đủ thông tin";
                return View(product);
            }
            _context.Add(product);
            await _context.SaveChangesAsync();
            TempData["success"] = "Thêm sản phẩm thành công";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Category = new SelectList(_context.Category, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            var existedProduct = await _context.Product.FindAsync(product.Id);

            if (existedProduct == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name", product.CategoryId);

                if (product.Image != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "images");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    string imageName = Guid.NewGuid().ToString() + "_" + product.Image.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.Image.CopyToAsync(fs);
                    }

                    existedProduct.Img = imageName;
                }

                existedProduct.Name = product.Name;
                existedProduct.Information = product.Information;
                existedProduct.Price = product.Price;
                existedProduct.CategoryId = product.CategoryId;
                _context.Update(existedProduct);
                await _context.SaveChangesAsync();

                TempData["success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");
            
            
        }

        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
