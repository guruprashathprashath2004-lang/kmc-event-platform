using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Event
{
    public class RegisterModel : PageModel
    {
        private readonly IApiService api;

        public RegisterModel(IApiService _api)
        {
            api = _api;
        }

        [BindProperty]
        public RegistrationVM RegForm { get; set; } = new RegistrationVM();

        // GET: /Event/Register/5 - citizens must be logged in to book a seat.
        public async Task<IActionResult> OnGet(int id)
        {
            var role = HttpContext.Session.GetString("AuthRole");
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Please log in (or create a free account) to register for this event.";
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Event/Register", new { id }) });
            }

            if (role != "Citizen")
            {
                TempData["Error"] = "Only citizen accounts can register for events. Please log in with a citizen account.";
                return RedirectToPage("/Event/Details", new { id });
            }

            var ev = await api.GetEventByIdAsync(id);
            if (ev == null)
                return NotFound();

            if (ev.SeatsLeft <= 0)
            {
                TempData["Error"] = "Sorry, this event is fully booked.";
                return RedirectToPage("/Event/Details", new { id });
            }

            RegForm = new RegistrationVM
            {
                EventId = id,
                Event = ev,
                FullName = HttpContext.Session.GetString("AuthName") ?? ""
            };
            return Page();
        }

        // POST: /Event/Register/5
        public async Task<IActionResult> OnPost()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            var role = HttpContext.Session.GetString("AuthRole");

            if (string.IsNullOrEmpty(token) || role != "Citizen")
            {
                TempData["Error"] = "Please log in with a citizen account to register.";
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Event/Register", new { id = RegForm.EventId }) });
            }

            var ev = await api.GetEventByIdAsync(RegForm.EventId);
            if (ev == null)
                return NotFound();

            RegForm.Event = ev;

            if (!ModelState.IsValid)
                return Page();

            var (success, message) = await api.RegisterForEventAsync(RegForm, token);

            if (!success)
            {
                ModelState.AddModelError("", message);
                return Page();
            }

            TempData["RegisteredName"] = RegForm.FullName;
            return RedirectToPage("/Event/RegisterSuccess", new { id = RegForm.EventId });
        }
    }
}
