using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SystemInfoApi.Services;

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

        // POST: api/<Auth>/GetToken
        [HttpPost]
        public IActionResult GetToken([FromBody] AuthRequest request)
        {
            Console.WriteLine();
            Console.WriteLine($"New token requested from: {HttpContext.Connection.RemoteIpAddress?.ToString()}");
            Console.WriteLine($"Request Content: Hash = {request.Pass}, Salt = {request.Salt}");

            if (CryptoService.VerifyPassword(request.Pass, request.Salt))
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
        public string Pass { get; set; }
        [Required]
        public byte[] Salt { get; set; }
    }
}
