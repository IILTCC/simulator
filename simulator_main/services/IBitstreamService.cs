using simulator_libary.icds;
using simulator_main.dtos;

namespace simulator_main.services
{
    public interface IBitstreamService
    {
        public void GetPacketData(GetSimulationDto getSimulationDto);
        public void GetPacketErrorData(GetErrorSimulationDto getSimulationErroDto);
        public void StopSimulator(StopSimulatorDto icdType);

    }
}
