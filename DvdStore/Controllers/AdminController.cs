using DvdStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DvdStore.Controllers
{
    public class AdminController : BaseAdminController
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


        // fetch all Users
        public IActionResult Users()
        {
            var users = db.tbl_Users.ToList();
            return View(users);
        }
        //Delete Users
        public IActionResult Delete(int id)
        {
            var user = db.tbl_Users.FirstOrDefault(u => u.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            db.tbl_Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Users");
        }


        [HttpPost]
        public IActionResult CreateAdmin(string Name, string Email, string Phone, string Password)
        {
            if (!AuthorizationHelper.IsAdmin(HttpContext))
                return RedirectToAction("Login", "Auth");

            // Check if email exists
            var existingUser = db.tbl_Users.FirstOrDefault(u => u.Email == Email);
            if (existingUser != null)
            {
                TempData["Error"] = "Email already exists";
                return RedirectToAction("Users");
            }

            var passwordHasher = new PasswordHasher<Users>();
            var hashedPassword = passwordHasher.HashPassword(null, Password);

            var newAdmin = new Users
            {
                Name = Name,
                Email = Email,
                Phone = Phone,
                Password = hashedPassword,
                Role = "Admin",
                Created_At = DateTime.Now
            };

            db.tbl_Users.Add(newAdmin);
            db.SaveChanges();

            TempData["Success"] = "Admin created successfully";
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult MakeAdmin(int id)
        {
            if (!AuthorizationHelper.IsAdmin(HttpContext))
                return RedirectToAction("Login", "Auth");

            var user = db.tbl_Users.FirstOrDefault(u => u.UserID == id);
            if (user != null)
            {
                user.Role = "Admin";
                db.SaveChanges();
                TempData["Success"] = "User promoted to admin";
            }

            return RedirectToAction("Users");
        }

    }
}
