using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEC_WMS_API.Interface;

namespace TEC_WMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseController : ControllerBase
    {
        private readonly IWareHouse _service;
        private readonly ILogger _logger;
        public WareHouseController(IWareHouse service)
        {
            _service = service;
        }
        [HttpGet]
        [Route("WareHouseList")]
        public async Task<IActionResult> WareHouseList()
        {
            var wareHouse = await _service.GetAllWareHouseAsync();
            return Ok(wareHouse);
        }
        
    }
}
