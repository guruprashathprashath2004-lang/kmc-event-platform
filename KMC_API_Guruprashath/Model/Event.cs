namespace KMC_API_Guruprashath.Model
{
    using System.ComponentModel.DataAnnotations;

    public class Event
    {
        [Key]
        public int EventId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string? Venue { get; set; }
        [Required]
        public DateTime EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public int Capacity { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Null OrganizerId = created directly by Admin. Otherwise, an Organizer's own event.
        public int? OrganizerId { get; set; }
        public User OrganizerUser { get; set; }

        // "Pending" (awaiting Admin approval), "Approved", or "Rejected".
        // Admin-created events are auto-approved; Organizer-created events start Pending.
        public string ApprovalStatus { get; set; } = "Approved";

        public int CategoryId { get; set; }
        public Category EventCategory { get; set; }

        public List<Registration> Registrations { get; set; } = new List<Registration>();
    }
}
