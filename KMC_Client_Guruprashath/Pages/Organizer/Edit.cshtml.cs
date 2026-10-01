using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KMC_Client_Guruprashath.Models;
using KMC_Client_Guruprashath.Services;

namespace KMC_Client_Guruprashath.Pages.Organizer
{
    public class EditModel : PageModel
    {
        private readonly IApiService api;

        public EditModel(IApiService _api)
        {
            api = _api;
        }

        [BindProperty]
        public EventWriteVM EventForm { get; set; } = new EventWriteVM();

        private string? RequireOrganizer()
        {
            var role = HttpContext.Session.GetString("AuthRole");
            var token = HttpContext.Session.GetString("AuthToken");
            return role == "Organizer" ? token : null;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var token = RequireOrganizer();
            if (token == null) return RedirectToPage("/Account/Login");

            var ev = await api.GetEventByIdAsync(id);
            if (ev == null) return NotFound();

            EventForm = new EventWriteVM
            {
                EventId = ev.EventId,
                Title = ev.Title,
                Description = ev.Description,
                Venue = ev.Venue,
                EventDate = ev.EventDate,
                StartTime = ev.StartTime,
                Capacity = ev.Capacity,
                ImageUrl = ev.ImageUrl,
                IsFeatured = ev.IsFeatured,
                IsPublished = ev.IsPublished,
                CategoryId = ev.CategoryId,
                Categories = await api.GetCategoriesAsync()
            };

            return Page();
        }

        public async Task<IActionResult> OnPost(IFormFile? PosterImage)
        {
            var token = RequireOrganizer();
            if (token == null) return RedirectToPage("/Account/Login");

            EventForm.Categories = await api.GetCategoriesAsync();

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

            var (ok, message) = await api.UpdateEventAsync(EventForm, token);
            if (!ok)
            {
                ModelState.AddModelError("", message);
                return Page();
            }

            TempData["Success"] = $"'{EventForm.Title}' was updated and sent back for Admin re-approval.";
            return RedirectToPage("/Organizer/Index");
        }
    }
}
