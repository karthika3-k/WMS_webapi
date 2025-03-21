using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Controllers
{
    public class BinMasterController : Controller
    {

        private readonly IBinMaster _service;      

        public BinMasterController(IBinMaster service)
        {
            _service = service;
        }


        [HttpGet("BinMasterList")]
        public async Task<IActionResult> BinMasterList()
        {
            var binMaster = await _service.GetAllMasterConfigAsync();
            return Ok(binMaster);
        }

        [HttpGet("BinMasterById")]
        public async Task<IActionResult> BinMasterById(int id)
        {
            var binConfig = await _service.GetBinMasterByIdAsync(id);
            if (binConfig == null)
            {
                return NotFound();
            }
            return Ok(binConfig);
        }


        [HttpPost("CreateBinMaster")]
        public async Task<IActionResult> CreateBinMaster([FromBody] BinMasterRequest binMaster)
        {
            if (binMaster == null)
            {
                return BadRequest("Invalid data.");
            }
            var binConfigResponse = await _service.CreateBinMasterAsync(binMaster);
            if (binConfigResponse == null)
            {
                return BadRequest("User creation failed.");
            }
            return CreatedAtAction(nameof(CreateBinMaster), new { id = binMaster.BinID }, binMaster);
        }

        [HttpPut("UpdateBinMaster")]
        public async Task<IActionResult> UpdateBinMaster(int id, [FromBody] BinMasterRequest binMaster)
        {
            if (string.IsNullOrEmpty(binMaster.BinID.ToString()))
            {
                return BadRequest("Invalid data.");
            }
            var user = await _service.UpdateBinMasterAsync(id, binMaster);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete("DeleteBinMaster")]
        public async Task<IActionResult> DeleteBinMaster(int id)
        {
            var user = await _service.DeleteBinMasterAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
