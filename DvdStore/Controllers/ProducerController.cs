using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DvdStore.Controllers
{
    public class ProducerController : BaseAdminController
    {

        private readonly DvdDbContext db;
        public ProducerController(DvdDbContext context)
        {
            db = context;
        }
        public IActionResult Producer()
        {
            var Producer = db.tbl_Producers.ToList();
            return View(Producer);

        }


        [HttpPost]
        public IActionResult Producer(Producers producer) // ✅ Producers model as parameter
        {
            // Manual validation karo
            if (string.IsNullOrEmpty(producer.ProducerName))
            {
                ViewBag.Error = "Producer name is required!";
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(producer.ProducerName, @"^[a-zA-Z\s]+$"))
            {
                ViewBag.Error = "Producer name can only contain letters and spaces!";
            }
            else if (string.IsNullOrEmpty(producer.ContactInfo))
            {
                ViewBag.Error = "Phone number is required!";
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(producer.ContactInfo, @"^\d{11}$"))
            {
                ViewBag.Error = "Phone number must be exactly 11 digits!";
            }
            else
            {
                // Success case
                producer.CreatedAt = DateTime.Now;
                db.tbl_Producers.Add(producer);
                db.SaveChanges();

                ViewBag.Success = "Producer added successfully!";
            }

            var producers = db.tbl_Producers.ToList();
            return View(producers);
        }
        //Edit producer
        [HttpGet]
        public IActionResult EditProducer(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var Producer = db.tbl_Producers.FirstOrDefault(c => c.ProducerID == id);

            if (Producer == null)
            {
                return NotFound();
            }

            return View(Producer);
        }
        [HttpPost]
        public IActionResult EditProducer(Producers model)
        {
            if (ModelState.IsValid)
            {
                var Producer = db.tbl_Producers.FirstOrDefault(c => c.ProducerID == model.ProducerID);
                if (Producer != null)
                {
                    Producer.ProducerName = model.ProducerName;
                    Producer.ContactInfo = model.ContactInfo;
                    db.SaveChanges();
                }
                return RedirectToAction("Producer");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteProducer(int id)
        {
            var Producer = db.tbl_Producers.FirstOrDefault(c => c.ProducerID == id);
            if (Producer == null)
            {
                return NotFound();
            }

            db.tbl_Producers.Remove(Producer);
            db.SaveChanges();

            return RedirectToAction("Producer");
        }
    }
}

