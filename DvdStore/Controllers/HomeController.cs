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

    public async Task<IActionResult> Index()
    {
        // Get active hero images ordered by display order
        var heroImages = await _context.tbl_HeroImages
            .Where(h => h.IsActive)
            .OrderBy(h => h.DisplayOrder)
            .ToListAsync();

        ViewBag.HeroImages = heroImages;


        // Featured Products (handpicked - newest products)
        var featuredProducts = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .ThenInclude(a => a.tbl_Category)
            .Include(p => p.tbl_Producers)
            .Where(p => p.IsActive && p.StockQuantity > 0)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToList();

        // Top Selling (based on order history - we need to join with OrderDetails)
        var topSellingProductIds = _context.tbl_OrderDetails
            .GroupBy(od => od.ProductID)
            .Select(g => new { ProductID = g.Key, TotalSold = g.Sum(od => od.Quantity) })
            .OrderByDescending(x => x.TotalSold)
            .Take(12)
            .Select(x => x.ProductID)
            .ToList();

        var topSelling = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .ThenInclude(a => a.tbl_Category)
            .Include(p => p.tbl_Producers)
            .Where(p => topSellingProductIds.Contains(p.ProductID) && p.IsActive && p.StockQuantity > 0)
            .ToList();

        // New Arrivals (recently added)
        var newArrivals = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .ThenInclude(a => a.tbl_Category)
            .Include(p => p.tbl_Producers)
            .Where(p => p.IsActive && p.StockQuantity > 0)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToList();

        // Statistics - Fixed to use proper relationships
        ViewBag.GamesCount = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .ThenInclude(a => a.tbl_Category)
            .Count(p => p.tbl_Albums.tbl_Category.CategoryName.Contains("Game") && p.IsActive);

        ViewBag.MoviesCount = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .ThenInclude(a => a.tbl_Category)
            .Count(p => p.tbl_Albums.tbl_Category.CategoryName.Contains("Movie") && p.IsActive);

        ViewBag.MusicCount = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .ThenInclude(a => a.tbl_Category)
            .Count(p => p.tbl_Albums.tbl_Category.CategoryName.Contains("Music") && p.IsActive);

        ViewBag.CustomersCount = _context.tbl_Users.Count();

        ViewBag.FeaturedProducts = featuredProducts;
        ViewBag.TopSelling = topSelling;
        ViewBag.NewArrivals = newArrivals;

        return View();
    }

    public IActionResult Products()
    {
        var products = _context.tbl_Products
            .Include(p => p.tbl_Albums)
            .ThenInclude(a => a.tbl_Category)
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
            .ThenInclude(a => a.tbl_Artists)
            .Include(p => p.tbl_Producers)
            .Include(p => p.tbl_Suppliers)
            .Include(p => p.tbl_Albums)
            .ThenInclude(a => a.tbl_Category)
            .FirstOrDefault(p => p.ProductID == id && p.IsActive);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    public async Task<IActionResult> News()
    {
        var news = await _context.tbl_NewsPromotions
            .Where(n => n.IsActive && n.PublishDate <= DateTime.Now &&
                       (n.ExpiryDate == null || n.ExpiryDate >= DateTime.Now))
            .OrderByDescending(n => n.PublishDate)
            .ToListAsync();

        return View(news);
    }
}