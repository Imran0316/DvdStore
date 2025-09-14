    ﻿using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DvdStore.Controllers
{
    public class MediaController : Controller
    {
        private readonly DvdDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public MediaController(DvdDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Stream audio (songs)
        public IActionResult StreamAudio(int id)
        {
            var song = _context.tbl_Songs.Find(id);
            if (song == null || string.IsNullOrEmpty(song.FileUrl))
                return NotFound();

            var filePath = Path.Combine(_environment.WebRootPath, song.FileUrl.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            return PhysicalFile(filePath, "audio/mpeg", enableRangeProcessing: true);
        }

        // Download song
        public IActionResult DownloadSong(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login to download songs";
                return RedirectToAction("Login", "Auth");
            }

            var song = _context.tbl_Songs.Find(id);
            if (song == null || string.IsNullOrEmpty(song.FileUrl))
                return NotFound();

            var filePath = Path.Combine(_environment.WebRootPath, song.FileUrl.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            // Log download
            var download = new DownloadHistory
            {
                UserID = userId.Value,
                SongID = id,
                DownloadDate = DateTime.Now,
                Type = "Song"
            };
            _context.tbl_Downloads.Add(download);
            _context.SaveChanges();

            return PhysicalFile(filePath, "audio/mpeg", $"{song.Title}.mp3");
        }

        // Download trailer - ONLY for local files (not YouTube URLs)
        public IActionResult DownloadTrailer(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login to download trailers";
                return RedirectToAction("Login", "Auth");
            }

            var product = _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .FirstOrDefault(p => p.ProductID == productId);

            if (product == null || string.IsNullOrEmpty(product.TrailerUrl))
                return NotFound();

            // Only allow download for local files, not YouTube URLs
            var trailerUrl = product.TrailerUrl.Trim();
            if (trailerUrl.StartsWith("http") || trailerUrl.StartsWith("www.") ||
                trailerUrl.Contains("youtube") || trailerUrl.Contains("youtu.be"))
            {
                TempData["Info"] = "Online trailers cannot be downloaded";
                return RedirectToAction("Index", "ProductDetail", new { id = productId });
            }

            var filePath = Path.Combine(_environment.WebRootPath, trailerUrl.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            // Log download
            var download = new DownloadHistory
            {
                UserID = userId.Value,
                ProductID = productId,
                DownloadDate = DateTime.Now,
                Type = "Trailer"
            };
            _context.tbl_Downloads.Add(download);
            _context.SaveChanges();

            return PhysicalFile(filePath, "video/mp4", $"{product.tbl_Albums?.Title}_Trailer.mp4");
        }

        // Download sample
        public IActionResult DownloadSample(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login to download samples";
                return RedirectToAction("Login", "Auth");
            }

            var product = _context.tbl_Products
                .Include(p => p.tbl_Albums)
                .FirstOrDefault(p => p.ProductID == productId);

            if (product == null)
                return NotFound();

            // For music products, offer first song as sample
            if (product.tbl_Albums?.tbl_Songs?.Any() == true)
            {
                var firstSong = product.tbl_Albums.tbl_Songs.First();
                return DownloadSong(firstSong.SongID);
            }

            // For movies/games with local trailers, offer trailer as sample
            if (!string.IsNullOrEmpty(product.TrailerUrl) &&
                !product.TrailerUrl.StartsWith("http") &&
                !product.TrailerUrl.Contains("youtube") &&
                !product.TrailerUrl.Contains("youtu.be"))
            {
                return DownloadTrailer(productId);
            }

            TempData["Info"] = "No sample available for this product";
            return RedirectToAction("Index", "ProductDetail", new { id = productId });
        }

        // View download history
        public IActionResult DownloadHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var downloads = _context.tbl_Downloads
                .Include(d => d.Song)
                .ThenInclude(s => s.Album)
                .Include(d => d.Product)
                .ThenInclude(p => p.tbl_Albums)
                .Where(d => d.UserID == userId)
                .OrderByDescending(d => d.DownloadDate)
                .ToList();

            return View(downloads);
        }
    }
}
