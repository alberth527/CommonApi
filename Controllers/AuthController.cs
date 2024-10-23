using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Comm.Model.Entity;
using Comm.WebUtil;

namespace CommonApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly JwtHelper _jwtHelper;

        public AuthController(IConfiguration configuration, JwtHelper jwtHelper)
        {
            _configuration = configuration;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegistrationRequest request)
        {
            // Implement user registration logic here
            // For example, save user details to the database

            return Ok(new APIResult { IsSuccess = true, Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginRequest request)
        {
            // Implement user login logic here
            // For example, validate user credentials against the database

            var token = _jwtHelper.GenerateToken(request.Username);

            return Ok(new APIResult { IsSuccess = true, Message = "Login successful", Data = token });
        }
    }

    public class UserRegistrationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
