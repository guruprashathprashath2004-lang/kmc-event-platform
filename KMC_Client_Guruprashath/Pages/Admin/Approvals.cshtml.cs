using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Admin
{
    public class ApprovalsModel : PageModel
    {
        private readonly IApiService api;

        public ApprovalsModel(IApiService _api)
        {
            api = _api;
        }

        public List<EventVM> Pending { get; set; } = new List<EventVM>();

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
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Admin/Approvals") });

            Pending = await api.GetPendingEventsAsync(token);
            return Page();
        }

        public async Task<IActionResult> OnPostApprove(int id)
        {
            var token = RequireAdmin();
            if (token == null) return RedirectToPage("/Account/Login");

            await api.ApproveEventAsync(id, token);
            TempData["Success"] = "Event approved and published.";
            return RedirectToPage("/Admin/Approvals");
        }

        public async Task<IActionResult> OnPostReject(int id)
        {
            var token = RequireAdmin();
            if (token == null) return RedirectToPage("/Account/Login");

            await api.RejectEventAsync(id, token);
            TempData["Success"] = "Event rejected.";
            return RedirectToPage("/Admin/Approvals");
        }
    }
}
