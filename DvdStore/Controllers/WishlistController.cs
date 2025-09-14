﻿using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class WishlistController : Controller
    {
        private readonly DvdDbContext _context;

        public WishlistController(DvdDbContext context)
        {
            _context = context;
        }

        // GET: Wishlist
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login to view your wishlist";
                return RedirectToAction("Login", "Auth");
            }

            var wishlistItems = _context.tbl_Wishlists
                .Include(w => w.Product)
                .ThenInclude(p => p.tbl_Albums)
                .ThenInclude(a => a.tbl_Artists)
                .Include(w => w.Product)
                .ThenInclude(p => p.tbl_Producers)
                .Where(w => w.UserID == userId)
                .ToList();

            return View(wishlistItems);
        }

        // POST: Wishlist/AddToWishlist
        [HttpPost]
        public IActionResult AddToWishlist(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login to add items to wishlist";
                return RedirectToAction("Login", "Auth");
            }

            // Check if product exists
            var product = _context.tbl_Products.Find(productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToAction("Index", "Home");
            }

            // Check if already in wishlist
            var existingItem = _context.tbl_Wishlists
                .FirstOrDefault(w => w.UserID == userId && w.ProductID == productId);

            if (existingItem != null)
            {
                TempData["Info"] = "Product is already in your wishlist";
                return RedirectToAction("Index", "ProductDetail", new { id = productId });
            }

            // Add to wishlist
            var wishlistItem = new Wishlist
            {
                UserID = userId.Value,
                ProductID = productId,
                AddedDate = DateTime.Now
            };

            _context.tbl_Wishlists.Add(wishlistItem);
            _context.SaveChanges();

            TempData["Success"] = "Added to wishlist successfully!";
            return RedirectToAction("Index", "ProductDetail", new { id = productId });
        }

        // POST: Wishlist/RemoveFromWishlist
        [HttpPost]
        public IActionResult RemoveFromWishlist(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login to manage your wishlist";
                return RedirectToAction("Login", "Auth");
            }

            var wishlistItem = _context.tbl_Wishlists
                .FirstOrDefault(w => w.WishlistID == id && w.UserID == userId);

            if (wishlistItem == null)
            {
                TempData["Error"] = "Wishlist item not found";
                return RedirectToAction("Index");
            }

            _context.tbl_Wishlists.Remove(wishlistItem);
            _context.SaveChanges();

            TempData["Success"] = "Removed from wishlist successfully";
            return RedirectToAction("Index");
        }

        // GET: Wishlist/MoveToCart/{id}
        public IActionResult MoveToCart(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login to manage your wishlist";
                return RedirectToAction("Login", "Auth");
            }

            var wishlistItem = _context.tbl_Wishlists
                .Include(w => w.Product)
                .FirstOrDefault(w => w.WishlistID == id && w.UserID == userId);

            if (wishlistItem == null)
            {
                TempData["Error"] = "Wishlist item not found";
                return RedirectToAction("Index");
            }

            // Find or create cart
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
            var existingCartItem = cart.tbl_CartItems?
                .FirstOrDefault(ci => ci.ProductID == wishlistItem.ProductID);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
            }
            else
            {
                var cartItem = new CartItems
                {
                    CartID = cart.CartID,
                    ProductID = wishlistItem.ProductID,
                    Quantity = 1
                };
                _context.tbl_CartItems.Add(cartItem);
            }

            // Remove from wishlist
            _context.tbl_Wishlists.Remove(wishlistItem);
            _context.SaveChanges();

            // Update cart count in session
            var cartCount = _context.tbl_CartItems
                .Count(ci => ci.CartID == cart.CartID);
            HttpContext.Session.SetInt32("CartCount", cartCount);

            TempData["Success"] = "Moved to cart successfully!";
            return RedirectToAction("Index", "Cart");
        }

        // GET: Wishlist/Count
        public IActionResult GetWishlistCount()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { count = 0 });
            }

            var count = _context.tbl_Wishlists
                .Count(w => w.UserID == userId);

            return Json(new { count = count });
        }
    }
}
