using App.Data;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "LocalAuthority")]
public class LocalAuthoritiesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LocalAuthoritiesController> _logger;

    public LocalAuthoritiesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<LocalAuthoritiesController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    private async Task<IncidentType?> GetCurrentAuthorityIncidentType()
    {
        var userId = _userManager.GetUserId(User);
        var authorityIncidentTypeString = await _context.LocalAuthorityIncidentTypes
            .Where(lait => lait.UserId == userId)
            .Select(lait => lait.IncidentTypeId)
            .FirstOrDefaultAsync();

        if (Enum.TryParse<IncidentType>(authorityIncidentTypeString, out var authorityIncidentType))
        {
            return authorityIncidentType;
        }

        return null; // Handle no match or parsing failure
    }

    // GET: LocalAuthorities/Index
    public async Task<IActionResult> Index()
    {
        try
        {
            var incidentType = await GetCurrentAuthorityIncidentType();
            if (!incidentType.HasValue)
            {
                _logger.LogWarning("No incident type value found for the current user.");
                return RedirectToAction("Error", new { message = "No incident type associated with this authority." });
            }

            var incidents = await _context.EnvironmentalIncidents
                .Where(i => i.Type == incidentType.Value)
                .ToListAsync();

            ViewData["AuthorityType"] = incidentType.Value.ToString();
            return View(incidents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Index method for LocalAuthoritiesController.");
            return View("Error", new { message = "An error occurred while processing your request." });
        }
    }

    // GET: LocalAuthorities/ViewReports
    public async Task<IActionResult> ViewReports()
    {
        var incidentType = await GetCurrentAuthorityIncidentType();
        if (!incidentType.HasValue)
        {
            return RedirectToAction("Error", new { message = "No incident type associated with this authority." });
        }

        var incidents = await _context.EnvironmentalIncidents
            .Where(i => i.Type == incidentType.Value)
            .ToListAsync();

        return View(incidents);
    }

    // GET: LocalAuthorities/EditReport/{id}
    public async Task<IActionResult> EditReport(int? id)
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

        if (!(await IsUserAuthorizedForIncident(incident)))
        {
            return Forbid();
        }

        return View(incident);
    }

    // POST: LocalAuthorities/EditReport/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditReport(int id, [Bind("IncidentId,Title,Description,Location,Type,DateReported,Status,ReporterId")] EnvironmentalIncident incident)
    {
        if (id != incident.IncidentId)
        {
            return NotFound();
        }

        if (!(await IsUserAuthorizedForIncident(incident)))
        {
            return Forbid();
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
                if (!EnvironmentalIncidentExists(incident.IncidentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(ViewReports));
        }
        return View(incident);
    }

    // GET: LocalAuthorities/DeleteReport/{id}
    public async Task<IActionResult> DeleteReport(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var incident = await _context.EnvironmentalIncidents
            .FirstOrDefaultAsync(m => m.IncidentId == id);
        if (incident == null)
        {
            return NotFound();
        }

        if (!(await IsUserAuthorizedForIncident(incident)))
        {
            return Forbid();
        }

        return View(incident);
    }

    // POST: LocalAuthorities/DeleteReport/{id}
    [HttpPost, ActionName("DeleteReport")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReportConfirmed(int id)
    {
        var incident = await _context.EnvironmentalIncidents.FindAsync(id);
        if (incident == null || !(await IsUserAuthorizedForIncident(incident)))
        {
            return Forbid();
        }

        _context.EnvironmentalIncidents.Remove(incident);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ViewReports));
    }

    private bool EnvironmentalIncidentExists(int id)
    {
        return _context.EnvironmentalIncidents.Any(e => e.IncidentId == id);
    }

    private async Task<bool> IsUserAuthorizedForIncident(EnvironmentalIncident incident)
    {
        var currentAuthorityType = await GetCurrentAuthorityIncidentType();
        return incident.Type == currentAuthorityType;
    }
}
