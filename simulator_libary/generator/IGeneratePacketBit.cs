using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulator_libary.generator
{
    public interface IGeneratePacketBit
    {
        public Task<byte[]> GeneratePacketBitData(int packetDelay, int packetNoise);
    }
}
