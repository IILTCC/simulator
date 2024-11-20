using System.Collections.Generic;

namespace simulator_libary.generator
{
    public class FlightBoxPacketGenerator<IcdType> : BasePacketGenerator<IcdType> where IcdType : IParameterIcd
    {
        public FlightBoxPacketGenerator(string json) : base (json){}
        public override  void GenerateByteArray(List<IcdType> icdRows, ref byte[] finalSequence, List<IcdType> errorLocations)
        {
            foreach (IcdType row in icdRows)
            {
                if (row.GetError() == string.Empty)
                {
                    int randomParamValue = rnd.Next(row.GetMin(), row.GetMax() + 1);

                    if (errorLocations.Count > 0 && row.GetRowId() == errorLocations[0].GetRowId())
                    {
                        randomParamValue = InduceError(row);
                        errorLocations.RemoveAt(0);
                    }

                    byte[] currentValue = GetAccurateByte(randomParamValue, row.GetSize());

                    CreateMask(row.GetMask(), ref currentValue[0]);

                    for (int i = 0; i < currentValue.Length; i++)
                        // append current value of all sizes to final sequence
                        finalSequence[row.GetLocation() + i] = (byte)(finalSequence[row.GetLocation() + i] | currentValue[i]);
                }
            }
        }
    }
}
