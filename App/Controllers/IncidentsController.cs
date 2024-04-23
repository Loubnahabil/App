using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using App.Models;
using App.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.EntityFrameworkCore;

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

        public ActionResult MyIncidents()
        {
            var userId = _userManager.GetUserId(User); // Get the ID of the current user.
            var myIncidents = _context.EnvironmentalIncidents.Where(i => i.ReporterId == userId).ToList(); // Fetch incidents from the database.
            return View(myIncidents); // Pass the incidents to the view.
        }

        //EDIT AND DELETE

        // GET: Incidents/Edit/5
        // GET: Incidents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incident = await _context.EnvironmentalIncidents.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            // Debugging statements to inspect values
            Console.WriteLine("ReporterId of incident: " + incident.ReporterId);
            Console.WriteLine("UserId of current user: " + _userManager.GetUserId(User));

            if (incident.ReporterId != _userManager.GetUserId(User))
            {
                return Forbid(); // Prevent editing incidents by other users
            }

            return View(incident);
        }


        // POST: Incidents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IncidentId,Title,Description,Location,Type,DateReported,Status,ReporterId")] EnvironmentalIncident incident)
        {
            if (id != incident.IncidentId)
            {
                return NotFound();
            }

            if (incident.ReporterId != _userManager.GetUserId(User))
            {
                return Forbid(); // Prevent editing incidents by other users
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incident);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncidentExists(incident.IncidentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyIncidents));
            }
            return View(incident);
        }

        private bool IncidentExists(int id)
        {
            return _context.EnvironmentalIncidents.Any(e => e.IncidentId == id);
        }


        // POST: Incidents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incident = await _context.EnvironmentalIncidents.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            if (incident.ReporterId != _userManager.GetUserId(User))
            {
                return Forbid(); // Prevent deleting incidents by other users
            }

            _context.EnvironmentalIncidents.Remove(incident);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyIncidents));
        }


    }





}
