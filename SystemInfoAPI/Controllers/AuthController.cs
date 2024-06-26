using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SystemInfoApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [HttpPost]
        public IActionResult GetToken([FromBody] AuthRequest request)
        {
            if (request.ClientId == "YourClientId" && request.ClientSecret == "YourClientSecret")
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var SecurityToken = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    null,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials);

                string token = new JwtSecurityTokenHandler().WriteToken(SecurityToken);
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }
    }

    public class AuthRequest
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
    }
}
