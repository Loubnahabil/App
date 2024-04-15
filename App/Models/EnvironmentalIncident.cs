using App.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class EnvironmentalIncident
    {
        [Key]
        public int IncidentId { get; set; }

        [Required, MaxLength(256)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required, MaxLength(256)]
        public string? Location { get; set; } // Location can be null

        [Required]
        public IncidentType? Type { get; set; } // Assuming this should be non-nullable

        [Required]
        public DateTime DateReported { get; set; }

        [Required]
        public IncidentStatus Status { get; set; }

        // Foreign key to ApplicationUser
        [ForeignKey("Reporter")]
        public string? ReporterId { get; set; } // ReporterId can be null

        // Navigation property to the ApplicationUser who reported the incident
        public virtual ApplicationUser? Reporter { get; set; } // Reporter can be null
    }

    public enum IncidentStatus
    {
        Reported,
        UnderInvestigation,
        Resolved,
        Closed
    }

    public enum IncidentType
    {
        WaterPollution,
        AirPollution,
        WildlifeDisturbance,
        Deforestation,
        IllegalDumping
    }
}
