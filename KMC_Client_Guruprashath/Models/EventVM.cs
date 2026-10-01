namespace KMC_Client_Guruprashath.Models
{
    public class EventVM
    {
        public int EventId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Venue { get; set; } = "";
        public DateTime EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; } = "";
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public int RegisteredCount { get; set; }
        public int SeatsLeft { get; set; }
        public int? OrganizerId { get; set; }
        public string OrganizerName { get; set; } = "";
        public string ApprovalStatus { get; set; } = "Approved"; // Pending | Approved | Rejected
    }

    public class EventWriteVM
    {
        public int EventId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Venue { get; set; } = "";
        public DateTime EventDate { get; set; } = DateTime.Today.AddDays(7);
        public TimeSpan StartTime { get; set; }
        public int Capacity { get; set; } = 100;
        public string? ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; } = true;
        public int CategoryId { get; set; }
        public List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();
    }
}
