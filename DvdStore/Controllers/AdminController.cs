using DvdStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    }
}
