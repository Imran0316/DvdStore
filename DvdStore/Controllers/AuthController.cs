using DvdStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class AuthController : Controller
    {
        private readonly DvdDbContext db;
        private PasswordHasher<Users> PasswordHasher;
        public AuthController(DvdDbContext context)
        {
            db = context;
            PasswordHasher = new PasswordHasher<Users>();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Users u)
        {
            if (ModelState.IsValid)
            {
                var existingEmail = db.tbl_Users.FirstOrDefault(x => x.Email == u.Email);
                if (existingEmail != null)
                {
                    ViewBag.message = "Email already exist";
                    return View();
                }
                else
                {
                    u.Password = PasswordHasher.HashPassword(u, u.Password);

                    db.tbl_Users.Add(u);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }

            }
            else
            {
                ViewBag.message = "Register error";
                return View();
            }
              
        }

        public IActionResult Login()
        {
            return View();
        }
        // In AuthController.cs - Update the Login method
        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var user = db.tbl_Users.FirstOrDefault(u => u.Email == Email);

            if (user != null)
            {
                var result = PasswordHasher.VerifyHashedPassword(user, user.Password, Password);

                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("UserName", user.Name);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetInt32("UserId", user.UserID);
                    HttpContext.Session.SetString("UserRole", user.Role); // Store role in session

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        //// Add this temporary method to AuthController
        //public IActionResult CreateFirstAdmin()
        //{
        //    var passwordHasher = new PasswordHasher<Users>();
        //    var hashedPassword = passwordHasher.HashPassword(null, "admin123");

        //    var admin = new Users
        //    {
        //        Name = "System Admin",
        //        Email = "admin@example.com",
        //        Phone = "12345678901",
        //        Password = hashedPassword,
        //        Role = "Admin",
        //        Created_At = DateTime.Now
        //    };

        //    db.tbl_Users.Add(admin);
        //    db.SaveChanges();

        //    return Content("First admin created! Email: admin@example.com, Password: admin123");
        //}


        public IActionResult Logout()
        {
            // Clear all session variables
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserRole");
            HttpContext.Session.Remove("CartCount");

            // Optional: Clear the entire session
            HttpContext.Session.Clear();

            // Optional: Regenerate session ID for security
            // HttpContext.Session.RegenerateSessionId();

            return RedirectToAction("Index", "Home");
        }
    }
}
