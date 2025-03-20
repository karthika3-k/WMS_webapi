using Microsoft.AspNetCore.Mvc;
using TEC_WMS_API.Models.RequestModel;
using TEC_WMS_API.Service;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TEC_WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LoginService _service;
        private readonly ILogger _logger;
        public LoginController(LoginService service)
        {
            _service = service;
        }      

        [HttpGet("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            if ( string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username or Password cannot be empty.");
            }

            var loginResponse = await _service.GetByIdAsync(username,password);

            if (loginResponse == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(loginResponse);
        }
        
    }
}
