using System.Threading.Tasks;

namespace simulator_libary.generator
{
    public interface IGeneratePacketBit
    {
        public Task<byte[]> GeneratePacketBitData(int packetDelay, int packetNoise);
    }
}
