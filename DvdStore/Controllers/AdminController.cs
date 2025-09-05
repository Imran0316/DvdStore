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

       
    }
}
