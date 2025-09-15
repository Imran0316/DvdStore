using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DvdDbContext db;
        public CategoryController(DvdDbContext context)
        {
            db = context;
        }

        // ADMIN: Categories management (your existing code)
        public IActionResult Categories()
        {
            var categories = db.tbl_Category.ToList();
            return View(categories);
        }

        [HttpPost]
        public IActionResult Categories(Category ct)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Category.Add(ct);
                db.SaveChanges();
                return RedirectToAction("Categories");
            }

            // ✅ Sirf error message show karo
            var firstError = ModelState.Values
                .SelectMany(v => v.Errors)
                .FirstOrDefault();

            if (firstError != null)
            {
                ViewBag.Error = firstError.ErrorMessage;
            }

            var categories = db.tbl_Category.ToList();
            return View(categories);
        }
        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var category = db.tbl_Category.FirstOrDefault(c => c.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult EditCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                var category = db.tbl_Category.FirstOrDefault(c => c.CategoryID == model.CategoryID);
                if (category != null)
                {
                    category.CategoryName = model.CategoryName;
                    category.Description = model.Description;
                    db.SaveChanges();
                }
                return RedirectToAction("Categories");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            var category = db.tbl_Category.FirstOrDefault(c => c.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }

            db.tbl_Category.Remove(category);
            db.SaveChanges();

            return RedirectToAction("Categories");
        }

        // NEW: Public facing categories browsing
        public IActionResult Index()
        {
            var categories = db.tbl_Category.ToList();
            return View(categories);
        }

        // NEW: Products by category or type
        public IActionResult Products(int id = 0, string type = "")
        {
            var productsQuery = db.tbl_Products
                .Include(p => p.tbl_Albums)
                .ThenInclude(a => a.tbl_Category)
                .Include(p => p.tbl_Producers)
                .Where(p => p.IsActive);

            // Filter by category ID if provided
            if (id > 0)
            {
                productsQuery = productsQuery.Where(p => p.tbl_Albums.CategoryID == id);
            }

            // Filter by product type (Games, Movies, Music)
            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToLower())
                {
                    case "games":
                        productsQuery = productsQuery.Where(p => p.tbl_Albums.tbl_Category.CategoryName.Contains("Game") ||
                                                               p.tbl_Albums.tbl_Category.CategoryName.Contains("Gaming"));
                        break;
                    case "movies":
                        productsQuery = productsQuery.Where(p => p.tbl_Albums.tbl_Category.CategoryName.Contains("Movie") ||
                                                               p.tbl_Albums.tbl_Category.CategoryName.Contains("Film"));
                        break;
                    case "music":
                        productsQuery = productsQuery.Where(p => p.tbl_Albums.tbl_Category.CategoryName.Contains("Music") ||
                                                               p.tbl_Albums.tbl_Category.CategoryName.Contains("Album"));
                        break;
                }
            }

            var products = productsQuery.ToList();
            ViewBag.Categories = db.tbl_Category.ToList();
            ViewBag.SelectedCategory = id;
            ViewBag.SelectedType = type;

            return View(products);
        }

        // NEW: Browse by specific type (convenience methods)
        public IActionResult Games()
        {
            return RedirectToAction("Products", new { type = "games" });
        }

        public IActionResult Movies()
        {
            return RedirectToAction("Products", new { type = "movies" });
        }

        public IActionResult Music()
        {
            return RedirectToAction("Products", new { type = "music" });
        }
    }
}