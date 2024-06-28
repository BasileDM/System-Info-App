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
        private readonly string _apiPass;

        public AuthController(IConfiguration configuration)
        {
            _config = configuration;
            _apiPass = AuthenticationService.ValidateApiPass(_config["ApiPassword"]);
        }

        // POST: api/<Auth>/GetToken
        [HttpPost]
        public IActionResult GetToken([FromBody] AuthRequest request)
        {
            try
            {
                AuthenticationService.LogRequestInfo(request, HttpContext.Connection);

                if (!AuthenticationService.VerifyPassword(_apiPass, request.Pass, request.Salt))
                {
                    return Unauthorized();
                }

                string token = AuthenticationService.GenerateJwtToken(_config);

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while issuing token: {ex.Message}");
                return StatusCode(500, "Could not deliver token.");
            }
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
