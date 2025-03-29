using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;
using TEC_WMS_API.Service;

namespace TEC_WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly IDevice _service;
        public DeviceController(IDevice service)
        {
            _service = service;
        }

        [HttpPost("CreateDevice")]
        public async Task<IActionResult> CreateDevice([FromBody] UpdateDeviceRequest device)
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

        [HttpGet("DevicebyId")]
        public async Task<IActionResult> DevicebyId(int id)
        {
            var user = await _service.GetDeviceByIdAsync(id);
            if (user == null)
            {
                return NotFound(); // Return 404 if user not found
            }
            return Ok(user);
        }

        [HttpPut("UpdateDevice")]
        public async Task<IActionResult> UpdateDevice(int id, [FromBody] UpdateDeviceRequest device)
        {
            if (string.IsNullOrEmpty(device.UserName))
            {
                return BadRequest("Invalid data.");
            }
            device.DeviceId = id;
            var Device = await _service.UpdateDeviceAsync(device);

            if (Device == null)
            {
                return NotFound();
            }

            return Ok(Device);
        }

        [HttpGet("DeviceList")]
        public async Task<IActionResult> DeviceList()
        {
            var user = await _service.GetAllDeviceAsync();
            return Ok(user);
        }

        [HttpDelete("DeleteDevice")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var user = await _service.DeleteDeviceAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet]
        [Route("DevicedropdownList")]
        public async Task<IActionResult> DevicedropdownList()
        {
            var devicedropdown = await _service.GetAllDevicedropdownAsync();
            return Ok(devicedropdown);
        }

    }
}
