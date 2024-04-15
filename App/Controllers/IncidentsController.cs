using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using App.Models;
using App.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;

namespace App.Controllers
{
    [Authorize] // Ensures only authenticated users can report incidents
    public class IncidentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IncidentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Incidents/Report
        public IActionResult Report()
        {
            // Pass a new view model to the view
            var viewModel = new EnvironmentalIncident();
            return View(viewModel);
        }

        // POST: Incidents/Report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(EnvironmentalIncident viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var incident = new EnvironmentalIncident
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Location = viewModel.Location,
                    Type = viewModel.Type, // No need to unwrap
                    DateReported = DateTime.UtcNow,
                    ReporterId = user.Id,
                    Status = IncidentStatus.Reported // Default status
                };

                _context.Add(incident);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ThankYou)); // Redirect to a thank you page
            }

            return View(viewModel); // If invalid, return to the form with the same view model
        }

        // GET: Incidents/ThankYou
        public IActionResult ThankYou()
        {
            return View();
        }

        // Other methods...
    }
}
