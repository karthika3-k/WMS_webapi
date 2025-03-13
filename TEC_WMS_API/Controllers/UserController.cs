using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TEC_WMS_API.Models.RequestModel;
using TEC_WMS_API.Service;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TEC_WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        private readonly ILogger _logger;

        public UserController(UserService service)
        {
            _service = service;
        }
        // GET: api/<UserController>
        [HttpGet("UserList")]
        public async Task<IActionResult> UserList()
        {
           var user = await _service.GetAllUserAsync();
            return Ok(user);
        }

        // GET api/<UserController>/5
        [HttpGet("UserbyId")]
        public async Task<IActionResult> UserbyId(int id)
        {
            var user = await _service.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(); // Return 404 if user not found
            }
            return Ok(user);
        }

        // POST api/<UserController>
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] LoginRequest login)
        {
            if (login == null)
            {
                return BadRequest("Invalid data."); // Return 400 if the body is null or invalid
            }
            var user = await _service.CreateUserAsync(login);
            if (user == null)
            {
                return BadRequest("User creation failed.");
            }

            return CreatedAtAction(nameof(CreateUser), new { id = login.UserId}, user);

        }

        // PUT api/<UserController>/5
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] LoginRequest login)
        {
            if (string.IsNullOrEmpty(login.UserName))
            {
                return BadRequest("Invalid data."); 
            }

            var user = await _service.UpdateUserAsync(login); 

            if (user == null)
            {
                return NotFound(); 
            }

            return Ok(user);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _service.DeleteUserAsync(id); 

            if (user == null)
            {
                return NotFound(); 
            }

            return NoContent(); 
        }
    }
}
