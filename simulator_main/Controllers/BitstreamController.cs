using Microsoft.AspNetCore.Mvc;
using simulator_main.dtos;
using simulator_main.services;

namespace simulator_main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BitstreamController : ControllerBase
    {
        IBitstreamService _BitstreamService;
        public BitstreamController(IBitstreamService bitstreamService)
        {
            _BitstreamService = bitstreamService;
        }

        [HttpPost("startSimulation")]
        public void GetBitstreamAsync([FromBody] GetSimulationDto simulationDto)
        {
              _BitstreamService.GetPacketData(simulationDto);
        }        

        [HttpPost("startErrorSimulation")]
        public void GetErrorBitstreamAsync([FromBody] GetErrorSimulationDto getErrorSimulationDto)
        {
             _BitstreamService.GetPacketErrorData(getErrorSimulationDto);
        }

        [HttpPost("stopSimulator")]
        public void StopSimulator()
        {
            _BitstreamService.StopSimulator();
        }

    }
}
