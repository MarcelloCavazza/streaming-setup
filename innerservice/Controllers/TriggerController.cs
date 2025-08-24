using Microsoft.AspNetCore.Mvc;
using innerservice.BLs.Interfaces;
using Models.InnerService.Responses.Trigger;

namespace innerservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TriggerController : ControllerBase
    {
        private readonly IOutterServiceBL _outterServiceBL;

        public TriggerController(IOutterServiceBL outterservicebl)
        {
            _outterServiceBL = outterservicebl;
        }

        [HttpGet]
        public async Task<IActionResult> TriggerIncommingDataAsync()
        {
            var id = _outterServiceBL.ExecuteCall();

            return await Task.FromResult(Ok(new TriggerResponse()
            {
                Id = id
            }));
        }

        
    }
}