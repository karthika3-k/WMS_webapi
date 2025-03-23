using Microsoft.AspNetCore.Mvc;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;
using TEC_WMS_API.Service;


namespace TEC_WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
          
            var result = await _service.CreateBinConfigsAsync(binConfigs);

            if (result <= 0)
            {
                return BadRequest("BinConfig creation failed.");
            }
           
            return CreatedAtAction(nameof(CreateBinConfig), new { count = result }, binConfigs);
        }


        // PUT api/<BinConfigController>/5
        [HttpPut("UpdateBinConfig")]
        public async Task<IActionResult> UpdateBinConfig(int id, [FromBody] BinConfigRequest binConfig)
        {
            if (string.IsNullOrEmpty(binConfig.BinCode))
            {
                return BadRequest("Invalid data.");
            }
            var user = await _service.UpdateBinConfigAsync(id,binConfig);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // DELETE api/<BinConfigController>/5
        [HttpDelete("DeleteBinConfig")]
        public async Task<IActionResult> DeleteBinConfig(int id)
        {
            var user = await _service.DeleteBinConfigAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
