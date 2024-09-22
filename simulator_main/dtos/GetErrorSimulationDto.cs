using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.dtos
{
    public class GetErrorSimulationDto
    {
        public GetErrorSimulationDto(string icdName,int packetDelayAmount,int packetNoiseAmount)
        {
            this.IcdName = icdName;
            this.PacketDelayAmount = packetDelayAmount;
            this.PacketNoiseAmount = packetNoiseAmount;
        }
        public string IcdName { get; set; }
        public int PacketDelayAmount { get; set; }
        public int PacketNoiseAmount { get; set; }
    }
}
