using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly DvdDbContext db;

        public AlbumsController(DvdDbContext context)
        {
            db = context;
        }

        // Show All Albums
        public IActionResult Albums()
        {

            var albums = db.tbl_Albums
                           .Include(a => a.tbl_Artists)
                           .Include(a => a.tbl_Category)
                           .ToList();

            ViewBag.Artists = db.tbl_Artists.ToList();
            ViewBag.Categories = db.tbl_Category.ToList();

            return View(albums);
        }

        // Add New Album (POST)
        [HttpPost]
        public IActionResult Albums(Albums album, IFormFile CoverImage)
        {
            if (ModelState.IsValid)
            {
                if (CoverImage != null && CoverImage.Length > 0)
                {
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "albums");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CoverImage.FileName);
                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        CoverImage.CopyTo(stream);
                    }

                    album.CoverImageUrl = "/uploads/albums/" + fileName;
                }

                db.tbl_Albums.Add(album);
                db.SaveChanges();
                return RedirectToAction("Albums");
            }

            // ModelState invalid hone par errors 
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            ViewBag.Errors = errors;

            var albums = db.tbl_Albums
                           .Include(a => a.tbl_Artists)
                           .Include(a => a.tbl_Category)
                           .ToList();

            ViewBag.Artists = db.tbl_Artists.ToList();
            ViewBag.Categories = db.tbl_Category.ToList();

            return View(albums);
        }


        // Edit Album (GET)
        [HttpGet]
        public IActionResult EditAlbum(int id)
        {
            if (id == 0) return NotFound();

            var album = db.tbl_Albums.FirstOrDefault(a => a.AlbumID == id);
            if (album == null) return NotFound();

            ViewBag.Artists = db.tbl_Artists.ToList();
            ViewBag.Categories = db.tbl_Category.ToList();

            return View(album);
        }

        // Edit Album (POST)
        [HttpPost]
        public IActionResult EditAlbum(Albums model, IFormFile? CoverImage)
        {
            if (ModelState.IsValid)
            {
                var album = db.tbl_Albums.FirstOrDefault(a => a.AlbumID == model.AlbumID);
                if (album != null)
                {
                    album.Title = model.Title;
                    album.ArtistID = model.ArtistID;
                    album.CategoryID = model.CategoryID;
                    album.ReleaseDate = model.ReleaseDate;
                    album.Description = model.Description;

                    // agar nayi image upload hui hai
                    if (CoverImage != null && CoverImage.Length > 0)
                    {
                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "albums");

                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CoverImage.FileName);
                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            CoverImage.CopyTo(stream);
                        }

                        album.CoverImageUrl = "/uploads/albums/" + fileName;
                    }

                    db.SaveChanges();
                }
                return RedirectToAction("Albums");
            }

            // agar validation fail ho to dropdowns wapas bhejo
            ViewBag.Artists = db.tbl_Artists.ToList();
            ViewBag.Categories = db.tbl_Category.ToList();

            return View(model);
        }

        // Delete Album
        [HttpGet]
        public IActionResult DeleteAlbum(int id)
        {
            var album = db.tbl_Albums.FirstOrDefault(a => a.AlbumID == id);
            if (album == null) return NotFound();

            db.tbl_Albums.Remove(album);
            db.SaveChanges();

            return RedirectToAction("Albums");
        }
    }
}
