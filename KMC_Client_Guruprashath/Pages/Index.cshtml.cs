using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IApiService api;

        public IndexModel(IApiService _api)
        {
            api = _api;
        }

        public List<EventVM> UpcomingEvents { get; set; } = new List<EventVM>();
        public List<EventVM> FeaturedEvents { get; set; } = new List<EventVM>();
        public List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();

        public async Task OnGet()
        {
            FeaturedEvents = await api.GetFeaturedEventsAsync();
            var upcoming = await api.GetUpcomingEventsAsync();
            Categories = await api.GetCategoriesAsync();

            UpcomingEvents = upcoming.Take(6).ToList();
        }
    }
}
