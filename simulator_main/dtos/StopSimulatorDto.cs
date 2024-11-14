using simulator_libary.icds;

namespace simulator_main.dtos
{
    public class StopSimulatorDto
    {
        public IcdTypes IcdType { get; set; }
        public StopSimulatorDto(IcdTypes icdType)
        {
            IcdType = icdType;
        }
    }
}
