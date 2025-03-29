using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;
using TEC_WMS_API.Service;


namespace TEC_WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BinConfigController : ControllerBase
    {
        private readonly IBinConfig _service;
        private readonly ILogger _logger;

        public BinConfigController(IBinConfig service)
        {
            _service = service;
        }
        // GET: api/<BinConfigController>
        [HttpGet("BinConfigList")]
        public async Task<IActionResult> BinConfigList()
        {
            var binConfig = await _service.GetAllBinConfigAsync();
            return Ok(binConfig);
        }
        
        // GET: api/<BinConfigController>
        [HttpGet("BinConfigListbyWhsCode")]
        public async Task<IActionResult> BinConfigListbyWhsCode(string whsCode)
        {
            var binConfig = await _service.GetAllBinListbywhsConfigAsync(whsCode);
            return Ok(binConfig);
        }

        // GET api/<BinConfigController>/5
        [HttpGet("BinConfigid")]
        public async Task<IActionResult> BinConfigid(int id)
        {
            var binConfig = await _service.GetBinConfigByIdAsync(id);
            if (binConfig==null)
            {
                return NotFound();
            }
            return Ok(binConfig);
        }

        // POST api/<BinConfigController>
        [HttpPost("CreateBinConfig")]
        public async Task<IActionResult> CreateBinConfig([FromBody] IEnumerable<BinConfigRequest> binConfigs)
        {
            if (binConfigs == null || !binConfigs.Any())
            {
                return BadRequest("Invalid data or empty list.");
            }

            // Assuming CreateBinConfigsAsync returns a string, so we parse it into an integer
            var result = await _service.CreateBinConfigsAsync(binConfigs);

            // Try to parse the result to an integer
            if (!int.TryParse(result, out int resultCode))
            {
                return Ok(result);
            }

            // Check if the result indicates failure (e.g., resultCode == 0)
            if (resultCode == 0)
            {
                return NotFound();
            }

            // If successful, return a CreatedAtAction response with the count
            return CreatedAtAction(nameof(CreateBinConfig), new { count = resultCode }, binConfigs);
        }


        // PUT api/<BinConfigController>/5
        [HttpPut("UpdateBinConfig")]
        public async Task<IActionResult> UpdateBinConfig(IEnumerable<BinConfigRequest> binConfigs)
        {
            if (binConfigs == null || !binConfigs.Any())
            {
                return BadRequest("Invalid data or empty list.");
            }
            var user = await _service.UpdateBinConfigAsync(binConfigs);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // DELETE api/<BinConfigController>/5
        [HttpDelete("DeleteBinConfig")]
        public async Task<IActionResult> DeleteBinConfig(string whsCode)
        {
            var user = await _service.DeleteBinConfigAsync(whsCode);

            if (user == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
