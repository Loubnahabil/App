using App.Data;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;


[Authorize(Roles = "Admin")] // Ensures only users in the Admin role can access these methods
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/ManageIncidents
    public async Task<IActionResult> ManageIncidents()
    {
        return View(await _context.EnvironmentalIncidents.ToListAsync());
    }

    // GET: Admin/ManageReport/{id}
    public async Task<IActionResult> ManageReport(int id)
    {
        var incident = await _context.EnvironmentalIncidents
                            .FirstOrDefaultAsync(m => m.IncidentId == id);

        if (incident == null)
        {
            return NotFound();
        }

        return View(incident);
    }


    // GET: Admin/EditIncident/5
    public async Task<IActionResult> EditIncident(int? id)
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
        return View(incident);
    }

    // POST: Admin/EditIncident/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditIncident(int id, [Bind("IncidentId,Title,Description,Location,Type,Status")] EnvironmentalIncident incident)
    {
        if (id != incident.IncidentId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(incident);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageIncidents));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.EnvironmentalIncidents.Any(e => e.IncidentId == incident.IncidentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        return View(incident);
    }

    // GET: Admin/DeleteIncident/5
    public async Task<IActionResult> DeleteIncident(int? id)
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

        return View(incident);
    }

    // POST: Admin/DeleteIncident/5
    [HttpPost, ActionName("DeleteIncident")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var incident = await _context.EnvironmentalIncidents.FindAsync(id);
        _context.EnvironmentalIncidents.Remove(incident);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageIncidents));
    }


}


