﻿using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class PurchaseInvoiceController : BaseAdminController
    {
        private readonly DvdDbContext _context;

        public PurchaseInvoiceController(DvdDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseInvoice
        public IActionResult Index()
        {
            var invoices = _context.tbl_PurchaseInvoices
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.InvoiceDetails)
                .ThenInclude(d => d.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .OrderByDescending(p => p.InvoiceDate)
                .ToList();

            return View(invoices);
        }

        // GET: PurchaseInvoice/Details/5
        public IActionResult Details(int id)
        {
            var invoice = _context.tbl_PurchaseInvoices
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.InvoiceDetails)
                .ThenInclude(d => d.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefault(p => p.PurchaseInvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: PurchaseInvoice/Create
        public IActionResult Create()
        {
            ViewBag.Suppliers = _context.tbl_Suppliers.ToList();
            ViewBag.Products = _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .Where(p => p.IsActive)
                .ToList();

            return View();
        }

        // POST: PurchaseInvoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PurchaseInvoice invoice, List<int> productIds, List<int> quantities, List<decimal> unitCosts)
        {
            if (ModelState.IsValid && productIds != null && productIds.Count > 0)
            {
                invoice.InvoiceDate = DateTime.Now;
                invoice.InvoiceDetails = new List<PurchaseInvoiceDetail>();

                decimal totalAmount = 0;

                for (int i = 0; i < productIds.Count; i++)
                {
                    var product = _context.tbl_Products.Find(productIds[i]);
                    if (product != null)
                    {
                        var detail = new PurchaseInvoiceDetail
                        {
                            ProductID = productIds[i],
                            Quantity = quantities[i],
                            UnitCost = unitCosts[i]
                        };

                        totalAmount += quantities[i] * unitCosts[i];
                        invoice.InvoiceDetails.Add(detail);

                        // Update product stock
                        product.StockQuantity += quantities[i];
                    }
                }

                invoice.TotalAmount = totalAmount;

                _context.tbl_PurchaseInvoices.Add(invoice);
                _context.SaveChanges();

                TempData["Success"] = "Purchase invoice created successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Suppliers = _context.tbl_Suppliers.ToList();
            ViewBag.Products = _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .Where(p => p.IsActive)
                .ToList();

            return View(invoice);
        }

        // GET: PurchaseInvoice/Edit/5
        public IActionResult Edit(int id)
        {
            var invoice = _context.tbl_PurchaseInvoices
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.InvoiceDetails)
                .ThenInclude(d => d.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefault(p => p.PurchaseInvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }

            ViewBag.Suppliers = _context.tbl_Suppliers.ToList();
            ViewBag.Products = _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .Where(p => p.IsActive)
                .ToList();

            return View(invoice);
        }

        // POST: PurchaseInvoice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PurchaseInvoice invoice, List<int> productIds, List<int> quantities, List<decimal> unitCosts)
        {
            if (id != invoice.PurchaseInvoiceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingInvoice = _context.tbl_PurchaseInvoices
                        .Include(p => p.InvoiceDetails)
                        .FirstOrDefault(p => p.PurchaseInvoiceID == id);

                    if (existingInvoice == null)
                    {
                        return NotFound();
                    }

                    // Remove old details and revert stock
                    foreach (var detail in existingInvoice.InvoiceDetails.ToList())
                    {
                        var product = _context.tbl_Products.Find(detail.ProductID);
                        if (product != null)
                        {
                            product.StockQuantity -= detail.Quantity;
                        }
                        _context.tbl_PurchaseInvoiceDetails.Remove(detail);
                    }

                    // Add new details
                    existingInvoice.InvoiceDetails = new List<PurchaseInvoiceDetail>();
                    decimal totalAmount = 0;

                    for (int i = 0; i < productIds.Count; i++)
                    {
                        var product = _context.tbl_Products.Find(productIds[i]);
                        if (product != null)
                        {
                            var detail = new PurchaseInvoiceDetail
                            {
                                ProductID = productIds[i],
                                Quantity = quantities[i],
                                UnitCost = unitCosts[i],
                                PurchaseInvoiceID = id
                            };

                            totalAmount += quantities[i] * unitCosts[i];
                            existingInvoice.InvoiceDetails.Add(detail);

                            // Update product stock
                            product.StockQuantity += quantities[i];
                        }
                    }

                    existingInvoice.SupplierID = invoice.SupplierID;
                    existingInvoice.InvoiceDate = invoice.InvoiceDate;
                    existingInvoice.TotalAmount = totalAmount;
                    existingInvoice.Notes = invoice.Notes;

                    _context.Update(existingInvoice);
                    _context.SaveChanges();

                    TempData["Success"] = "Purchase invoice updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseInvoiceExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Suppliers = _context.tbl_Suppliers.ToList();
            ViewBag.Products = _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .Where(p => p.IsActive)
                .ToList();

            return View(invoice);
        }

        // GET: PurchaseInvoice/Delete/5
        public IActionResult Delete(int id)
        {
            var invoice = _context.tbl_PurchaseInvoices
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.InvoiceDetails)
                .ThenInclude(d => d.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefault(p => p.PurchaseInvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: PurchaseInvoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var invoice = _context.tbl_PurchaseInvoices
                .Include(p => p.InvoiceDetails)
                .FirstOrDefault(p => p.PurchaseInvoiceID == id);

            if (invoice != null)
            {
                // Revert stock changes
                foreach (var detail in invoice.InvoiceDetails)
                {
                    var product = _context.tbl_Products.Find(detail.ProductID);
                    if (product != null)
                    {
                        product.StockQuantity -= detail.Quantity;
                    }
                }

                _context.tbl_PurchaseInvoices.Remove(invoice);
                _context.SaveChanges();

                TempData["Success"] = "Purchase invoice deleted successfully!";
            }

            return RedirectToAction("Index");
        }

        // GET: PurchaseInvoice/Print/5
        public IActionResult Print(int id)
        {
            var invoice = _context.tbl_PurchaseInvoices
                .Include(p => p.tbl_Suppliers)
                .Include(p => p.InvoiceDetails)
                .ThenInclude(d => d.tbl_Products)
                .ThenInclude(p => p.tbl_Albums)
                .FirstOrDefault(p => p.PurchaseInvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        private bool PurchaseInvoiceExists(int id)
        {
            return _context.tbl_PurchaseInvoices.Any(e => e.PurchaseInvoiceID == id);
        }

        // AJAX: Get product details
        [HttpGet]
        public IActionResult GetProductDetails(int productId)
        {
            var product = _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .FirstOrDefault(p => p.ProductID == productId);

            if (product == null)
            {
                return Json(new { success = false });
            }

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
