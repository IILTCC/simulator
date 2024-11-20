using simulator_libary.Enums;
using simulator_main.dtos;

namespace simulator_main.services
{
    public interface IBitstreamService
    {
        public ReturnAnswers GetPacketData(GetSimulationDto getSimulationDto);
        public ReturnAnswers GetPacketErrorData(GetErrorSimulationDto getSimulationErroDto);
        public ReturnAnswers StopSimulator(StopSimulatorDto icdType);
    }
}
