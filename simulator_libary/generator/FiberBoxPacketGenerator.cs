using simulator_libary.generator;
using System.Collections.Generic;

namespace simulator_libary
{
    public class FiberBoxPacketGenerator<IcdType> : BasePacketGenerator<IcdType> where IcdType : IParameterIcd
    {
        public FiberBoxPacketGenerator(string json) : base(json){}

        public override void GenerateByteArray(List<IcdType> icdRows, ref byte[] finalSequence, List<IcdType> errorLocations)
        {
            int checkSumValues = 0;
            int corValue = -1;
            foreach (IcdType row in icdRows)
            {
                if (row.GetCorrValue() != -1 && corValue == -1)
                    return;

                if ((row.GetCorrValue() == -1 || row.GetCorrValue() == corValue) && row.GetError() == string.Empty)
                {
                    int randomParamValue = GetParamValue(row,ref errorLocations);

                    if (_prevValue.ContainsKey(row.GetRowId()))
                        _prevValue[row.GetRowId()] = randomParamValue;
                    else
                        _prevValue.Add(row.GetRowId(), randomParamValue);                        

                    if (row.IsRowCorIdentifier())
                        corValue = randomParamValue;

                    if (row.GetRowId() > _icdRows.Count - Consts.CHECKSUM_SIZE - 1 && row.GetRowId() <= _icdRows.Count - 1)       
                        checkSumValues += randomParamValue;
                    

                    AppendValue(randomParamValue, row, ref finalSequence);
                }
            }

            BurnValue(checkSumValues, _icdRows[_icdRows.Count - 1],ref finalSequence);
           
            if (_packetCounter > _curWindowOscillation)
                RestardPacketCounter();
            else _packetCounter++;

        }
    }
}
