using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class SalesReport
    {
        [Display(Name = "Report Period")]
        public string Period { get; set; }

        [Display(Name = "Total Sales")]
        [DataType(DataType.Currency)]
        public decimal TotalSales { get; set; }

        [Display(Name = "Total Orders")]
        public int TotalOrders { get; set; }

        [Display(Name = "Average Order Value")]
        [DataType(DataType.Currency)]
        public decimal AverageOrderValue { get; set; }

        [Display(Name = "Top Selling Product")]
        public string TopProduct { get; set; }

        [Display(Name = "Top Category")]
        public string TopCategory { get; set; }

        public List<MonthlySales> MonthlyData { get; set; }
        public List<ProductSales> ProductSales { get; set; }
        public List<CategorySales> CategorySales { get; set; }
    }

    public class MonthlySales
    {
        public string Month { get; set; }
        public decimal Sales { get; set; }
        public int Orders { get; set; }
    }

    public class ProductSales
    {
        public string ProductName { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
        public string Category { get; set; }
    }

    public class CategorySales
    {
        public string CategoryName { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }
}