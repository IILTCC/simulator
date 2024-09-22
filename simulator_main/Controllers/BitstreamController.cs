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
        public async Task<string> GetBitstreamAsync([FromBody] GetSimulationDto simulationDto)
        {
            return  await _BitstreamService.GetPacketDataAsync(simulationDto);
        }        
        [HttpPost("getErrorSimulation")]
        public async Task<string> GetErrorBitstreamAsync([FromBody] GetErrorSimulationDto getErrorSimulationDto)
        {
            return await _BitstreamService.GetPacketErrorDataAsync(getErrorSimulationDto);
        }
    }
}
