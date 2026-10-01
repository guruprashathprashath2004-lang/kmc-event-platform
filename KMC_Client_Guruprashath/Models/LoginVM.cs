using System.ComponentModel.DataAnnotations;

namespace KMC_Client_Guruprashath.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public string? ErrorMessage { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
