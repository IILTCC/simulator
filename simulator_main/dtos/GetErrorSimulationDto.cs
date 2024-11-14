using simulator_libary.icds;

namespace simulator_main.dtos
{
    public class GetErrorSimulationDto
    {
        public IcdTypes IcdType { get; set; }
        public int PacketDelayAmount { get; set; }
        public int PacketNoiseAmount { get; set; }

        public GetErrorSimulationDto(IcdTypes icdType,int packetDelayAmount,int packetNoiseAmount)
        {
            this.IcdType = icdType;
            this.PacketDelayAmount = packetDelayAmount;
            this.PacketNoiseAmount = packetNoiseAmount;
        }
    }
}
