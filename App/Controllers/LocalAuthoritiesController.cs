using App.Data;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Authorize(Roles = "LocalAuthority")]
public class LocalAuthoritiesController : Controller
{
    private readonly ApplicationDbContext _context;

    public LocalAuthoritiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: LocalAuthorities/ViewReports
    public async Task<IActionResult> ViewReports(string authorityType)
    {
        IncidentType type;
        if (!Enum.TryParse(authorityType, out type))
        {
            return BadRequest("Invalid incident type.");
        }

        var incidents = await _context.EnvironmentalIncidents
                            .Where(incident => incident.Type == type)
                            .ToListAsync();
        return View(incidents);
    }

    // GET: LocalAuthorities/ManageReport/{id}
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
