using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DvdStore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DvdStore.Controllers
{
    public class ProfileController : Controller
    {
        private readonly DvdDbContext _context;

        public ProfileController(DvdDbContext context)
        {
            _context = context;
        }

        // GET: Profile/Index
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.tbl_Users
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get user's orders with details
            var orders = await _context.tbl_Orders
                .Include(o => o.tbl_OrderDetails)
                .ThenInclude(od => od.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.Orders = orders;
            return View(user);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.tbl_Users
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Users userModel)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Remove password from validation since we're not changing it here
            ModelState.Remove("Password");
            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                var existingUser = await _context.tbl_Users.FindAsync(userId);
                if (existingUser != null)
                {
                    existingUser.Name = userModel.Name;
                    existingUser.Email = userModel.Email;
                    existingUser.Phone = userModel.Phone;

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();

                    // Update session name if changed
                    HttpContext.Session.SetString("UserName", existingUser.Name);

                    TempData["Success"] = "Profile updated successfully!";
                    return RedirectToAction("Index");
                }
            }

            return View(userModel);
        }

        // GET: Profile/OrderDetails/{id}
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            Console.WriteLine($"=== ORDER DETAILS DEBUG ===");
            Console.WriteLine($"OrderID: {id}, UserID: {userId}");

            // FIX: Use explicit loading with Include/ThenInclude
            var order = await _context.tbl_Orders
                .AsNoTracking() // Better performance for read-only
                .Include(o => o.tbl_OrderDetails) // Load order details
                    .ThenInclude(od => od.tbl_Products) // Load products from details
                        .ThenInclude(p => p.tbl_Albums) // Load albums from products
                .Include(o => o.tbl_OrderDetails)
                    .ThenInclude(od => od.tbl_Products)
                        .ThenInclude(p => p.tbl_Producers) // Load producers if needed
                .FirstOrDefaultAsync(o => o.OrderID == id && o.UserID == userId);

            if (order == null)
            {
                Console.WriteLine($"❌ Order {id} not found for user {userId}");
                return NotFound();
            }

            Console.WriteLine($"✅ Found order #{order.OrderID}");
            Console.WriteLine($"📦 Order details count: {order.tbl_OrderDetails?.Count}");

            // Debug: List all order items
            if (order.tbl_OrderDetails != null)
            {
                foreach (var item in order.tbl_OrderDetails)
                {
                    Console.WriteLine($"Item: {item.tbl_Products?.ProductID}, " +
                                    $"Product: {item.tbl_Products?.tbl_Albums?.Title}, " +
                                    $"Quantity: {item.Quantity}");
                }
            }
            else
            {
                Console.WriteLine("❌ Order details are null");
            }

            return View(order);
        }
    }
}