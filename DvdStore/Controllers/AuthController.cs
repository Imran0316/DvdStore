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
        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var user = db.tbl_Users.FirstOrDefault(u => u.Email == Email);

            if (user != null)
            {
                // Use PasswordHasher to verify the password
                var result = PasswordHasher.VerifyHashedPassword(user, user.Password, Password);

                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetInt32("UserId", user.UserID);

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }
    }
}
