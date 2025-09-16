using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DurbanMunicipalApp.Data;
using DurbanMunicipalApp.Models;
using System.ComponentModel.DataAnnotations;

namespace DurbanMunicipalApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var reports = await _context.Reports.OrderByDescending(r => r.SubmittedDate).ToListAsync();
            return View(reports);
        }

        // GET: Reports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProblemType,Description,Location,Latitude,Longitude,PhoneNumber")] Report report, IFormFile? picture)
        {
            Console.WriteLine("=== CREATE POST METHOD CALLED ===");
            Console.WriteLine($"ProblemType: {report.ProblemType}");
            Console.WriteLine($"Description: {report.Description}");
            Console.WriteLine($"PhoneNumber: {report.PhoneNumber}");
            Console.WriteLine($"Picture file: {(picture != null ? picture.FileName : "null")}\n");

            // Fix locale-related parsing for Latitude/Longitude (use invariant culture)
            var latRaw = Request.Form["Latitude"].ToString();
            var lngRaw = Request.Form["Longitude"].ToString();
            if (!string.IsNullOrWhiteSpace(latRaw) || !string.IsNullOrWhiteSpace(lngRaw))
            {
                var invariant = System.Globalization.CultureInfo.InvariantCulture;
                if (!string.IsNullOrWhiteSpace(latRaw) && double.TryParse(latRaw, System.Globalization.NumberStyles.Float, invariant, out var latValue))
                {
                    report.Latitude = latValue;
                    ModelState.Remove(nameof(report.Latitude));
                }
                if (!string.IsNullOrWhiteSpace(lngRaw) && double.TryParse(lngRaw, System.Globalization.NumberStyles.Float, invariant, out var lngValue))
                {
                    report.Longitude = lngValue;
                    ModelState.Remove(nameof(report.Longitude));
                }
                // Re-validate after fixing values
                TryValidateModel(report);
            }

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var err in kv.Value.Errors)
                    {
                        Console.WriteLine($"ModelState error on '{kv.Key}': {err.ErrorMessage}");
                    }
                }
                // Return view so user can fix errors (file input will reset per browser security)
                return View(report);
            }

            // Generate reference number and set timestamps
            report.ReferenceNumber = GenerateReferenceNumber();
            report.SubmittedDate = DateTime.Now;
            report.LastUpdated = DateTime.Now;

            // Handle picture upload
            if (picture != null && picture.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{report.ReferenceNumber}_{Guid.NewGuid()}{Path.GetExtension(picture.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await picture.CopyToAsync(stream);
                }

                report.PicturePath = $"/uploads/{fileName}";
            }

            _context.Add(report);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(report.PhoneNumber))
            {
                await SendNotificationsAsync(report);
            }

            return RedirectToAction(nameof(Details), new { id = report.Id });
        }

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports.FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        private string GenerateReferenceNumber()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"RPT-{timestamp}-{random}";
        }

        private Task SendNotificationsAsync(Report report)
        {
            try
            {
                var message = $"Your report has been submitted successfully!\n\nReference Number: {report.ReferenceNumber}\nProblem Type: {report.ProblemType}\nSubmitted: {report.SubmittedDate:MMM dd, yyyy 'at' h:mm tt}\n\nTrack your report at: {Request.Scheme}://{Request.Host}/Reports/Details/{report.Id}";
                
                // Generate WhatsApp URL
                var whatsappUrl = $"https://wa.me/{CleanPhoneNumber(report.PhoneNumber)}?text={Uri.EscapeDataString(message)}";
                
                // Store notification URLs in TempData for display
                TempData["WhatsAppUrl"] = whatsappUrl;
                TempData["SMSMessage"] = message;
                TempData["PhoneNumber"] = report.PhoneNumber;
            }
            catch (Exception ex)
            {
                // Log error but don't fail the report submission
                Console.WriteLine($"Error sending notifications: {ex.Message}");
            }
            
            return Task.CompletedTask;
        }

        private string CleanPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return string.Empty;
                
            // Remove all non-digit characters
            var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());
            
            // Add country code if not present (assuming +1 for US/Canada)
            if (cleaned.Length == 10)
            {
                cleaned = "1" + cleaned;
            }
            
            return cleaned;
        }
    }
}
