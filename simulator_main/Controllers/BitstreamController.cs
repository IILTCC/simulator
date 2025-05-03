using Microsoft.AspNetCore.Mvc;
using simulator_libary.Enums;
using simulator_libary.icds;
using simulator_main.dtos;
using simulator_main.Dtos;
using simulator_main.services;
using System.Collections.Generic;

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
        public ReturnStatus StartBitstream([FromBody] GetSimulationDto simulationDto)
        {
              return _BitstreamService.GetPacketData(simulationDto);
        }        

        [HttpPost("startErrorSimulation")]
        public ReturnStatus StartErrorBitstream([FromBody] GetErrorSimulationDto getErrorSimulationDto)
        {
             return _BitstreamService.GetPacketErrorData(getErrorSimulationDto);
        }

        [HttpPost("stopSimulator")]
        public ReturnStatus StopSimulator([FromBody] StopSimulatorDto stopSimulatorDto)
        {
            return _BitstreamService.StopSimulator(stopSimulatorDto);
        }
        [HttpGet("getCurrent")]
        public GetChannelsCurrentDto getCurrentChannels()
        {
            return _BitstreamService.GetCurrentChannels();
        }
    }
}
