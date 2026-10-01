namespace KMC_API_Guruprashath.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Registration
    {
        [Key]
        public int RegistrationId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? NIC { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime RegisteredOn { get; set; } = DateTime.Now;

        [ForeignKey("RegisteredEvent")]
        public int EventId { get; set; }
        public Event RegisteredEvent { get; set; }

        // Registering for an event now requires a logged-in Citizen account.
        [ForeignKey("RegisteredByUser")]
        public int UserId { get; set; }
        public User RegisteredByUser { get; set; }
    }
}
