using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KMC_API_Guruprashath.Model;
using KMC_API_Guruprashath.Data;
using KMC_API_Guruprashath.DTO;
using AutoMapper;

namespace KMC_API_Guruprashath.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private IMapper mapper;
        private EventRepo repo;
        private IWebHostEnvironment env;

        public EventController(IMapper _mapper, EventRepo _repo, IWebHostEnvironment _env)
        {
            mapper = _mapper;
            repo = _repo;
            env = _env;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        private string CurrentRole => User.FindFirstValue(ClaimTypes.Role) ?? "";

        // ---- Public browsing (Admin-approved + published events only) ----

        [HttpGet]
        public ActionResult<List<EventReadDTO>> GetEvents()
        {
            var events = repo.GetEvents();
            return Ok(mapper.Map<List<EventReadDTO>>(events));
        }

        [HttpGet("upcoming")]
        public ActionResult<List<EventReadDTO>> GetUpcoming()
        {
            var events = repo.GetUpcomingPublishedEvents();
            return Ok(mapper.Map<List<EventReadDTO>>(events));
        }

        [HttpGet("featured")]
        public ActionResult<List<EventReadDTO>> GetFeatured()
        {
            var events = repo.GetFeaturedEvents();
            return Ok(mapper.Map<List<EventReadDTO>>(events));
        }

        [HttpGet("category/{categoryId}")]
        public ActionResult<List<EventReadDTO>> GetByCategory(int categoryId)
        {
            var events = repo.GetEventsByCategory(categoryId);
            return Ok(mapper.Map<List<EventReadDTO>>(events));
        }

        [HttpGet("{id}")]
        public ActionResult<EventReadDTO> GetEventByID(int id)
        {
            var ev = repo.GetEventByID(id);
            if (ev != null)
                return Ok(mapper.Map<EventReadDTO>(ev));
            return NotFound();
        }

        [HttpGet("search/{key}")]
        public ActionResult<List<EventReadDTO>> Search(string key)
        {
            var events = repo.SearchEvents(key);
            if (events.Count > 0)
                return Ok(mapper.Map<List<EventReadDTO>>(events));
            return NotFound();
        }

        // ---- Organizer: manage own events ----

        [Authorize(Roles = "Organizer")]
        [HttpGet("mine")]
        public ActionResult<List<EventReadDTO>> GetMyEvents()
        {
            var events = repo.GetEventsByOrganizer(CurrentUserId);
            return Ok(mapper.Map<List<EventReadDTO>>(events));
        }

        // ---- Admin: approval queue ----

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public ActionResult<List<EventReadDTO>> GetPending()
        {
            var events = repo.GetPendingEvents();
            return Ok(mapper.Map<List<EventReadDTO>>(events));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/approve")]
        public ActionResult Approve(int id)
        {
            var ev = repo.GetEventByID(id);
            if (ev == null) return NotFound();

            ev.ApprovalStatus = "Approved";
            ev.IsPublished = true;
            if (repo.UpdateEvent(ev)) return Ok();
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/reject")]
        public ActionResult Reject(int id)
        {
            var ev = repo.GetEventByID(id);
            if (ev == null) return NotFound();

            ev.ApprovalStatus = "Rejected";
            ev.IsPublished = false;
            if (repo.UpdateEvent(ev)) return Ok();
            return BadRequest();
        }

        // ---- Create / Update / Delete (Organizer: own events only, Admin: any event) ----

        [Authorize(Roles = "Organizer,Admin")]
        [HttpPost]
        public ActionResult AddEvent(EventWriteDTO dto)
        {
            try
            {
                var ev = mapper.Map<Event>(dto);

                if (CurrentRole == "Organizer")
                {
                    // Organizer-created events always need Admin approval before going live.
                    ev.OrganizerId = CurrentUserId;
                    ev.ApprovalStatus = "Pending";
                    ev.IsPublished = false;
                }
                else
                {
                    ev.OrganizerId = null;
                    ev.ApprovalStatus = "Approved";
                }

                if (repo.AddEvent(ev))
                    return Ok(new { ev.EventId });

                return BadRequest("The event could not be saved (no rows were written).");
            }
            catch (Exception ex)
            {
                // Surface the real error instead of a silent failure - makes debugging possible from the client side too.
                var detail = ex.InnerException?.Message ?? ex.Message;
                return BadRequest($"Server error while creating event: {detail}");
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpPut("{id}")]
        public ActionResult UpdateEvent(EventWriteDTO dto, int id)
        {
            var existing = repo.GetEventByID(id);
            if (existing == null)
                return NotFound();

            if (CurrentRole == "Organizer" && existing.OrganizerId != CurrentUserId)
                return Forbid();

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.Venue = dto.Venue;
            existing.EventDate = dto.EventDate;
            existing.StartTime = dto.StartTime;
            existing.Capacity = dto.Capacity;
            if (!string.IsNullOrEmpty(dto.ImageUrl))
                existing.ImageUrl = dto.ImageUrl;
            existing.IsFeatured = dto.IsFeatured;
            existing.CategoryId = dto.CategoryId;

            if (CurrentRole == "Organizer")
            {
                // Any edit by the organizer must be re-approved by Admin.
                existing.ApprovalStatus = "Pending";
                existing.IsPublished = false;
            }
            else
            {
                existing.IsPublished = dto.IsPublished;
            }

            if (repo.UpdateEvent(existing))
                return Ok();
            return BadRequest();
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteEvent(int id)
        {
            var ev = repo.GetEventByID(id);
            if (ev == null) return NotFound();

            if (CurrentRole == "Organizer" && ev.OrganizerId != CurrentUserId)
                return Forbid();

            if (repo.RemoveEvent(ev))
                return Ok();
            return BadRequest();
        }

        // Handles poster image upload for an event and returns the relative URL to store on the event record.
        [Authorize(Roles = "Organizer,Admin")]
        [HttpPost("upload-image")]
        public async Task<ActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file supplied.");

                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest("Image is too large (max 5 MB).");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
                    return BadRequest("Unsupported file type. Please upload a JPG, PNG, GIF, or WEBP image.");

                var uploadsFolder = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images", "events");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { url = $"{Request.Scheme}://{Request.Host}/images/events/{fileName}" });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Image upload failed: {detail}");
            }
        }
    }
}
