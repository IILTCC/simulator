using Newtonsoft.Json;
using simulator_libary.generator;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace simulator_libary
{
    public class CorrelatorPacketGenerator<IcdType> : BasePacketGenerator<IcdType> where IcdType : IParameterIcd
    {
        public CorrelatorPacketGenerator(string json)
        {
            rnd = new Random();
            try
            {
                _icdRows = JsonConvert.DeserializeObject<List<IcdType>>(json);
            }
            catch (Exception e)
            {
                return;
            }
        }
        public override void GenerateByteArray(List<IcdType> icdRows, ref byte[] finalSequence, List<IcdType> errorLocations)
        {
            int corValue = -1;
            foreach (IcdType row in icdRows)
            {
                if (row.GetCorrValue() != -1 && corValue == -1)
                    return;

                if ((row.GetCorrValue() == -1 || row.GetCorrValue() == corValue) && row.GetError() == string.Empty)
                {
                    int randomParamValue = rnd.Next(row.GetMin(), row.GetMax() + 1);
                    // creates error at this row id if demanded by errorLocation list
                    if (errorLocations.Count > 0 && row.GetRowId() == errorLocations[0].GetRowId())
                    {
                        randomParamValue = InduceError(row);
                        errorLocations.RemoveAt(0);
                    }

                    if (row.IsRowCorIdentifier())
                        corValue = randomParamValue;

                    // parses the requested data to byte array
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
