using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO; // Add this for Path.Combine

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

            // DEBUG: Log model state
            Console.WriteLine("=== CREATE METHOD STARTED ===");
            Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");

            // FIX: REMOVE VALIDATION FOR THESE PROPERTIES
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Product");    // Add this line
            ModelState.Remove("ProductID");  // Add this line

            // Log all errors
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("Model is valid, processing...");

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

                // FIX: Ensure ProductID is properly handled
                if (item.ProductID == 0) // If 0 is passed (default), set to null
                {
                    item.ProductID = null;
                }

                _context.tbl_NewsPromotions.Add(item);
                await _context.SaveChangesAsync();

                Console.WriteLine("Item saved successfully!");
                await SendNewsNotification(item);
                return RedirectToAction("Index");
            }

            // If we get here, something went wrong - redisplay form
            Console.WriteLine("Model validation failed - redisplaying form");
            ViewBag.Products = _context.tbl_Products.Where(p => p.IsActive).ToList();
            return View(item);
        }





        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(NewsPromotion item, IFormFile imageFile)
        //{
        //    // TEMPORARY: Skip admin check and image upload for testing
        //    Console.WriteLine("CREATE called with: " + item.Title);

        //    // Manually validate required fields
        //    if (string.IsNullOrEmpty(item.Title) || string.IsNullOrEmpty(item.Content))
        //    {
        //        ModelState.AddModelError("", "Title and Content are required");
        //    }
        //    else
        //    {
        //        // Simple save without image
        //        item.ImageUrl = "/images/placeholder.jpg"; // Temporary placeholder
        //        item.PublishDate = DateTime.Now;

        //        _context.tbl_NewsPromotions.Add(item);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Products = _context.tbl_Products.Where(p => p.IsActive).ToList();
        //    return View(item);
        //}






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

            Console.WriteLine("=== EDIT METHOD STARTED ===");
            Console.WriteLine($"Editing item ID: {id}, ModelState IsValid: {ModelState.IsValid}");

            // FIX: REMOVE VALIDATION FOR THESE PROPERTIES
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Product");    // Add this line
            ModelState.Remove("ProductID");  // Add this line

            // Log all errors
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingItem = await _context.tbl_NewsPromotions.FindAsync(id);
                    if (existingItem == null) return NotFound();

                    Console.WriteLine($"Found existing item: {existingItem.Title}");

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

                    // FIX: Ensure ProductID is properly handled
                    if (item.ProductID == 0) // If 0 is passed (default), set to null
                    {
                        item.ProductID = null;
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

                    Console.WriteLine("Item updated successfully!");
                    await SendNewsNotification(existingItem);

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"Concurrency error: {ex.Message}");
                    if (!NewsPromotionExists(item.NewsID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General error: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while updating: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Model validation failed");
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


        // GET: Public News Page
        public async Task<IActionResult> News(string type = "")
        {
            Console.WriteLine("=== NEWS METHOD CALLED ===");

            var query = _context.tbl_NewsPromotions
                .Include(n => n.Product)
                .ThenInclude(p => p.tbl_Albums)
                .Where(n => n.IsActive && n.PublishDate <= DateTime.Now &&
                           (n.ExpiryDate == null || n.ExpiryDate >= DateTime.Now));

            // Filter by type if specified
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(n => n.Type == type);
            }

            var news = await query.OrderByDescending(n => n.PublishDate).ToListAsync();

            // DEBUG: Log what's being returned
            Console.WriteLine($"Found {news.Count} news items");
            foreach (var item in news)
            {
                Console.WriteLine($"- {item.Title} (Active: {item.IsActive}, Publish: {item.PublishDate}, Expiry: {item.ExpiryDate})");
            }

            ViewBag.SelectedType = type;
            return View(news);
        }


        private async Task SendNewsNotification(NewsPromotion newsItem)
        {
            try
            {
                // Get all users who opted in for notifications
                var subscribers = await _context.tbl_Users
                    .Where(u => u.Role == "Customer") // Only customers
                    .ToListAsync();

                foreach (var user in subscribers)
                {
                    // Here you would integrate with your email service
                    Console.WriteLine($"Would send email to {user.Email} about: {newsItem.Title}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email notification error: {ex.Message}");
            }
        }
    }  
}     