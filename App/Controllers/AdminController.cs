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


    // Other methods...
}


