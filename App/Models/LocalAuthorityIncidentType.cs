using App.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class LocalAuthorityIncidentType
{
    [Key, ForeignKey("User")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    // Use the enum directly, and EF Core will store it as a string in the database
    public string IncidentTypeId { get; set; }

}
