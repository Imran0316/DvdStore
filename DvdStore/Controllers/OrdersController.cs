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

        // GET: Orders/OrderHistory - Show order history for logged-in user
        public IActionResult OrderHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var orders = _context.tbl_Orders
                .Include(o => o.tbl_OrderDetails)
                .ThenInclude(od => od.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

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