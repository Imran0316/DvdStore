using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DvdDbContext _context;

        public CheckoutController(DvdDbContext context)
        {
            _context = context;
        }

        // GET: Checkout/ReviewOrder
        public IActionResult ReviewOrder()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cart = _context.tbl_Carts
                .Include(c => c.tbl_CartItems)
                .ThenInclude(ci => ci.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefault(c => c.UserID == userId);

            if (cart == null || !cart.tbl_CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Cart = cart;
            ViewBag.Total = cart.tbl_CartItems.Sum(item => item.tbl_Products.Price * item.Quantity);

            return View(cart);
        }

        // POST: Checkout/ProcessOrder
        [HttpPost]
        public IActionResult ProcessOrder(string shippingAddress, string paymentMethod)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cart = _context.tbl_Carts
                .Include(c => c.tbl_CartItems)
                .ThenInclude(ci => ci.tbl_Products)
                .FirstOrDefault(c => c.UserID == userId);

            if (cart == null || !cart.tbl_CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // Create order
            var order = new Orders
            {
                UserID = userId.Value,
                OrderDate = DateTime.Now,
                TotalAmount = cart.tbl_CartItems.Sum(item => item.tbl_Products.Price * item.Quantity),
                Status = "Processing",
                ShippingAddress = shippingAddress
            };

            _context.tbl_Orders.Add(order);
            _context.SaveChanges();

            // Create order details
            foreach (var cartItem in cart.tbl_CartItems)
            {
                var orderDetail = new OrderDetails
                {
                    OrderID = order.OrderID,
                    ProductID = cartItem.ProductID,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.tbl_Products.Price
                };

                _context.tbl_OrderDetails.Add(orderDetail);

                // Update product stock
                var product = _context.tbl_Products.Find(cartItem.ProductID);
                if (product != null)
                {
                    product.StockQuantity -= cartItem.Quantity;
                }
            }

            // Create payment record
            var payment = new Payments
            {
                OrderID = order.OrderID,
                UserID = userId.Value,
                PaymentDate = DateTime.Now,
                Amount = order.TotalAmount,
                PaymentMethod = paymentMethod,
                Status = "Completed"
            };

            _context.tbl_Payments.Add(payment);

            // Clear cart
            _context.tbl_CartItems.RemoveRange(cart.tbl_CartItems);
            _context.SaveChanges();

            // Update cart count in session
            HttpContext.Session.SetInt32("CartCount", 0);

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderID });
        }

        // GET: Checkout/OrderConfirmation/{orderId}
        public IActionResult OrderConfirmation(int orderId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var order = _context.tbl_Orders
                .Include(o => o.tbl_OrderDetails)
                .ThenInclude(od => od.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefault(o => o.OrderID == orderId && o.UserID == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}