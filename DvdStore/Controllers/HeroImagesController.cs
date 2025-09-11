using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class HeroImagesController : BaseAdminController
    {
        private readonly DvdDbContext _context;

        public HeroImagesController(DvdDbContext context)
        {
            _context = context;
        }

        // Helper method to check if user is authenticated and is admin
        private bool IsAdminUser()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            // For now, we'll assume any logged-in user can access admin features
            // You can add role checking later if needed
            return userId != null;
        }

        // GET: Admin/HeroImages
        public async Task<IActionResult> Index()
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(await _context.tbl_HeroImages.OrderBy(h => h.DisplayOrder).ToListAsync());
        }

        // GET: Admin/HeroImages/Create
        public IActionResult Create()
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HeroImage heroImage, IFormFile imageFile)
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Login", "Auth");
            }

            // REMOVE ImageUrl from ModelState validation since we set it programmatically
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                        var extension = Path.GetExtension(imageFile.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("imageFile", "Only image files are allowed (jpg, png, gif, bmp, webp)");
                            return View(heroImage);
                        }

                        // Validate file size (max 5MB)
                        if (imageFile.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("imageFile", "Image size must be less than 5MB");
                            return View(heroImage);
                        }

                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "hero");
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + extension;
                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        heroImage.ImageUrl = "/uploads/hero/" + fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("imageFile", "Please select an image file");
                        return View(heroImage);
                    }

                    _context.Add(heroImage);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating hero image: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while creating the hero image. Please try again.");
                }
            }

            return View(heroImage);
        }

        // GET: Admin/HeroImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id == null) return NotFound();

            var heroImage = await _context.tbl_HeroImages.FindAsync(id);
            if (heroImage == null) return NotFound();

            return View(heroImage);
        }

        // POST: Admin/HeroImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HeroImage heroImage, IFormFile? imageFile)
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id != heroImage.HeroImageID) return NotFound();

            // REMOVE ImageUrl from ModelState validation for edit too
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingImage = await _context.tbl_HeroImages.FindAsync(id);
                    if (existingImage == null) return NotFound();

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                        var extension = Path.GetExtension(imageFile.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("imageFile", "Only image files are allowed (jpg, png, gif, bmp, webp)");
                            return View(heroImage);
                        }

                        // Validate file size (max 5MB)
                        if (imageFile.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("imageFile", "Image size must be less than 5MB");
                            return View(heroImage);
                        }

                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "hero");
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + extension;
                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingImage.ImageUrl))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingImage.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        existingImage.ImageUrl = "/uploads/hero/" + fileName;
                    }

                    // Update other fields
                    existingImage.Title = heroImage.Title;
                    existingImage.Subtitle = heroImage.Subtitle;
                    existingImage.IsActive = heroImage.IsActive;
                    existingImage.DisplayOrder = heroImage.DisplayOrder;
                    existingImage.ButtonText = heroImage.ButtonText;
                    existingImage.ButtonLink = heroImage.ButtonLink;

                    _context.Update(existingImage);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HeroImageExists(heroImage.HeroImageID))
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
                    Console.WriteLine($"Error updating hero image: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while updating the hero image. Please try again.");
                }
            }

            return View(heroImage);
        }

        // POST: Admin/HeroImages/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Login", "Auth");
            }

            var heroImage = await _context.tbl_HeroImages.FindAsync(id);
            if (heroImage != null)
            {
                // Delete image file
                if (!string.IsNullOrEmpty(heroImage.ImageUrl))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", heroImage.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.tbl_HeroImages.Remove(heroImage);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool HeroImageExists(int id)
        {
            return _context.tbl_HeroImages.Any(e => e.HeroImageID == id);
        }
    }
}