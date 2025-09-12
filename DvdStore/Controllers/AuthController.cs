using DvdStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class AuthController : Controller
    {
        private readonly DvdDbContext db;
        private readonly PasswordHasher<Users> PasswordHasher;
        public AuthController(DvdDbContext context)
        {
            db = context;
            PasswordHasher = new PasswordHasher<Users>();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(Users u)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.message = "Register error";
                return View();
            }

            if (db.tbl_Users.Any(x => x.Email == u.Email))
            {
                ViewBag.message = "Email already exist";
                return View();
            }

            u.Password = PasswordHasher.HashPassword(u, u.Password);
            db.tbl_Users.Add(u);
            db.SaveChanges();
            return RedirectToAction("Login");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var user = db.tbl_Users.FirstOrDefault(u => u.Email == Email);
            if (user != null &&
                PasswordHasher.VerifyHashedPassword(user, user.Password, Password) == PasswordVerificationResult.Success)
            {
                // Store session info
                HttpContext.Session.SetInt32("UserId", user.UserID);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role ?? "");

                // 🔑 Role-based redirect
                if (user.Role != null && user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Admin"); // go to admin dashboard
                }
                else
                {
                    return RedirectToAction("Index", "Home");  // normal user
                }
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
