using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class ProductsController : BaseAdminController
    {
        private readonly DvdDbContext db;

        public ProductsController(DvdDbContext context)
        {
            db = context;
        }

        // In ProductsController.cs - Update the Products action
        [HttpGet]
        public IActionResult Products()
        {
            ViewBag.Albums = db.tbl_Albums.ToList();
            ViewBag.Suppliers = db.tbl_Suppliers.ToList();
            ViewBag.Producers = db.tbl_Producers.ToList();

            var products = db.tbl_Products
                             .Include(p => p.tbl_Albums)
                             .Include(p => p.tbl_Suppliers)
                             .Include(p => p.tbl_Producers)
                             .Where(p => p.IsActive) // Only show active products
                             .ToList();

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> Products(Products product, IFormFile ProductImage)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (ProductImage != null && ProductImage.Length > 0)
                {
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProductImage.FileName);
                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProductImage.CopyToAsync(stream);
                    }

                    product.ProductImageUrl = "/uploads/products/" + fileName;
                }

                db.tbl_Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Products");
            }

            // If we got this far, something failed; redisplay form
            ViewBag.Albums = db.tbl_Albums.ToList();
            ViewBag.Suppliers = db.tbl_Suppliers.ToList();
            ViewBag.Producers = db.tbl_Producers.ToList();

            var products = db.tbl_Products
                             .Include(p => p.tbl_Albums)
                             .Include(p => p.tbl_Suppliers)
                             .Include(p => p.tbl_Producers)
                             .ToList();

            return View(products);
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = db.tbl_Products.FirstOrDefault(p => p.ProductID == id);
            if (product == null) return NotFound();

            // ✅ Always set dropdown data
            ViewBag.Albums = db.tbl_Albums.ToList();
            ViewBag.Suppliers = db.tbl_Suppliers.ToList();
            ViewBag.Producers = db.tbl_Producers.ToList();

            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(Products model, IFormFile? ProductImage)
        {
            if (ModelState.IsValid)
            {
                var product = db.tbl_Products.FirstOrDefault(p => p.ProductID == model.ProductID);
                if (product != null)
                {
                    product.AlbumID = model.AlbumID;
                    product.SupplierID = model.SupplierID;
                    product.ProducerID = model.ProducerID;
                    product.SKU = model.SKU;
                    product.StockQuantity = model.StockQuantity;
                    product.Price = model.Price;
                    product.TrailerUrl = model.TrailerUrl;
                    product.IsActive = model.IsActive;

                    // ✅ Image upload
                    if (ProductImage != null && ProductImage.Length > 0)
                    {
                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProductImage.FileName);
                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            ProductImage.CopyTo(stream);
                        }

                        product.ProductImageUrl = "/uploads/products/" + fileName;
                    }

                    db.SaveChanges();
                    return RedirectToAction("Products");
                }
            }

            // ✅ Agar error ho (ModelState invalid ya product not found)
            // Always set dropdowns again
            ViewBag.Albums = db.tbl_Albums.ToList();
            ViewBag.Suppliers = db.tbl_Suppliers.ToList();
            ViewBag.Producers = db.tbl_Producers.ToList();


            return View(model);
        }


        // 🟢 Delete Product
        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var product = db.tbl_Products.FirstOrDefault(p => p.ProductID == id);
            if (product == null) return NotFound();

            db.tbl_Products.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Products");
        }
    }
}
