using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;
using TEC_WMS_API.Service;

namespace TEC_WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BinMasterController : ControllerBase
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


        [HttpGet("BinMasterByWhs")]
        public async Task<IActionResult> BinMasterByWhs(string whsCode)
        {
            var binConfig = await _service.GetBinMasterByWhsAsync(whsCode);
            if (binConfig == null)
            {
                return NotFound();
            }
            return Ok(binConfig);
        }


        [HttpPost("CreateBinMaster")]
        public async Task<IActionResult> CreateBinMasterAsync([FromBody] List<BinMasterRequest> binMasters)
        {
            if (binMasters == null || binMasters.Count == 0)
            {
                return BadRequest("No data provided.");
            }

            try
            {
                int insertedRows = await _service.CreateBinMasterAsync(binMasters);
                return Ok(new { message = "Data inserted successfully", count = insertedRows });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
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
