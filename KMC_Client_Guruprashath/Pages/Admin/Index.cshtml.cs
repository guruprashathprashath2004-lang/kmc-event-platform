using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IApiService api;

        public IndexModel(IApiService _api)
        {
            api = _api;
        }

        public string? AdminName { get; set; }
        public List<EventVM> Events { get; set; } = new List<EventVM>();

        private string? RequireAdmin()
        {
            var role = HttpContext.Session.GetString("AuthRole");
            var token = HttpContext.Session.GetString("AuthToken");
            return role == "Admin" ? token : null;
        }

        public async Task<IActionResult> OnGet()
        {
            var token = RequireAdmin();
            if (token == null)
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Admin/Index") });

            AdminName = HttpContext.Session.GetString("AuthName");
            var events = await api.GetAllEventsAsync();
            Events = events.OrderByDescending(e => e.EventDate).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var token = RequireAdmin();
            if (token == null) return RedirectToPage("/Account/Login");

            await api.DeleteEventAsync(id, token);
            TempData["Success"] = "Event deleted.";
            return RedirectToPage("/Admin/Index");
        }
    }
}
