using App.Data;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq; // For LINQ queries
using Microsoft.EntityFrameworkCore; // For Entity Framework functionality
using System.Threading.Tasks; // For async method

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context; // Assuming you have an ApplicationDbContext

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new StatisticsViewModel
        {
            TotalIncidentsReported = await _context.EnvironmentalIncidents.CountAsync(),
            IncidentsResolved = await _context.EnvironmentalIncidents.CountAsync(i => i.Status == IncidentStatus.Resolved),
            ActiveIncidents = await _context.EnvironmentalIncidents.CountAsync(i => i.Status == IncidentStatus.UnderInvestigation)
        };

        return View(model);
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
