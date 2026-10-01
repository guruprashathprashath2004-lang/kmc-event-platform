using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KMC_API_Guruprashath.Data;
using KMC_API_Guruprashath.DTO;
using AutoMapper;

namespace KMC_API_Guruprashath.Controllers
{
    // Lets the Admin identify registered accounts by role (Citizen / Organizer / Admin) and manage them.
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IMapper mapper;
        private UserRepo repo;

        public UserController(IMapper _mapper, UserRepo _repo)
        {
            mapper = _mapper;
            repo = _repo;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [HttpGet]
        public ActionResult<List<UserReadDTO>> GetUsers()
        {
            var users = repo.GetAllUsers();
            return Ok(mapper.Map<List<UserReadDTO>>(users));
        }

        [HttpGet("role/{role}")]
        public ActionResult<List<UserReadDTO>> GetByRole(string role)
        {
            var users = repo.GetUsersByRole(role);
            return Ok(mapper.Map<List<UserReadDTO>>(users));
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            var user = repo.GetByID(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            if (user.UserId == CurrentUserId)
                return BadRequest(new { message = "You can't remove your own Admin account while logged in." });

            if (user.Role == "Admin")
                return BadRequest(new { message = "Admin accounts can't be removed from here." });

            if (!repo.DeleteUser(user))
                return BadRequest(new { message = "This user still owns events or registrations, so their account can't be deleted yet." });

            return Ok(new { message = "User removed." });
        }
    }
}
