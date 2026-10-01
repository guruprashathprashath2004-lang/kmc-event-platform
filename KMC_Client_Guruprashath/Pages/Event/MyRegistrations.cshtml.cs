using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Event
{
    public class MyRegistrationsModel : PageModel
    {
        private readonly IApiService api;

        public MyRegistrationsModel(IApiService _api)
        {
            api = _api;
        }

        public List<RegistrationVM> Registrations { get; set; } = new List<RegistrationVM>();

        public async Task<IActionResult> OnGet()
        {
            var role = HttpContext.Session.GetString("AuthRole");
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token) || role != "Citizen")
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Event/MyRegistrations") });

            Registrations = await api.GetMyRegistrationsAsync(token);
            return Page();
        }
    }
}
