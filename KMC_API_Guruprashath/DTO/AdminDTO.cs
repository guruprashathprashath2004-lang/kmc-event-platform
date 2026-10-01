namespace KMC_API_Guruprashath.DTO
{
    public class LoginRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
    }

    public class RegisterRequestDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        // Only "Citizen" or "Organizer" are accepted here - Admin accounts are never self-registered.
        public string Role { get; set; }
    }
}
