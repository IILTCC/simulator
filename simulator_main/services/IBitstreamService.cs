using simulator_main.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.services
{
    public interface IBitstreamService
    {
        public Task GetPacketDataAsync(GetSimulationDto getSimulationDto);
        public Task GetPacketErrorDataAsync(GetErrorSimulationDto getSimulationErroDto);

    }
}
