using DvdStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DvdStore.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DvdDbContext _context;
        public DashboardController(DvdDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.tbl_Users.CountAsync();
            ViewBag.TotalOrders = await _context.tbl_Orders.CountAsync();
            ViewBag.TotalProducts = await _context.tbl_Products.CountAsync();
            ViewBag.TotalFeedbacks = await _context.tbl_Feedbacks.CountAsync();

            return View();
        }
    }
}
