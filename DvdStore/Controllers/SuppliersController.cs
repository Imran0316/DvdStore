using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DvdStore.Controllers
{
    public class SuppliersController : Controller
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
            try
            {
                if (ModelState.IsValid)
                {
                    var supplier = new Suppliers
                    {
                        SupplierName = SupplierName,
                        ContactInfo = ContactInfo,
                        CreatedAt = DateTime.Now
                    };

                    db.tbl_Suppliers.Add(supplier);
                    db.SaveChanges();

                    ViewBag.Success = "Supplier added successfully!";
                    return RedirectToAction("Suppliers"); // Redirect to refresh the list
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error adding supplier: " + ex.Message;
            }

            // If we get here, something went wrong - redisplay the form
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

