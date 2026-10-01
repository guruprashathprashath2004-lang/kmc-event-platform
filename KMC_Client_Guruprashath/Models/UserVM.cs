namespace KMC_Client_Guruprashath.Models
{
    public class UserVM
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime CreatedOn { get; set; }
        public int EventCount { get; set; }
        public int RegistrationCount { get; set; }
    }
}
