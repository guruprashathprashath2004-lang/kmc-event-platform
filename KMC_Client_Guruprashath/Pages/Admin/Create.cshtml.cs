using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Admin
{
    public class CreateModel : PageModel
    {
        private readonly IApiService api;

        public CreateModel(IApiService _api)
        {
            api = _api;
        }

        [BindProperty]
        public EventWriteVM EventForm { get; set; } = new EventWriteVM();

        private string? RequireAdmin()
        {
            var role = HttpContext.Session.GetString("AuthRole");
            var token = HttpContext.Session.GetString("AuthToken");
            return role == "Admin" ? token : null;
        }

        public async Task<IActionResult> OnGet()
        {
            var token = RequireAdmin();
            if (token == null) return RedirectToPage("/Account/Login");

            EventForm.Categories = await api.GetCategoriesAdminAsync(token);
            return Page();
        }

        public async Task<IActionResult> OnPost(IFormFile? PosterImage)
        {
            var token = RequireAdmin();
            if (token == null) return RedirectToPage("/Account/Login");

            EventForm.Categories = await api.GetCategoriesAdminAsync(token);

            if (!ModelState.IsValid)
                return Page();

            if (PosterImage != null && PosterImage.Length > 0)
            {
                using var stream = PosterImage.OpenReadStream();
                var (url, uploadMessage) = await api.UploadEventImageAsync(stream, PosterImage.FileName, PosterImage.ContentType, token);
                if (url == null)
                {
                    ModelState.AddModelError("", uploadMessage);
                    return Page();
                }
                EventForm.ImageUrl = url;
            }

            var (ok, message) = await api.AddEventAsync(EventForm, token);
            if (!ok)
            {
                ModelState.AddModelError("", message);
                return Page();
            }

            TempData["Success"] = $"'{EventForm.Title}' was created successfully.";
            return RedirectToPage("/Admin/Index");
        }
    }
}
