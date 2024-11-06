namespace simulator_main.dtos
{
    public class GetErrorSimulationDto
    {
        public string IcdName { get; set; }
        public int PacketDelayAmount { get; set; }
        public int PacketNoiseAmount { get; set; }

        public GetErrorSimulationDto(string icdName,int packetDelayAmount,int packetNoiseAmount)
        {
            this.IcdName = icdName;
            this.PacketDelayAmount = packetDelayAmount;
            this.PacketNoiseAmount = packetNoiseAmount;
        }
    }
}
