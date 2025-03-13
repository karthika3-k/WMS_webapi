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
        //// GET: api/<LoginController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<LoginController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<LoginController>



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

        //[HttpPost("myroute")]
        //public async Task<IActionResult> Post([FromBody] LoginRequest loginRequest)
        //{
        //    if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.Password))
        //    {
        //        return BadRequest("Username or Password cannot be empty.");
        //    }

        //    var loginResponse = await _service.GetByIdAsync(loginRequest.UserName, loginRequest.Password);

        //    if (loginResponse == null)
        //    {
        //        return Unauthorized("Invalid username or password.");
        //    }

        //    return Ok(loginResponse);
        //}


        //// PUT api/<LoginController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<LoginController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
