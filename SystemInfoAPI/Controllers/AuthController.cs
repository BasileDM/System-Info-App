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
        private readonly string? _apiPass;

        public AuthController(IConfiguration configuration)
        {
            _config = configuration;
            _apiPass = _config["ApiPassword"];
        }

        // POST: api/<Auth>/GetToken
        [HttpPost]
        public IActionResult GetToken([FromBody] AuthRequest request)
        {
            try
            {
                AuthenticationService.LogRequestInfo(request, HttpContext.Connection);

                string validPass = AuthenticationService.ValidateApiPass(_apiPass);
                if (!AuthenticationService.VerifyPassword(validPass, request.Pass, request.Salt))
                {
                    return Unauthorized();
                }

                string token = AuthenticationService.GenerateJwtToken(_config);
                return Ok(new { Token = token });
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error has occured.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while issuing token: {ex.Message}");
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
