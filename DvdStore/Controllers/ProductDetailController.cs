using DvdStore.Migrations;
using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class ProductDetailController : Controller
    {
        private readonly DvdDbContext _context;

        public ProductDetailController(DvdDbContext context)
        {
            _context = context;
        }

        // In ProductDetailController.cs - Temporary fix
        public async Task<IActionResult> Index(int id)
        {
            var product = await _context.tbl_Products
     .Include(p => p.tbl_Albums!.tbl_Artists)
     .Include(p => p.tbl_Albums!.tbl_Category)
     .Include(p => p.tbl_Albums!.tbl_Songs)  // 🔑 Direct include of songs
     .Include(p => p.tbl_Producers)
     .Include(p => p.tbl_Suppliers)
     .FirstOrDefaultAsync(p => p.ProductID == id && p.IsActive);


            if (product == null)
            {
                return NotFound();
            }

            // Load reviews separately (this will work even if table doesn't exist yet)
            try
            {
                var reviews = await _context.tbl_ProductReviews
                    .Include(r => r.User)
                    .Where(r => r.ProductID == id && r.IsApproved)
                    .ToListAsync();

                ViewBag.Reviews = reviews;

                // Calculate average rating
                if (reviews.Any())
                {
                    product.AverageRating = reviews.Average(r => r.Rating);
                    product.ReviewCount = reviews.Count;
                }
                else
                {
                    product.AverageRating = 0;
                    product.ReviewCount = 0;
                }
            }
            catch (Exception ex)
            {
                // If table doesn't exist, set default values
                Console.WriteLine($"Reviews table not available: {ex.Message}");
                product.AverageRating = 0;
                product.ReviewCount = 0;
                ViewBag.Reviews = new List<Models.ProductReviews>();
            }

            // Get related products
            var relatedProducts = await _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .Where(p => p.IsActive &&
                           p.tbl_Albums.CategoryID == product.tbl_Albums.CategoryID &&
                           p.ProductID != id)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }

        // POST: Add Review
        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var review = new Models.ProductReviews
            {
                ProductID = productId,
                UserID = userId.Value,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            _context.tbl_ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = productId });
        }

        // Helper method to check if user is admin (if needed)
        private bool IsAdminUser()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        // In ProductDetailController.cs - Add this method
        [HttpPost]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            if (!AuthorizationHelper.IsAdmin(HttpContext))
            {
                return RedirectToAction("Login", "Auth");
            }

            var review = await _context.tbl_ProductReviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.tbl_ProductReviews.Remove(review);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Review deleted successfully";
            }

            return RedirectToAction("Index", new { id = review.ProductID });
        }
    }
}