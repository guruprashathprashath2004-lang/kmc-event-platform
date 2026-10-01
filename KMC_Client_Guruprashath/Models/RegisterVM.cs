using System.ComponentModel.DataAnnotations;

namespace KMC_Client_Guruprashath.Models
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Please choose a username.")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please enter your full name.")]
        public string FullName { get; set; } = "";

        public string? Phone { get; set; }

        [Required]
        public string Role { get; set; } = "Citizen"; // "Citizen" or "Organizer"

        // Where to send the user back to once they've signed up (e.g. the event they wanted to register for).
        public string? ReturnUrl { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
