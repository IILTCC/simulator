using simulator_libary.Enums;
using simulator_libary.icds;
using simulator_main.dtos;
using simulator_main.Dtos;
using System.Collections.Generic;

namespace simulator_main.services
{
    public interface IBitstreamService
    {
        public ReturnStatus GetPacketData(GetSimulationDto getSimulationDto);
        public ReturnStatus GetPacketErrorData(GetErrorSimulationDto getSimulationErroDto);
        public ReturnStatus StopSimulator(StopSimulatorDto icdType);
        public GetChannelsCurrentDto GetCurrentChannels();

    }
}
