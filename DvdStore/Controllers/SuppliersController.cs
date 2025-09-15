using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DvdStore.Controllers
{
    public class SuppliersController : BaseAdminController
    {
        private readonly DvdDbContext db;
        public SuppliersController(DvdDbContext context)
        {
            db = context;
        }
        public IActionResult Suppliers()
        {
            var suppliers = db.tbl_Suppliers.ToList(); 
            return View(suppliers); 
            
        }


        [HttpPost]
        public IActionResult Suppliers(string SupplierName, string ContactInfo)
        {
            // Manual validation
            if (string.IsNullOrEmpty(SupplierName))
            {
                ViewBag.Error = "Supplier name is required!";
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(SupplierName, @"^[a-zA-Z\s]+$"))
            {
                ViewBag.Error = "Name can only contain letters and spaces!";
            }
            else if (string.IsNullOrEmpty(ContactInfo))
            {
                ViewBag.Error = "Contact info is required!";
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(ContactInfo, @"^\d{11}$"))
            {
                ViewBag.Error = "Phone number must be 11 digits!";
            }
            else
            {
                // Success case
                var supplier = new Suppliers
                {
                    SupplierName = SupplierName,
                    ContactInfo = ContactInfo,
                    CreatedAt = DateTime.Now
                };

                db.tbl_Suppliers.Add(supplier);
                db.SaveChanges();

                ViewBag.Success = "Supplier added successfully!";
            }

            var suppliers = db.tbl_Suppliers.ToList();
            return View(suppliers);
        }

        //Edit Suppliers
        [HttpGet]
        public IActionResult EditSuppliers(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var supplier = db.tbl_Suppliers.FirstOrDefault(c => c.SupplierID == id);

            if (supplier == null)
            {
                return NotFound(); 
            }

            return View(supplier); 
        }
        [HttpPost]
        public IActionResult EditSuppliers(Suppliers model)
        {
            if (ModelState.IsValid)
            {
                var supplier = db.tbl_Suppliers.FirstOrDefault(c => c.SupplierID == model.SupplierID);
                if (supplier != null)
                {
                    supplier.SupplierName = model.SupplierName;
                    supplier.ContactInfo = model.ContactInfo;
                    db.SaveChanges();
                }
                return RedirectToAction("suppliers");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteSupplier(int id)
        {
            var supplier = db.tbl_Suppliers.FirstOrDefault(c => c.SupplierID == id);
            if (supplier == null)
            {
                return NotFound();
            }

            db.tbl_Suppliers.Remove(supplier);
            db.SaveChanges();

            return RedirectToAction("suppliers");
        }
    }

}

