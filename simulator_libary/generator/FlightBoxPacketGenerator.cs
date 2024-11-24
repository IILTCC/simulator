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
                    int randomParamValue = GetParamValue(row, ref errorLocations);

                    AppendValue(randomParamValue, row, ref finalSequence);
                }
            }
        }
    }
}
