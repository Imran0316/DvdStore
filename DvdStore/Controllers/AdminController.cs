using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Auth");

            // Dashboard counts
            ViewBag.TotalUsers = await db.tbl_Users.CountAsync();
            ViewBag.TotalOrders = await db.tbl_Orders.CountAsync();
            ViewBag.TotalProducts = await db.tbl_Products.CountAsync();
            ViewBag.TotalFeedbacks = await db.tbl_Feedbacks.CountAsync();
            // Recent Orders with INCLUDES
            var recentOrders = await db.tbl_Orders
                .Include(o => o.tbl_Users) // User info
                .Include(o => o.tbl_OrderDetails) // OrderDetails
                    .ThenInclude(od => od.tbl_Products) // Product info
                        .ThenInclude(p => p.tbl_Albums) // ✅ YE LINE ADD KARO - Album info
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentOrders = recentOrders;
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
        // Make User Admin
        public IActionResult MakeAdmin(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = db.tbl_Users.FirstOrDefault(u => u.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            // Role update karo
            user.Role = "Admin";
            db.SaveChanges();

            TempData["Success"] = "User promoted to Admin successfully!";
            return RedirectToAction("Users");
        }

    }

}
