namespace KMC_Client_Guruprashath.Models
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string IconClass { get; set; } = "bi-calendar-event";
        public int EventCount { get; set; }
    }

    public class CategoryWriteVM
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string IconClass { get; set; } = "";
    }
}
