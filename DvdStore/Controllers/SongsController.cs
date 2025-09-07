using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class SongsController : Controller
    {
        private readonly DvdDbContext db;

        public SongsController(DvdDbContext context)
        {
            db = context;
        }

        // Show All Songs
        public IActionResult Songs()
        {
            var songs = db.tbl_Songs
                          .Include(s => s.Album)   // Album ka data include karega
                          .ToList();

            ViewBag.Albums = db.tbl_Albums.ToList(); // dropdown ke liye albums
            return View(songs);
        }

        // Add New Song (POST)
        [HttpPost]
        public IActionResult Songs(Songs song, IFormFile AudioFile)
        {
            if (AudioFile != null && AudioFile.Length > 0)
            {
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "songs");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AudioFile.FileName);
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    AudioFile.CopyTo(stream);
                }

                song.FileUrl = "/uploads/songs/" + fileName;
            }

            db.tbl_Songs.Add(song);
            db.SaveChanges();
            return RedirectToAction("Songs");
        }

        // Edit Song (GET)
        [HttpGet]
        public IActionResult EditSong(int id)
        {
            if (id == 0) return NotFound();

            var song = db.tbl_Songs.FirstOrDefault(s => s.SongID == id);
            if (song == null) return NotFound();

            ViewBag.Albums = db.tbl_Albums.ToList();
            return View(song);
        }

        // Edit Song (POST)
        [HttpPost]
        public IActionResult EditSong(Songs model, IFormFile? AudioFile)
        {
            var song = db.tbl_Songs.FirstOrDefault(s => s.SongID == model.SongID);
            if (song != null)
            {
                song.Title = model.Title;
                song.Duration = model.Duration;
                song.AlbumID = model.AlbumID;

                if (AudioFile != null && AudioFile.Length > 0)
                {
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "songs");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AudioFile.FileName);
                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        AudioFile.CopyTo(stream);
                    }

                    song.FileUrl = "/uploads/songs/" + fileName;
                }

                db.SaveChanges();
            }

            return RedirectToAction("Songs");
        }

        // Delete Song
        [HttpGet]
        public IActionResult DeleteSong(int id)
        {
            var song = db.tbl_Songs.FirstOrDefault(s => s.SongID == id);
            if (song == null) return NotFound();

            db.tbl_Songs.Remove(song);
            db.SaveChanges();

            return RedirectToAction("Songs");
        }
    }
}

