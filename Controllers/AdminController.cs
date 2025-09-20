using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DurbanMunicipalApp.Data;
using DurbanMunicipalApp.Models;
using BCrypt.Net;
using DurbanMunicipalApp.ViewModels;

namespace DurbanMunicipalApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddTeam()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeam(TeamRegistrationViewModel viewmodel)
        {
            if (!ModelState.IsValid)
                return View(viewmodel);

            var existingUser = await _db.UserProfiles.FirstOrDefaultAsync(u => u.Email == viewmodel.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(viewmodel);
            }

            var userProfile = new UserProfile
            {
                Email = viewmodel.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(viewmodel.Password),
                IsActive = true,
                UserType = "Team"
            };

            _db.UserProfiles.Add(userProfile);
            await _db.SaveChangesAsync();

            var team = new Team
            {
                UserId = userProfile.UserId,
                StaffName = viewmodel.StaffName,
                StaffRole = viewmodel.StaffRole,
                PhoneNumber = viewmodel.PhoneNumber
            };

            _db.Teams.Add(team);
            await _db.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }
    }
}
