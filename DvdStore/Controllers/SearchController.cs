// Add SearchController.cs
using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SearchController : Controller
{
    private readonly DvdDbContext _context;

    public SearchController(DvdDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string query, string category = "")
    {
        ViewBag.Query = query;
        ViewBag.Category = category;

        var products = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .ThenInclude(a => a.tbl_Artists)
            .Include(p => p.tbl_Albums)
            .ThenInclude(a => a.tbl_Category)
            .Include(p => p.tbl_Producers)
            .Where(p => p.IsActive);

        if (!string.IsNullOrEmpty(query))
        {
            products = products.Where(p =>
                p.tbl_Albums.Title.Contains(query) ||
                p.tbl_Albums.tbl_Artists.ArtistName.Contains(query) ||
                p.tbl_Producers.ProducerName.Contains(query) ||
                p.tbl_Albums.Description.Contains(query));
        }

        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p =>
                p.tbl_Albums.tbl_Category.CategoryName.Contains(category));
        }

        return View(products.ToList());
    }
}