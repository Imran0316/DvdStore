using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DvdStore.Controllers
{
    public class ProducerController : Controller
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
        public IActionResult Producer(Producers p)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Producers.Add(p);
                db.SaveChanges();
                return RedirectToAction();

            }
            return View();
        }
        //Edit Suppliers
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

