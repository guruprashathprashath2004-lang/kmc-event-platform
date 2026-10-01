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
    public class RegistrationController : ControllerBase
    {
        private IMapper mapper;
        private RegistrationRepo repo;
        private EventRepo eventRepo;
        private IWebHostEnvironment env;

        public RegistrationController(IMapper _mapper, RegistrationRepo _repo, EventRepo _eventRepo, IWebHostEnvironment _env)
        {
            mapper = _mapper;
            repo = _repo;
            eventRepo = _eventRepo;
            env = _env;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<RegistrationReadDTO>> GetRegistrations()
        {
            var regs = repo.GetRegistrations();
            return Ok(mapper.Map<List<RegistrationReadDTO>>(regs));
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpGet("event/{eventId}")]
        public ActionResult<List<RegistrationReadDTO>> GetByEvent(int eventId)
        {
            var ev = eventRepo.GetEventByID(eventId);
            if (ev == null) return NotFound();

            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == "Organizer" && ev.OrganizerId != CurrentUserId)
                return Forbid();

            var regs = repo.GetRegistrationsByEvent(eventId);
            return Ok(mapper.Map<List<RegistrationReadDTO>>(regs));
        }

        // A logged-in Citizen's own registration history.
        [Authorize(Roles = "Citizen")]
        [HttpGet("mine")]
        public ActionResult<List<RegistrationReadDTO>> GetMine()
        {
            var regs = repo.GetRegistrationsByUser(CurrentUserId);
            return Ok(mapper.Map<List<RegistrationReadDTO>>(regs));
        }

        // Registering for an event now requires a logged-in Citizen account.
        [Authorize(Roles = "Citizen")]
        [HttpPost]
        public ActionResult Register(RegistrationWriteDTO dto)
        {
            var ev = eventRepo.GetEventByID(dto.EventId);
            if (ev == null)
                return NotFound(new { message = "Event not found." });

            if (repo.CountForEvent(dto.EventId) >= ev.Capacity)
                return BadRequest(new { message = "This event is fully booked. No seats left." });

            if (repo.IsAlreadyRegistered(dto.EventId, CurrentUserId))
                return BadRequest(new { message = "You have already registered for this event." });

            var registration = mapper.Map<Registration>(dto);
            registration.UserId = CurrentUserId;

            if (repo.AddRegistration(registration))
                return Ok(new { registration.RegistrationId, message = "Registration successful." });

            return BadRequest(new { message = "Could not complete registration." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteRegistration(int id)
        {
            var reg = repo.GetRegistrationByID(id);
            if (reg != null)
                if (repo.RemoveRegistration(reg))
                    return Ok();
            return NotFound();
        }
    }
}
