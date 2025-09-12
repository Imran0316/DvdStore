using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DvdStore.Controllers
{
    public class AdminController : BaseAdminController
    {
        private readonly DvdDbContext db;
        public AdminController(DvdDbContext context)
        {
            db = context;
        }

        // Dashboard
        public IActionResult Index()
        {
            // ✅ Ensure only admins can access
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        // Fetch all Users
        public IActionResult Users()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var users = db.tbl_Users.ToList();
            return View(users);
        }

        // Delete Users
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = db.tbl_Users.FirstOrDefault(u => u.UserID == id);
            if (user == null) return NotFound();

            db.tbl_Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Users");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
        // GET: /Admin/Profile
        public IActionResult Profile()
        {
            // Session se current AdminID le lo
            var adminId = HttpContext.Session.GetInt32("UserId");
            if (adminId == null) return RedirectToAction( "Login", "Auth");

            var admin = db.tbl_Users.FirstOrDefault(u => u.UserID == adminId);
            if (admin == null) return NotFound();

            return View(admin);
        }

        // POST: /Admin/Profile (for updating)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(Users model)
        {
            var adminId = HttpContext.Session.GetInt32("UserId");
            if (adminId == null) return RedirectToAction("Login");

            var admin = db.tbl_Users.FirstOrDefault(u => u.UserID == adminId);
            if (admin == null) return NotFound();

            // Update only allowed fields
            admin.Name = model.Name;
            admin.Phone = model.Phone;

            db.SaveChanges();
            ViewBag.Success = "Profile updated!";
            return View(admin);
        }

    }
}
