using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DvdDbContext _context;

        public OrdersController(DvdDbContext context)
        {
            _context = context;
        }

        // GET: Orders/Index - Show all orders (for admin)
        public IActionResult Index()
        {
            var orders = _context.tbl_Orders
                .Include(o => o.tbl_Users)
                .Include(o => o.tbl_OrderDetails)
                .ThenInclude(od => od.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // GET: Orders/Details/{id} - View specific order details
        public IActionResult Details(int id)
        {
            var order = _context.tbl_Orders
                .Include(o => o.tbl_Users)
                .Include(o => o.tbl_OrderDetails)
                .ThenInclude(od => od.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // In OrdersController.cs
        public IActionResult OrderHistory()
        {
            // Debug: Check all session values
            var userId = HttpContext.Session.GetInt32("UserId");
            var userName = HttpContext.Session.GetString("UserName");

            Console.WriteLine($"=== ORDER HISTORY DEBUG ===");
            Console.WriteLine($"Session UserID: {userId}");
            Console.WriteLine($"Session UserName: {userName}");
            Console.WriteLine($"Request Path: {HttpContext.Request.Path}");

            if (userId == null)
            {
                Console.WriteLine("❌ UserID is null - redirecting to login");
                return RedirectToAction("Login", "Auth");
            }

            // Check if user exists in database
            var user = _context.tbl_Users.Find(userId.Value);
            if (user == null)
            {
                Console.WriteLine("❌ User not found in database - clearing session and redirecting");
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Auth");
            }

            Console.WriteLine($"✅ User found: {user.Name} (ID: {user.UserID})");

            // Get orders with proper includes
            var orders = _context.tbl_Orders
                .Include(o => o.tbl_OrderDetails)
                    .ThenInclude(od => od.tbl_Products)
                        .ThenInclude(p => p.tbl_Albums)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            Console.WriteLine($"📦 Found {orders.Count} orders for user {userId}");

            // Debug each order
            foreach (var order in orders)
            {
                Console.WriteLine($"Order #{order.OrderID}, Status: {order.Status}, Items: {order.tbl_OrderDetails?.Count}");
            }

            return View(orders);
        }

        // POST: Orders/UpdateStatus/{id} - Update order status (for admin)
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var order = _context.tbl_Orders.Find(id);
            if (order != null)
            {
                order.Status = status;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}