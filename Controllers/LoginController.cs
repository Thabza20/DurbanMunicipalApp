using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DurbanMunicipalApp.Data;
using DurbanMunicipalApp.Models;
using BCrypt.Net;
using DurbanMunicipalApp.ViewModels;

namespace DurbanMunicipalApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LoginController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserProfile objUser)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Invalid input.";
                return View("Login", objUser);
            }

            var obj = await _db.UserProfiles.FirstOrDefaultAsync(a => a.Email == objUser.Email);
            if (obj == null)
            {
                ViewBag.Message = "User not found.";
                return View("Login", objUser);
            }

            if (!BCrypt.Net.BCrypt.Verify(objUser.Password, obj.Password))
            {
                ViewBag.Message = "Incorrect password.";
                return View("Login", objUser);
            }

            HttpContext.Session.SetString("UserId", obj.UserId.ToString());
            HttpContext.Session.SetString("Email", obj.Email);
            HttpContext.Session.SetString("UserType", obj.UserType ?? "");

            switch (obj.UserType?.ToLower())
            {
                case "admin":
                    return RedirectToAction("Dashboard", "Admin");

                case "customer":
                    return await HandleCustomerLogin(obj);

                case "team":
                    return await HandleTeamLogin(obj);

                default:
                    ViewBag.Message = "Invalid user type.";
                    return View("Login", objUser);
            }
        }

        private async Task<IActionResult> HandleCustomerLogin(UserProfile user)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.UserId == user.UserId);
            if (customer != null)
            {
                HttpContext.Session.SetString("CustomerName", $"{customer.CustomerFirstName} {customer.CustomerLastName}");
                return RedirectToAction("CustomerDashboard", "Customer");
            }

            ViewBag.Message = "Customer profile not found.";
            return View("Login");
        }

        private async Task<IActionResult> HandleTeamLogin(UserProfile user)
        {
            var team = await _db.Teams.FirstOrDefaultAsync(t => t.UserId == user.UserId);
            if (team != null)
            {
                HttpContext.Session.SetString("StaffName", team.StaffName);
                HttpContext.Session.SetString("StaffRole", team.StaffRole);
                return RedirectToAction("Dashboard", "Team");
            }

            ViewBag.Message = "Team profile not found.";
            return View("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CustomerRegistrationViewModel viewmodel)
        {
            if (!ModelState.IsValid)
                return View(viewmodel);

            var existingUser = await _db.UserProfiles.FirstOrDefaultAsync(u => u.Email == viewmodel.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return View(viewmodel);
            }

            if (viewmodel.Password != viewmodel.ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Passwords do not match.");
                return View(viewmodel);
            }

            var userProfile = new UserProfile
            {
                Email = viewmodel.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(viewmodel.Password),
                IsActive = true,
                UserType = "Customer"
            };

            _db.UserProfiles.Add(userProfile);
            await _db.SaveChangesAsync();

            var customer = new Customer
            {
                UserId = userProfile.UserId,
                CustomerId = Guid.NewGuid().ToString(), // Auto-generated ID
                CustomerFirstName = viewmodel.CustomerFirstName,
                CustomerLastName = viewmodel.CustomerLastName
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();


            HttpContext.Session.SetString("UserId", userProfile.UserId.ToString());
            HttpContext.Session.SetString("Email", userProfile.Email);
            HttpContext.Session.SetString("UserType", "Customer");

            return RedirectToAction("CustomerDashboard", "Customer");
        }
    }
}
