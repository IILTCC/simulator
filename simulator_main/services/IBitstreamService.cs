using simulator_libary.Enums;
using simulator_main.dtos;

namespace simulator_main.services
{
    public interface IBitstreamService
    {
        public ReturnStatus GetPacketData(GetSimulationDto getSimulationDto);
        public ReturnStatus GetPacketErrorData(GetErrorSimulationDto getSimulationErroDto);
        public ReturnStatus StopSimulator(StopSimulatorDto icdType);
    }
}
