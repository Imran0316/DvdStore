using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DvdDbContext _context;

    public HomeController(ILogger<HomeController> logger, DvdDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var featuredProducts = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .Include(p => p.tbl_Producers)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToList();

        ViewBag.FeaturedProducts = featuredProducts;
        return View();
    }

    public IActionResult Products()
    {
        var products = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .Include(p => p.tbl_Producers)
            .Include(p => p.tbl_Suppliers)
            .Where(p => p.IsActive)
            .ToList();

        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ProductDetails(int id)
    {
        var product = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .Include(p => p.tbl_Producers)
            .Include(p => p.tbl_Suppliers)
            .FirstOrDefault(p => p.ProductID == id && p.IsActive);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}

