using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InwardController : ControllerBase
    {

        private readonly IInward _service;

        public InwardController(IInward service)
        {
            _service = service;
        }
        [HttpGet("InwardList")]
        public async Task<IActionResult> InwardList(string userSign)
        {
            var inward = await _service.GetAllInwardAsync(userSign);
            return Ok(inward);
        }

        [HttpPost("CreateInwardChild")]
        public async Task<IActionResult> CreateInwardChildAsync([FromBody] List<InwardChildRequest> inwardChildRequest)
        {
            if (inwardChildRequest == null)
            {
                return BadRequest("No data provided.");
            }

            try
            {
                bool insertedRows = await _service.CreateInwardChildAsync(inwardChildRequest);

                if (insertedRows)
                {
                    return Ok(new { message = "Data inserted successfully" });
                }
                else
                {
                    return Ok(new { message = "No rows were inserted. Please check input BaseEntry and ItemCode." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





        [HttpPut("UpdateInward")]
        public async Task<IActionResult> UpdateInward([FromBody] List<InwardChildRequest> inwardList, [FromQuery] string UserSign)
        {
            if (inwardList == null || !inwardList.Any())
            {
                return BadRequest("Inward list is empty or null.");
            }

            bool result = await _service.UpdateInwardAsync(inwardList, UserSign);

            if (!result)
            {
                return StatusCode(500, "Update failed.");
            }

            return Ok("Update successful.");
        }


    }
}
