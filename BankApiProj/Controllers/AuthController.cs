using BankApiProj.Data;
using BankApiProj.Entites;
using BankApiProj.Model;
using BankApiProj.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApiProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;


        private readonly ApiDbContext _apiDbContext;
        public AuthController(IAuthService authService,ApiDbContext apiDbContext)
        {
            _authService = authService;
            _apiDbContext = apiDbContext;
        }

        //[HttpPost("register")]
        //public async Task<ActionResult<User>> Register(User user, string password)
        //{
        //    var newUser = await _authService.Register(user, password);
        //    return Ok(newUser);
        //}

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(int userId, [FromQuery] string password)
        {
            if (userId == 0 || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("User data and password are required.");
            }

            var user=await _apiDbContext.Users.Where(e=>e.UserId==userId).FirstOrDefaultAsync();

            var newUser = await _authService.Register(user, password);

            if (newUser == null)
            {
                return BadRequest("Registration failed.");
            }

            return Ok(newUser);
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginDto)
        {
            var token = await _authService.Login(loginDto.Email, loginDto.Password);
            if (token == null) return Unauthorized();

            return Ok(token);
        }
    }
}
