using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DvdStore.Models;

namespace DvdStore.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly DvdDbContext _context;

        public FeedbackController(DvdDbContext context)
        {
            _context = context;
        }

        // GET: Feedback/Create
        public IActionResult Create()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Create(Feedback feedback)
        //{
        //    Console.WriteLine("=== FEEDBACK FORM SUBMISSION ===");

        //    // Log all model state errors
        //    if (!ModelState.IsValid)
        //    {
        //        Console.WriteLine("MODEL VALIDATION FAILED:");
        //        foreach (var key in ModelState.Keys)
        //        {
        //            var state = ModelState[key];
        //            if (state.Errors.Count > 0)
        //            {
        //                Console.WriteLine($"- {key}: {string.Join(", ", state.Errors.Select(e => e.ErrorMessage))}");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Model validation passed");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var userId = HttpContext.Session.GetInt32("UserId");
        //        if (!userId.HasValue)
        //        {
        //            TempData["Error"] = "Please login to submit feedback";
        //            return RedirectToAction("Login", "Auth");
        //        }

        //        feedback.UserID = userId.Value;
        //        feedback.SubmittedDate = DateTime.Now;
        //        feedback.Status = "New";

        //        _context.tbl_Feedbacks.Add(feedback);
        //        _context.SaveChanges();

        //        TempData["Success"] = "Thank you for your feedback!";
        //        return RedirectToAction("Index", "Home");
        //    }

        //    // If we get here, validation failed - return to form with errors
        //    return View(feedback);
        //}



        [HttpPost]
        public IActionResult Create(Feedback feedback)
        {
            // TEMPORARY: Skip validation for testing
            Console.WriteLine("BYPASSING VALIDATION FOR TESTING");

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "Please login to submit feedback";
                return RedirectToAction("Login", "Auth");
            }

            feedback.UserID = userId.Value;
            feedback.SubmittedDate = DateTime.Now;
            feedback.Status = "New";

            _context.tbl_Feedbacks.Add(feedback);
            _context.SaveChanges();

            TempData["Success"] = "Thank you for your feedback!";
            return RedirectToAction("Index", "Home");
        }



        // GET: Feedback/Index (Admin only)
        public IActionResult Index()
        {
            // FIXED: Use HttpContext instead of Context
            if (!AuthorizationHelper.IsAdmin(HttpContext))
                return RedirectToAction("Login", "Auth");

            var feedbacks = _context.tbl_Feedbacks
                .Include(f => f.User)
                .OrderByDescending(f => f.SubmittedDate)
                .ToList();

            return View(feedbacks);
        }

        // POST: Feedback/UpdateStatus
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            // FIXED: Use HttpContext instead of Context
            if (!AuthorizationHelper.IsAdmin(HttpContext))
                return RedirectToAction("Login", "Auth");

            var feedback = _context.tbl_Feedbacks.Find(id);
            if (feedback != null)
            {
                feedback.Status = status;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //  - this method is for testing
        public IActionResult TestDatabase()
        {
            try
            {
                var canConnect = _context.Database.CanConnect();
                var feedbackCount = _context.tbl_Feedbacks.Count();

                return Content($"Database connected: {canConnect}, Feedback records: {feedbackCount}");
            }
            catch (Exception ex)
            {
                return Content($"Database error: {ex.Message}");
            }
        }


        // In FeedbackController.cs
        public IActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Test(string testField)
        {
            Console.WriteLine($"Test received: {testField}");
            return Content($"Server received: {testField}");
        }
    }
}