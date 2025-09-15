namespace DvdStore.Models
{
    public class DashboardViewModel
    {
        public SalesReport SalesReport { get; set; }
        public List<TrendingProduct> TrendingProducts { get; set; }
        public List<BestSeller> BestSellers { get; set; }
        public RevenueAnalysis RevenueAnalysis { get; set; }
        public CustomerStats CustomerStats { get; set; }
    }

    public class TrendingProduct
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int Views { get; set; }
        public int AddedToCart { get; set; }
        public decimal ConversionRate { get; set; }
    }

    public class BestSeller
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
        public int Rank { get; set; }
    }

    public class RevenueAnalysis
    {
        public decimal MonthlyRevenue { get; set; }
        public decimal QuarterlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        public decimal RevenueGrowth { get; set; }
        public List<RevenueTrend> RevenueTrends { get; set; }
    }

    public class RevenueTrend
    {
        public string Period { get; set; }
        public decimal Revenue { get; set; }
    }

    public class CustomerStats
    {
        public int TotalCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int ReturningCustomers { get; set; }
        public decimal AverageCustomerValue { get; set; }
    }
}