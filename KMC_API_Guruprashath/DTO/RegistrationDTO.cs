namespace KMC_API_Guruprashath.DTO
{
    public class RegistrationWriteDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? NIC { get; set; }
        public string? PhotoUrl { get; set; }
        public int EventId { get; set; }
    }

    public class RegistrationReadDTO
    {
        public int RegistrationId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? NIC { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime RegisteredOn { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public int UserId { get; set; }
    }
}
