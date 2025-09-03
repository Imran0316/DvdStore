using DvdStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DvdStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly DvdDbContext db;
        public AdminController(DvdDbContext context)
        {
            db = context;
        }
        public IActionResult index()
        {
            return View();
        }
        public IActionResult Categories()
        {
            var categories = db.tbl_Category.ToList(); // sari categories DB se le aai
            return View(categories); // list ko view me bhej di
        }

        [HttpPost]
        public IActionResult Categories(Category ct)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Category.Add(ct);
                db.SaveChanges();
                return RedirectToAction("Categories"); // add ke baad wapas categories page
            }

            // agar validation fail ho to list ke sath hi wapas bhejo
            var categories = db.tbl_Category.ToList();
            return View(categories);
        }
        public IActionResult EditCategory() {
        
            return View();
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
                return NotFound(); // ya phir view me "Category not found." ka message
            }

            return View(category); // yahan single category bhejna hai, list nahi
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


    }
}
