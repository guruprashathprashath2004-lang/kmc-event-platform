namespace KMC_API_Guruprashath.Model
{
    using System.ComponentModel.DataAnnotations;

    // Role is one of: "Citizen", "Organizer", "Admin".
    // Citizens need an account only to register for events.
    // Organizers need an account to create/manage their own events (subject to Admin approval).
    // Admin is seeded directly and oversees/approves everything.
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        [Required]
        public string Role { get; set; } = "Citizen";
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public List<Event> OrganizedEvents { get; set; } = new List<Event>();
        public List<Registration> Registrations { get; set; } = new List<Registration>();
    }
}
