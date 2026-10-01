namespace KMC_API_Guruprashath.Model
{
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IconClass { get; set; }

        public List<Event> Events { get; set; } = new List<Event>();
    }
}
