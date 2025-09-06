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
                // agar file upload hui hai
                if (CoverImage != null && CoverImage.Length > 0)
                {
                    // folder path (wwwroot ke andar)
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "albums");

                    // agar folder nahi hai to create karo
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // unique filename
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CoverImage.FileName);

                    // full path
                    var filePath = Path.Combine(uploadFolder, fileName);

                    // file save
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        CoverImage.CopyTo(stream);
                    }

                    // relative path save database me
                    album.CoverImageUrl = "/uploads/albums/" + fileName;
                }


                db.tbl_Albums.Add(album);
                db.SaveChanges();
                return RedirectToAction("Albums");

            }
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

            return View(album);
        }

        // Edit Album (POST)
        [HttpPost]
        public IActionResult EditAlbum(Albums model)
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
                    album.CoverImageUrl = model.CoverImageUrl;
                    db.SaveChanges();
                }
                return RedirectToAction("Albums");
            }
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
