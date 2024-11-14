using System.Collections.Generic;
using System.Threading.Tasks;

namespace simulator_libary.generator
{
    public interface IBasePacketGenerator<IcdType> where IcdType : IBaseIcd
    {
        // gets a value and returns a byte array in the exact length needed (in 8 bits per item)
        public byte[] GetAccurateByte(int value, int size);

        public void CreateMask(string rowMask, ref byte currentValue);

        public void GenerateByteArray(List<IcdType> icdRows, ref byte[] finalSequence, List<IcdType> errorLocations);

        public int InduceError(IcdType row);

        public List<IcdType> ErrorArrayLocation(List<IcdType> icdRows, int packetNoise);

        public Task<byte[]> GeneratePacketBitData(int packetDelay, int packetNoise);

    }
}
