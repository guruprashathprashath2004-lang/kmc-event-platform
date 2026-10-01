using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly IApiService api;

        public UsersModel(IApiService _api)
        {
            api = _api;
        }

        public List<UserVM> Users { get; set; } = new List<UserVM>();

        [BindProperty(SupportsGet = true)]
        public string? Role { get; set; }

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
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Admin/Users") });

            var users = await api.GetUsersAsync(token);
            if (!string.IsNullOrEmpty(Role))
                users = users.Where(u => u.Role.Equals(Role, StringComparison.OrdinalIgnoreCase)).ToList();

            Users = users;
            return Page();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var token = RequireAdmin();
            if (token == null) return RedirectToPage("/Account/Login");

            var (success, message) = await api.DeleteUserAsync(id, token);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToPage("/Admin/Users", new { Role });
        }
    }
}
