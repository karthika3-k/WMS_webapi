using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;
using TEC_WMS_API.Service;

namespace TEC_WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDevice _service;
        public DeviceController(IDevice service)
        {
            _service = service;
        }

        [HttpPost("CreateDevice")]
        public async Task<IActionResult> CreateDevice([FromBody] DeviceRequest device)
        {
            try
            {
                // 🚨 Ensure DeviceId is not included in the request
                device.DeviceId = null;

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new { Error = "Validation failed", Details = errors });
                }

                var result = await _service.CreateDeviceAsync(device);

                if (result <= 0)
                {
                    return BadRequest("Device creation failed.");
                }

                return CreatedAtAction(nameof(CreateDevice), new { id = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred", Details = ex.Message });
            }
        }




    }
}
