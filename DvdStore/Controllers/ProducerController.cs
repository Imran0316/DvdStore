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
        public IActionResult Producer(string ProducerName, string ContactInfo)
        {
            if (ModelState.IsValid)
            {
                var producer = new Producers
                {
                    ProducerName = ProducerName,
                    ContactInfo = ContactInfo,
                    CreatedAt = DateTime.Now
                };

                db.tbl_Producers.Add(producer);
                db.SaveChanges();
                return RedirectToAction("Producer");
            }
            return View();
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

