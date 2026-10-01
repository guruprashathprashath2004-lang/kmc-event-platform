namespace KMC_API_Guruprashath.DTO
{
    public class CategoryWriteDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IconClass { get; set; }
    }

    public class CategoryReadDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IconClass { get; set; }
        public int EventCount { get; set; }
    }
}
