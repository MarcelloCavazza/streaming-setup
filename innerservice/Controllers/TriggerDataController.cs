using Microsoft.AspNetCore.Mvc;
using innerservice.BLs.Interfaces;
using Models;

namespace innerservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TriggerDataController : ControllerBase
    {
        private readonly IOutterServiceBL _outterServiceBL;

        public TriggerDataController(IOutterServiceBL outterservicebl)
        {
            _outterServiceBL = outterservicebl;
        }

        [HttpGet]
        public async Task<IActionResult> TriggerIncommingDataAsync()
        {
            var id = _outterServiceBL.ExecuteCall();

            return await Task.FromResult(Ok(new TrigerredResult()
            {
                Id = id.ToString()
            }));
        }

        
    }
}