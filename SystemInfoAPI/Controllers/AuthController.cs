using Microsoft.AspNetCore.Mvc;
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
            AuthenticationService.LogRequestInfo(request, HttpContext.Connection);

            if (!AuthenticationService.VerifyPassword(request.Pass, request.Salt))
            {
                return Unauthorized();
            }

            string token = AuthenticationService.GenerateJwtToken(_config);
            return Ok(new { Token = token });
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
