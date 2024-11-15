using Microsoft.AspNetCore.Mvc;
using simulator_libary.Enums;
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
        public ReturnAnswers GetBitstreamAsync([FromBody] GetSimulationDto simulationDto)
        {
              return _BitstreamService.GetPacketData(simulationDto);
        }        

        [HttpPost("startErrorSimulation")]
        public ReturnAnswers GetErrorBitstreamAsync([FromBody] GetErrorSimulationDto getErrorSimulationDto)
        {
             return _BitstreamService.GetPacketErrorData(getErrorSimulationDto);
        }

        [HttpPost("stopSimulator")]
        public ReturnAnswers StopSimulator([FromBody] StopSimulatorDto stopSimulatorDto)
        {
            return _BitstreamService.StopSimulator(stopSimulatorDto);
        }

    }
}
