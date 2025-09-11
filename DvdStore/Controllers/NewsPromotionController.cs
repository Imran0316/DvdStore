using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class NewsPromotionController : Controller
    {
        private readonly DvdDbContext _context;

        public NewsPromotionController(DvdDbContext context)
        {
            _context = context;
        }

        // GET: Admin - List all news/promotions
        public async Task<IActionResult> Index()
        {
            if (!IsAdminUser()) return RedirectToAction("Login", "Auth");

            var items = await _context.tbl_NewsPromotions
                .Include(n => n.Product)
                .ThenInclude(p => p.tbl_Albums)
                .OrderByDescending(n => n.PublishDate)
                .ToListAsync();

            return View(items);
        }

        // GET: Admin - Create form
        public IActionResult Create()
        {
            if (!IsAdminUser()) return RedirectToAction("Login", "Auth");

            ViewBag.Products = _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .Where(p => p.IsActive)
                .ToList();
            return View();
        }

        // POST: Admin - Create news/promotion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsPromotion item, IFormFile imageFile)
        {
            if (!IsAdminUser()) return RedirectToAction("Login", "Auth");

            // Remove validation for ImageUrl since we set it programmatically
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "news");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var extension = Path.GetExtension(imageFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("imageFile", "Only image files are allowed");
                        ViewBag.Products = _context.tbl_Products.Where(p => p.IsActive).ToList();
                        return View(item);
                    }

                    var fileName = Guid.NewGuid().ToString() + extension;
                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    item.ImageUrl = "/uploads/news/" + fileName;
                }

                _context.tbl_NewsPromotions.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Products = _context.tbl_Products.Where(p => p.IsActive).ToList();
            return View(item);
        }

        // GET: Admin - Edit form
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdminUser()) return RedirectToAction("Login", "Auth");

            if (id == null) return NotFound();

            var item = await _context.tbl_NewsPromotions.FindAsync(id);
            if (item == null) return NotFound();

            ViewBag.Products = _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .Where(p => p.IsActive)
                .ToList();
            return View(item);
        }

        // POST: Admin - Edit news/promotion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewsPromotion item, IFormFile imageFile)
        {
            if (!IsAdminUser()) return RedirectToAction("Login", "Auth");

            if (id != item.NewsID) return NotFound();

            // Remove validation for ImageUrl
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingItem = await _context.tbl_NewsPromotions.FindAsync(id);
                    if (existingItem == null) return NotFound();

                    // Handle image upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "news");
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                        var extension = Path.GetExtension(imageFile.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("imageFile", "Only image files are allowed");
                            ViewBag.Products = _context.tbl_Products.Where(p => p.IsActive).ToList();
                            return View(item);
                        }

                        var fileName = Guid.NewGuid().ToString() + extension;
                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingItem.ImageUrl))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingItem.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        existingItem.ImageUrl = "/uploads/news/" + fileName;
                    }

                    // Update other fields
                    existingItem.Title = item.Title;
                    existingItem.Content = item.Content;
                    existingItem.Type = item.Type;
                    existingItem.IsActive = item.IsActive;
                    existingItem.PublishDate = item.PublishDate;
                    existingItem.ExpiryDate = item.ExpiryDate;
                    existingItem.ProductID = item.ProductID;

                    _context.Update(existingItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsPromotionExists(item.NewsID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            ViewBag.Products = _context.tbl_Products.Where(p => p.IsActive).ToList();
            return View(item);
        }

        // POST: Admin - Delete news/promotion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdminUser()) return RedirectToAction("Login", "Auth");

            var item = await _context.tbl_NewsPromotions.FindAsync(id);
            if (item != null)
            {
                // Delete image file if exists
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.tbl_NewsPromotions.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private bool NewsPromotionExists(int id)
        {
            return _context.tbl_NewsPromotions.Any(e => e.NewsID == id);
        }

        private bool IsAdminUser()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }
    }
}