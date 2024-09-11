using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        [HttpGet("getSimulation{icdId}")]
       
        public async Task<string> GetBitstream(int icdId)
        {
            return await _BitstreamService.GetBitstream(icdId);
        }
    }
}
