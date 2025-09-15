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



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM model) 
        {

            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            // Database se user check karein
            var user = db.tbl_Users.FirstOrDefault(u => u.Email == model.Email);

            // Password verify karein
            if (user != null &&
                PasswordHasher.VerifyHashedPassword(user, user.Password, model.Password) == PasswordVerificationResult.Success)
            {
                // Session set karein
                HttpContext.Session.SetInt32("UserId", user.UserID);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role ?? "");

                // Role-based redirect
                if (user.Role != null && user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // ✅ Properly error add karein
            ModelState.AddModelError(string.Empty, "Invalid Email or Password!");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
