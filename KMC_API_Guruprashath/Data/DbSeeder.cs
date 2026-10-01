namespace KMC_API_Guruprashath.Data
{
    using KMC_API_Guruprashath.Model;
    using System.Security.Cryptography;
    using System.Text;

    public static class DbSeeder
    {
        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static void Seed(AppDBContext db)
        {
            db.Database.EnsureCreated();

            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User
                    {
                        Username = "kmc.admin",
                        Email = "admin@kmc.lk.gov",
                        PasswordHash = HashPassword("Kmc@2026"),
                        FullName = "KMC Platform Administrator",
                        Phone = "0812223344",
                        Role = "Admin"
                    },
                    new User
                    {
                        Username = "organizer.demo",
                        Email = "organizer@kmc.lk.gov",
                        PasswordHash = HashPassword("Organizer@2026"),
                        FullName = "Demo Event Organizer",
                        Phone = "0771234567",
                        Role = "Organizer"
                    },
                    new User
                    {
                        Username = "citizen.demo",
                        Email = "citizen@kmc.lk.gov",
                        PasswordHash = HashPassword("Citizen@2026"),
                        FullName = "Demo Citizen",
                        Phone = "0779876543",
                        Role = "Citizen"
                    }
                );
                db.SaveChanges();
            }

            if (!db.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Cultural", Description = "Perahera, art & heritage events across Kandy", IconClass = "bi-flower1" },
                    new Category { Name = "Community", Description = "Council meetings, clean-up drives & civic programs", IconClass = "bi-people-fill" },
                    new Category { Name = "Sports", Description = "Municipal sports meets and marathons", IconClass = "bi-trophy-fill" },
                    new Category { Name = "Workshops", Description = "Skills, safety and youth development workshops", IconClass = "bi-mortarboard-fill" }
                };
                db.Categories.AddRange(categories);
                db.SaveChanges();
            }

            if (!db.Events.Any())
            {
                var cultural = db.Categories.First(c => c.Name == "Cultural");
                var community = db.Categories.First(c => c.Name == "Community");
                var sports = db.Categories.First(c => c.Name == "Sports");
                var workshop = db.Categories.First(c => c.Name == "Workshops");
                var organizer = db.Users.First(u => u.Role == "Organizer");

                db.Events.AddRange(
                    new Event
                    {
                        Title = "Esala Perahera Street Pageant",
                        Description = "A grand torch-lit procession of drummers, dancers and caparisoned tuskers through the streets of Kandy, celebrating the Sacred Tooth Relic.",
                        Venue = "Dalada Veediya, Kandy",
                        EventDate = DateTime.Today.AddDays(14),
                        StartTime = new TimeSpan(19, 30, 0),
                        Capacity = 500,
                        ImageUrl = "/images/events/perahera.jpg",
                        IsFeatured = true,
                        IsPublished = true,
                        ApprovalStatus = "Approved",
                        OrganizerId = null,
                        CategoryId = cultural.CategoryId
                    },
                    new Event
                    {
                        Title = "Kandy City Clean-Up Drive",
                        Description = "Join KMC officers and volunteers for a city-wide clean-up around the Kandy Lake and market area.",
                        Venue = "Kandy Lake Promenade",
                        EventDate = DateTime.Today.AddDays(5),
                        StartTime = new TimeSpan(7, 0, 0),
                        Capacity = 150,
                        ImageUrl = "/images/events/cleanup.jpg",
                        IsFeatured = true,
                        IsPublished = true,
                        ApprovalStatus = "Approved",
                        OrganizerId = null,
                        CategoryId = community.CategoryId
                    },
                    new Event
                    {
                        Title = "KMC Inter-School Marathon",
                        Description = "An annual marathon promoting fitness among school students within the Kandy Municipal Council area.",
                        Venue = "Bogambara Grounds",
                        EventDate = DateTime.Today.AddDays(21),
                        StartTime = new TimeSpan(6, 0, 0),
                        Capacity = 300,
                        ImageUrl = "/images/events/marathon.jpg",
                        IsFeatured = false,
                        IsPublished = true,
                        ApprovalStatus = "Approved",
                        OrganizerId = organizer.UserId,
                        CategoryId = sports.CategoryId
                    },
                    new Event
                    {
                        Title = "Digital Literacy Workshop for Youth",
                        Description = "A free hands-on workshop introducing digital skills, online safety and freelancing basics for Kandy youth.",
                        Venue = "KMC Community Hall",
                        EventDate = DateTime.Today.AddDays(9),
                        StartTime = new TimeSpan(9, 0, 0),
                        Capacity = 80,
                        ImageUrl = "/images/events/workshop.jpg",
                        IsFeatured = true,
                        IsPublished = false,
                        ApprovalStatus = "Pending",
                        OrganizerId = organizer.UserId,
                        CategoryId = workshop.CategoryId
                    }
                );
                db.SaveChanges();
            }
        }
    }
}
