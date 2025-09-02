using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DvdStore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DvdStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DvdDbContext _context;

        public ProductsController(DvdDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.tbl_Producers)
                .ToListAsync();

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.tbl_Producers)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["AlbumID"] = new SelectList(_context.tbl_Albums, "AlbumID", "Title");
            ViewData["SupplierID"] = new SelectList(_context.tbl_Suppliers, "SupplierID", "SupplierName");
            ViewData["ProducerID"] = new SelectList(_context.tbl_Producers, "ProducerID", "ProducerName");

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumID,SupplierID,ProducerID,SKU,StockQuantity,Price,IsActive")] Products product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdowns if validation fails
            ViewData["AlbumID"] = new SelectList(_context.tbl_Albums, "AlbumID", "Title", product.AlbumID);
            ViewData["SupplierID"] = new SelectList(_context.tbl_Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            ViewData["ProducerID"] = new SelectList(_context.tbl_Producers, "ProducerID", "ProducerName", product.ProducerID);

            return View(product);
        }
        
        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.tbl_Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewData["AlbumID"] = new SelectList(_context.tbl_Albums, "AlbumID", "Title", product.AlbumID);
            ViewData["SupplierID"] = new SelectList(_context.tbl_Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            ViewData["ProducerID"] = new SelectList(_context.tbl_Producers, "ProducerID", "ProducerName", product.ProducerID);

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,AlbumID,SupplierID,ProducerID,SKU,StockQuantity,Price,IsActive")] Products product)
        {
            if (id != product.ProductID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["AlbumID"] = new SelectList(_context.tbl_Albums, "AlbumID", "Title", product.AlbumID);
            ViewData["SupplierID"] = new SelectList(_context.tbl_Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            ViewData["ProducerID"] = new SelectList(_context.tbl_Producers, "ProducerID", "ProducerName", product.ProducerID);

            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.tbl_Producers)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.tbl_Products.FindAsync(id);
            if (product != null)
            {
                _context.tbl_Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.tbl_Products.Any(e => e.ProductID == id);
        }
    }
}
