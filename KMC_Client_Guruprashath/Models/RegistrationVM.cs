using System.ComponentModel.DataAnnotations;

namespace KMC_Client_Guruprashath.Models
{
    public class RegistrationVM
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please enter your phone number.")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Please enter your NIC number.")]
        public string NIC { get; set; } = "";

        public string? PhotoUrl { get; set; }

        public int EventId { get; set; }

        // Populated to redisplay the event summary alongside the form / confirmation.
        public EventVM? Event { get; set; }

        // Used when listing a citizen's own past registrations ("My Registrations").
        public int RegistrationId { get; set; }
        public DateTime RegisteredOn { get; set; }
    }
}
