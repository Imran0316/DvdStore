using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DvdStore.Controllers
{
    public class ReportsController : BaseAdminController
    {
        private readonly DvdDbContext _context;

        public ReportsController(DvdDbContext context)
        {
            _context = context;
        }

        // GET: Reports/Dashboard
        public IActionResult Dashboard()
        {
            var viewModel = new DashboardViewModel
            {
                SalesReport = GenerateSalesReport(),
                TrendingProducts = GetTrendingProducts(),
                BestSellers = GetBestSellers(),
                RevenueAnalysis = GetRevenueAnalysis(),
                CustomerStats = GetCustomerStats()
            };

            return View(viewModel);
        }

        // GET: Reports/Sales
        public IActionResult Sales(string period = "monthly")
        {
            var salesReport = GenerateSalesReport(); // This returns SalesReport
            ViewBag.Period = period;
            return View(salesReport); // Pass SalesReport, not DashboardViewModel
        }

        // GET: Reports/Products
        public IActionResult Products()
        {
            var productsReport = GetProductAnalytics();
            return View(productsReport);
        }

        // GET: Reports/Categories
        public IActionResult Categories()
        {
            var categoriesReport = GetCategoryAnalytics();
            return View(categoriesReport);
        }

        // GET: Reports/Customers
        public IActionResult Customers()
        {
            var customersReport = GetCustomerAnalytics();
            return View(customersReport);
        }

        // AJAX: Get sales data for charts
        [HttpGet]
        public IActionResult GetSalesData(string period)
        {
            var data = GetSalesChartData(period);
            return Json(data);
        }

        private SalesReport GenerateSalesReport()
        {
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var startOfYear = new DateTime(currentDate.Year, 1, 1);

            var monthlyOrders = _context.tbl_Orders
                .Where(o => o.OrderDate >= startOfMonth && o.Status != "Cancelled")
                .ToList();

            var yearlyOrders = _context.tbl_Orders
                .Where(o => o.OrderDate >= startOfYear && o.Status != "Cancelled")
                .ToList();

            var report = new SalesReport
            {
                Period = $"{currentDate:MMMM yyyy}",
                TotalSales = monthlyOrders.Sum(o => o.TotalAmount),
                TotalOrders = monthlyOrders.Count,
                AverageOrderValue = monthlyOrders.Any() ? monthlyOrders.Average(o => o.TotalAmount) : 0,
                MonthlyData = GetMonthlySalesData(),
                ProductSales = GetProductSalesData(),
                CategorySales = GetCategorySalesData()
            };

            // Set top product and category
            if (report.ProductSales.Any())
            {
                report.TopProduct = report.ProductSales.OrderByDescending(p => p.Revenue).First().ProductName;
            }

            if (report.CategorySales.Any())
            {
                report.TopCategory = report.CategorySales.OrderByDescending(c => c.Revenue).First().CategoryName;
            }

            return report;
        }

        private List<MonthlySales> GetMonthlySalesData()
        {
            var currentYear = DateTime.Now.Year;
            var salesData = new List<MonthlySales>();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(currentYear, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var monthlyOrders = _context.tbl_Orders
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status != "Cancelled")
                    .ToList();

                salesData.Add(new MonthlySales
                {
                    Month = startDate.ToString("MMM"),
                    Sales = monthlyOrders.Sum(o => o.TotalAmount),
                    Orders = monthlyOrders.Count
                });
            }

            return salesData;
        }

        private List<ProductSales> GetProductSalesData()
        {
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

            var productSales = _context.tbl_OrderDetails
                .Include(od => od.tbl_Orders)
                .Include(od => od.tbl_Products)
                    .ThenInclude(p => p.tbl_Albums)
                .Where(od => od.tbl_Orders.OrderDate >= startOfMonth && od.tbl_Orders.Status != "Cancelled")
                .GroupBy(od => new { od.ProductID, od.tbl_Products.tbl_Albums.Title })
                .Select(g => new ProductSales
                {
                    ProductName = g.Key.Title,
                    UnitsSold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.Quantity * od.UnitPrice),
                    Category = g.First().tbl_Products.tbl_Albums.tbl_Category.CategoryName
                })
                .OrderByDescending(ps => ps.Revenue)
                .Take(10)
                .ToList();

            return productSales;
        }

        private List<CategorySales> GetCategorySalesData()
        {
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

            var categorySales = _context.tbl_OrderDetails
                .Include(od => od.tbl_Orders)
                .Include(od => od.tbl_Products)
                    .ThenInclude(p => p.tbl_Albums)
                        .ThenInclude(a => a.tbl_Category)
                .Where(od => od.tbl_Orders.OrderDate >= startOfMonth && od.tbl_Orders.Status != "Cancelled")
                .GroupBy(od => od.tbl_Products.tbl_Albums.tbl_Category.CategoryName)
                .Select(g => new CategorySales
                {
                    CategoryName = g.Key,
                    UnitsSold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.Quantity * od.UnitPrice)
                })
                .OrderByDescending(cs => cs.Revenue)
                .ToList();

            return categorySales;
        }

        private List<TrendingProduct> GetTrendingProducts()
        {
            // This would ideally come from analytics data
            // For now, we'll use order data as a proxy
            var trendingProducts = _context.tbl_OrderDetails
                .Include(od => od.tbl_Products)
                    .ThenInclude(p => p.tbl_Albums)
                .Include(od => od.tbl_Products)
                    .ThenInclude(p => p.tbl_Albums.tbl_Category)
                .Where(od => od.tbl_Orders.OrderDate >= DateTime.Now.AddDays(-30))
                .GroupBy(od => new { od.ProductID, od.tbl_Products.tbl_Albums.Title })
                .Select(g => new TrendingProduct
                {
                    ProductName = g.Key.Title,
                    Category = g.First().tbl_Products.tbl_Albums.tbl_Category.CategoryName,
                    AddedToCart = g.Sum(od => od.Quantity),
                    ConversionRate = 0.15m // Placeholder
                })
                .OrderByDescending(tp => tp.AddedToCart)
                .Take(5)
                .ToList();

            return trendingProducts;
        }

        private List<BestSeller> GetBestSellers()
        {
            var bestSellers = _context.tbl_OrderDetails
                .Include(od => od.tbl_Products)
                    .ThenInclude(p => p.tbl_Albums)
                .Include(od => od.tbl_Products)
                    .ThenInclude(p => p.tbl_Albums.tbl_Category)
                .Where(od => od.tbl_Orders.OrderDate >= DateTime.Now.AddMonths(-3))
                .GroupBy(od => new { od.ProductID, od.tbl_Products.tbl_Albums.Title })
                .Select(g => new BestSeller
                {
                    ProductName = g.Key.Title,
                    Category = g.First().tbl_Products.tbl_Albums.tbl_Category.CategoryName,
                    UnitsSold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.Quantity * od.UnitPrice)
                })
                .OrderByDescending(bs => bs.Revenue)
                .Take(10)
                .ToList();

            // Add ranks
            for (int i = 0; i < bestSellers.Count; i++)
            {
                bestSellers[i].Rank = i + 1;
            }

            return bestSellers;
        }

        private RevenueAnalysis GetRevenueAnalysis()
        {
            var currentDate = DateTime.Now;
            var analysis = new RevenueAnalysis
            {
                MonthlyRevenue = _context.tbl_Orders
                    .Where(o => o.OrderDate.Month == currentDate.Month &&
                               o.OrderDate.Year == currentDate.Year &&
                               o.Status != "Cancelled")
                    .Sum(o => o.TotalAmount),
                QuarterlyRevenue = _context.tbl_Orders
                    .Where(o => o.OrderDate >= currentDate.AddMonths(-3) &&
                               o.Status != "Cancelled")
                    .Sum(o => o.TotalAmount),
                YearlyRevenue = _context.tbl_Orders
                    .Where(o => o.OrderDate.Year == currentDate.Year &&
                               o.Status != "Cancelled")
                    .Sum(o => o.TotalAmount),
                RevenueTrends = GetRevenueTrends()
            };

            return analysis;
        }

        private List<RevenueTrend> GetRevenueTrends()
        {
            var trends = new List<RevenueTrend>();
            var currentDate = DateTime.Now;

            for (int i = 5; i >= 0; i--)
            {
                var date = currentDate.AddMonths(-i);
                var revenue = _context.tbl_Orders
                    .Where(o => o.OrderDate.Month == date.Month &&
                               o.OrderDate.Year == date.Year &&
                               o.Status != "Cancelled")
                    .Sum(o => o.TotalAmount);

                trends.Add(new RevenueTrend
                {
                    Period = date.ToString("MMM yyyy"),
                    Revenue = revenue
                });
            }

            return trends;
        }

        private CustomerStats GetCustomerStats()
        {
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

            var stats = new CustomerStats
            {
                TotalCustomers = _context.tbl_Users.Count(u => u.Role == "Customer"),
                NewCustomersThisMonth = _context.tbl_Users
                    .Count(u => u.Role == "Customer" && u.Created_At >= startOfMonth),
                ReturningCustomers = _context.tbl_Orders
                    .Where(o => o.OrderDate >= startOfMonth)
                    .Select(o => o.UserID)
                    .Distinct()
                    .Count()
            };

            return stats;
        }

        private object GetSalesChartData(string period)
        {
            // Implement chart data generation based on period
            return new { };
        }

        private object GetProductAnalytics()
        {
            // Implement product analytics
            return new { };
        }

        private object GetCategoryAnalytics()
        {
            // Implement category analytics
            return new { };
        }

        private object GetCustomerAnalytics()
        {
            // Implement customer analytics
            return new { };
        }
    }
}