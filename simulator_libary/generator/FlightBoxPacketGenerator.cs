using System.Collections.Generic;
using System.IO;

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
                    if (_prevValue.ContainsKey(row.GetRowId()))
                        _prevValue[row.GetRowId()] = randomParamValue;
                    else
                        _prevValue.Add(row.GetRowId(), randomParamValue);

                    AppendValue(randomParamValue, row, ref finalSequence);

                }
            }
            if (_packetCounter > _curWindowOscillation)
                RestardPacketCounter();
            else _packetCounter++;
        }
    }
}
