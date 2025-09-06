using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DvdStore.Controllers
{
    public class ArtistController : Controller
    {
        private readonly DvdDbContext db;
        public ArtistController(DvdDbContext context)
        {
            db = context;
        }

        // Show All Artists
        public IActionResult Artists()
        {
            var artists = db.tbl_Artists.ToList();
            return View(artists);
        }

        // Add New Artist (POST)
        [HttpPost]
        public IActionResult Artists(Artists a)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Artists.Add(a);
                db.SaveChanges();
                return RedirectToAction("Artists");
            }
            return View();
        }

        // Edit Artist (GET)
        [HttpGet]
        public IActionResult EditArtist(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var artist = db.tbl_Artists.FirstOrDefault(c => c.ArtistID == id);

            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // Edit Artist (POST)
        [HttpPost]
        public IActionResult EditArtist(Artists model)
        {
            if (ModelState.IsValid)
            {
                var artist = db.tbl_Artists.FirstOrDefault(c => c.ArtistID == model.ArtistID);
                if (artist != null)
                {
                    artist.ArtistName = model.ArtistName;
                    artist.Bio = model.Bio;
                    db.SaveChanges();
                }
                return RedirectToAction("Artists");
            }
            return View(model);
        }

        // Delete Artist
        [HttpGet]
        public IActionResult DeleteArtist(int id)
        {
            var artist = db.tbl_Artists.FirstOrDefault(c => c.ArtistID == id);
            if (artist == null)
            {
                return NotFound();
            }

            db.tbl_Artists.Remove(artist);
            db.SaveChanges();

            return RedirectToAction("Artists");
        }
    }
}
