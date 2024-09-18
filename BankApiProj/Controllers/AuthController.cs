using BankApiProj.Entites;
using BankApiProj.Model;
using BankApiProj.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankApiProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user, string password)
        {
            var newUser = await _authService.Register(user, password);
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
