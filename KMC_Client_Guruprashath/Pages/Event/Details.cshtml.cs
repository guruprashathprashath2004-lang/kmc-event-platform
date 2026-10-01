using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Event
{
    public class DetailsModel : PageModel
    {
        private readonly IApiService api;

        public DetailsModel(IApiService _api)
        {
            api = _api;
        }

        public EventVM Event { get; set; } = new EventVM();

        public async Task<IActionResult> OnGet(int id)
        {
            var ev = await api.GetEventByIdAsync(id);
            if (ev == null)
                return NotFound();

            Event = ev;
            return Page();
        }
    }
}
