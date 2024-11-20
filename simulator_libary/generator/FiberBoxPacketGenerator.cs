﻿using simulator_libary.generator;
using System.Collections.Generic;

namespace simulator_libary
{
    public class FiberBoxPacketGenerator<IcdType> : BasePacketGenerator<IcdType> where IcdType : IParameterIcd
    {
        public FiberBoxPacketGenerator(string json):base(json){}
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

                    if (errorLocations.Count > 0 && row.GetRowId() == errorLocations[0].GetRowId())
                    {
                        randomParamValue = InduceError(row);
                        errorLocations.RemoveAt(0);
                    }

                    if (row.IsRowCorIdentifier())
                        corValue = randomParamValue;

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
