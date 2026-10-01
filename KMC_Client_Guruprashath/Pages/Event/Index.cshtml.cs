using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Event
{
    public class IndexModel : PageModel
    {
        private readonly IApiService api;

        public IndexModel(IApiService _api)
        {
            api = _api;
        }

        public List<EventVM> Events { get; set; } = new List<EventVM>();
        public List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        // GET: /Event?categoryId=2&q=perahera
        public async Task OnGet()
        {
            if (!string.IsNullOrWhiteSpace(Q))
                Events = await api.SearchEventsAsync(Q);
            else if (CategoryId.HasValue)
                Events = await api.GetEventsByCategoryAsync(CategoryId.Value);
            else
                Events = await api.GetUpcomingEventsAsync();

            Categories = await api.GetCategoriesAsync();
        }
    }
}
