using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using simulator_main.dtos;
using simulator_main.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [HttpPost("getSimulation")]
        public async Task GetBitstreamAsync([FromBody] GetSimulationDto simulationDto)
        {
             await _BitstreamService.GetPacketDataAsync(simulationDto);
        }        
        [HttpPost("getErrorSimulation")]
        public async Task GetErrorBitstreamAsync([FromBody] GetErrorSimulationDto getErrorSimulationDto)
        {
            await _BitstreamService.GetPacketErrorDataAsync(getErrorSimulationDto);
        }
    }
}
