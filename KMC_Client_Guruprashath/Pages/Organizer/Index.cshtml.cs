using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Organizer
{
    public class IndexModel : PageModel
    {
        private readonly IApiService api;

        public IndexModel(IApiService _api)
        {
            api = _api;
        }

        public string? OrganizerName { get; set; }
        public List<EventVM> Events { get; set; } = new List<EventVM>();

        private string? RequireOrganizer()
        {
            var role = HttpContext.Session.GetString("AuthRole");
            var token = HttpContext.Session.GetString("AuthToken");
            return role == "Organizer" ? token : null;
        }

        public async Task<IActionResult> OnGet()
        {
            var token = RequireOrganizer();
            if (token == null)
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Organizer/Index") });

            OrganizerName = HttpContext.Session.GetString("AuthName");
            Events = await api.GetMyEventsAsync(token);
            return Page();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var token = RequireOrganizer();
            if (token == null) return RedirectToPage("/Account/Login");

            await api.DeleteEventAsync(id, token);
            TempData["Success"] = "Event removed.";
            return RedirectToPage("/Organizer/Index");
        }
    }
}
