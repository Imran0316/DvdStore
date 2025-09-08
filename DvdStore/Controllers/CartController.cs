using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class CartController : Controller
    {
        private readonly DvdDbContext _context;

        public CartController(DvdDbContext context)
        {
            _context = context;
        }

        // Add to Cart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Find or create cart for user
            var cart = _context.tbl_Carts
                .Include(c => c.tbl_CartItems)
                .FirstOrDefault(c => c.UserID == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserID = userId.Value,
                    CreatedAt = DateTime.Now
                };
                _context.tbl_Carts.Add(cart);
                _context.SaveChanges();
            }

            // Check if product already in cart
            var existingItem = cart.tbl_CartItems?.FirstOrDefault(ci => ci.ProductID == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItems
                {
                    CartID = cart.CartID,
                    ProductID = productId,
                    Quantity = quantity
                };
                _context.tbl_CartItems.Add(cartItem);
            }

            _context.SaveChanges();

            // After _context.SaveChanges(); in AddToCart method:
            var cartCount = _context.tbl_CartItems
                .Count(ci => ci.CartID == cart.CartID);
            HttpContext.Session.SetInt32("CartCount", cartCount);

            return RedirectToAction("Index");
        }

        // View Cart
        public IActionResult Index()
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

            return View(cart);
        }

        // Remove from Cart
        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.tbl_CartItems.Find(cartItemId);
            if (cartItem != null)
            {
                _context.tbl_CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Update Quantity
        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            var cartItem = _context.tbl_CartItems.Find(cartItemId);
            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    _context.tbl_CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}