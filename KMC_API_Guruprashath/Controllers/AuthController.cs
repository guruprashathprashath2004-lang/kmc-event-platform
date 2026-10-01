using Microsoft.AspNetCore.Mvc;
using KMC_API_Guruprashath.Data;
using KMC_API_Guruprashath.DTO;
using KMC_API_Guruprashath.Helpers;
using KMC_API_Guruprashath.Model;

namespace KMC_API_Guruprashath.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserRepo repo;
        private JwtHelper jwtHelper;

        private static readonly string[] AllowedSelfRegisterRoles = { "Citizen", "Organizer" };

        public AuthController(UserRepo _repo, JwtHelper _jwtHelper)
        {
            repo = _repo;
            jwtHelper = _jwtHelper;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponseDTO> Login(LoginRequestDTO dto)
        {
            var user = repo.GetByUsername(dto.Username ?? "");
            if (user == null || user.PasswordHash != DbSeeder.HashPassword(dto.Password ?? ""))
            {
                return Ok(new LoginResponseDTO { Success = false, Message = "Invalid username or password." });
            }

            var token = jwtHelper.GenerateToken(user);
            return Ok(new LoginResponseDTO
            {
                Success = true,
                Token = token,
                FullName = user.FullName,
                Role = user.Role,
                Message = "Login successful."
            });
        }

        // Public sign-up for Citizens (to register for events) and Organizers (to create events).
        // Admin accounts can never be created through this endpoint.
        [HttpPost("register")]
        public ActionResult<LoginResponseDTO> Register(RegisterRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return Ok(new LoginResponseDTO { Success = false, Message = "Username, email and password are required." });

            if (!AllowedSelfRegisterRoles.Contains(dto.Role))
                return Ok(new LoginResponseDTO { Success = false, Message = "Role must be either Citizen or Organizer." });

            if (repo.UsernameOrEmailTaken(dto.Username, dto.Email))
                return Ok(new LoginResponseDTO { Success = false, Message = "That username or email is already registered." });

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = DbSeeder.HashPassword(dto.Password),
                FullName = dto.FullName,
                Phone = dto.Phone,
                Role = dto.Role
            };

            if (!repo.AddUser(user))
                return Ok(new LoginResponseDTO { Success = false, Message = "Could not create the account. Please try again." });

            var token = jwtHelper.GenerateToken(user);
            return Ok(new LoginResponseDTO
            {
                Success = true,
                Token = token,
                FullName = user.FullName,
                Role = user.Role,
                Message = "Account created successfully."
            });
        }
    }
}
