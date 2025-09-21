using System.Diagnostics;
using DurbanMunicipalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DurbanMunicipalApp.Data;

namespace DurbanMunicipalApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // API endpoint for fetching reports data
        public async Task<IActionResult> GetReportsData()
        {
            try
            {
                var reports = await _context.Reports
                    .Select(r => new {
                        r.Id,
                        ProblemType = r.ProblemType.ToString(), // convert enum to string
                        r.Description,
                        r.Location,
                        r.Latitude,
                        r.Longitude,
                        Status = r.Status.ToString(),
                        r.SubmittedDate
                    })
                    .ToListAsync();

                return Json(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reports data");
                return Json(new { error = "Error loading data: " + ex.Message });
            }
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
    }
}