using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class PurchaseInvoiceController : BaseAdminController
    {
        private readonly DvdDbContext _context;
        public PurchaseInvoiceController(DvdDbContext context) { _context = context; }

        // Index - list
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.tbl_PurchaseInvoices
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.InvoiceDetails)
                    .ThenInclude(d => d.tbl_Products)
                        .ThenInclude(p => p.tbl_Albums)
                .OrderByDescending(p => p.InvoiceDate)
                .ToListAsync();

            return View(invoices);
        }

        // Details
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.tbl_PurchaseInvoices
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.InvoiceDetails)
                    .ThenInclude(d => d.tbl_Products)
                        .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefaultAsync(p => p.PurchaseInvoiceID == id);

            if (invoice == null) return NotFound();
            return View(invoice);
        }
        // GET: PurchaseInvoice/Create
        [HttpGet]                                  
        public IActionResult Create()
        {
            ViewBag.Suppliers = _context.tbl_Suppliers.ToList();
            ViewBag.Products = _context.tbl_Products
                                        .Include(p => p.tbl_Albums)
                                        .Where(p => p.IsActive)
                                        .ToList();
            return View(new PurchaseInvoice());
        }

        // GET Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
          PurchaseInvoice invoice,
          List<int> ProductID,     
          List<int> Quantity,
          List<decimal> UnitCost)
        {

            if (!ModelState.IsValid || ProductID == null || ProductID.Count == 0)
            {
                TempData["Error"] = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage));
                ViewBag.Suppliers = _context.tbl_Suppliers.ToList();
                ViewBag.Products = _context.tbl_Products.Include(p => p.tbl_Albums)
                                                         .Where(p => p.IsActive).ToList();
                return View(invoice);
            }


            invoice.InvoiceDate = DateTime.Now;
            invoice.InvoiceDetails = new List<PurchaseInvoiceDetail>();
            decimal totalAmount = 0m;

            for (int i = 0; i < ProductID.Count; i++)
            {
                var pid = ProductID[i];
                var qty = Quantity.ElementAtOrDefault(i);
                var cost = UnitCost.ElementAtOrDefault(i);

                var product = await _context.tbl_Products.FindAsync(pid);
                if (product == null) continue;

                invoice.InvoiceDetails.Add(new PurchaseInvoiceDetail
                {
                    ProductID = pid,
                    Quantity = qty,
                    UnitCost = cost
                });

                product.StockQuantity += qty;
                totalAmount += qty * cost;
            }

            invoice.TotalAmount = totalAmount;
            _context.tbl_PurchaseInvoices.Add(invoice);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Purchase invoice created successfully!";
            return RedirectToAction(nameof(Index));
        }


        // GET Edit
        public async Task<IActionResult> Edit(int id)
        {
            var invoice = await _context.tbl_PurchaseInvoices
                .Include(p => p.InvoiceDetails)
                    .ThenInclude(d => d.tbl_Products)
                        .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefaultAsync(p => p.PurchaseInvoiceID == id);

            if (invoice == null) return NotFound();

            ViewBag.Suppliers = _context.tbl_Suppliers.ToList();
            ViewBag.Products = _context.tbl_Products.Include(p => p.tbl_Albums).Where(p => p.IsActive).ToList();

            return View(invoice);
        }

        // POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseInvoice invoice, List<int> productIds, List<int> quantities, List<decimal> unitCosts)
        {
            if (id != invoice.PurchaseInvoiceID) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = _context.tbl_Suppliers.ToList();
                ViewBag.Products = _context.tbl_Products.Include(p => p.tbl_Albums).Where(p => p.IsActive).ToList();
                return View(invoice);
            }

            var existing = await _context.tbl_PurchaseInvoices
                .Include(p => p.InvoiceDetails)
                .FirstOrDefaultAsync(p => p.PurchaseInvoiceID == id);

            if (existing == null) return NotFound();

            // revert stock for old details
            foreach (var d in existing.InvoiceDetails.ToList())
            {
                var prod = await _context.tbl_Products.FindAsync(d.ProductID);
                if (prod != null) prod.StockQuantity -= d.Quantity;
                _context.tbl_PurchaseInvoiceDetails.Remove(d);
            }

            // add new details and update stock
            existing.InvoiceDetails = new List<PurchaseInvoiceDetail>();
            decimal total = 0m;
            for (int i = 0; i < productIds.Count; i++)
            {
                var pid = productIds[i];
                var qty = quantities.ElementAtOrDefault(i);
                var cost = unitCosts.ElementAtOrDefault(i);

                var prod = await _context.tbl_Products.FindAsync(pid);
                if (prod == null) continue;

                var d = new PurchaseInvoiceDetail
                {
                    ProductID = pid,
                    Quantity = qty,
                    UnitCost = cost,
                    PurchaseInvoiceID = id
                };
                existing.InvoiceDetails.Add(d);
                total += qty * cost;
                prod.StockQuantity += qty;
            }

            existing.SupplierID = invoice.SupplierID;
            existing.InvoiceDate = invoice.InvoiceDate;
            existing.TotalAmount = total;
            existing.Notes = invoice.Notes;

            _context.tbl_PurchaseInvoices.Update(existing);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Purchase invoice updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET Delete
        public async Task<IActionResult> Delete(int id)
        {
            var invoice = await _context.tbl_PurchaseInvoices
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.InvoiceDetails)
                    .ThenInclude(d => d.tbl_Products)
                        .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefaultAsync(p => p.PurchaseInvoiceID == id);

            if (invoice == null) return NotFound();
            return View(invoice);
        }

        // POST DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.tbl_PurchaseInvoices
                .Include(p => p.InvoiceDetails)
                .FirstOrDefaultAsync(p => p.PurchaseInvoiceID == id);

            if (invoice != null)
            {
                // revert stock
                foreach (var d in invoice.InvoiceDetails)
                {
                    var prod = await _context.tbl_Products.FindAsync(d.ProductID);
                    if (prod != null) prod.StockQuantity -= d.Quantity;
                }

                _context.tbl_PurchaseInvoices.Remove(invoice);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Purchase invoice deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // Print view
        public async Task<IActionResult> Print(int id)
        {
            var invoice = await _context.tbl_PurchaseInvoices
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.InvoiceDetails)
                    .ThenInclude(d => d.tbl_Products)
                        .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefaultAsync(p => p.PurchaseInvoiceID == id);

            if (invoice == null) return NotFound();
            return View(invoice);
        }

        // AJAX product details
        [HttpGet]
        public IActionResult GetProductDetails(int productId)
        {
            var product = _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .FirstOrDefault(p => p.ProductID == productId);

            if (product == null) return Json(new { success = false });
            return Json(new
            {
                success = true,
                productName = product.tbl_Albums?.Title,
                currentStock = product.StockQuantity,
                price = product.Price
            });
        }
    }
}
