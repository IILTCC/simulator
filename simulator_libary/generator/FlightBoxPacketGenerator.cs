﻿using System.Collections.Generic;
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

                    if (row.GetRowId()==28)
                        File.AppendAllText(@"C:\Users\user\Desktop\miniproj\mainproject\DownSampling\TestingGround\sample\simtest.txt",randomParamValue.ToString()+"\n");
                    if (row.GetRowId() == 22)
                        File.AppendAllText(@"C:\Users\user\Desktop\miniproj\mainproject\DownSampling\TestingGround\sample\simtest22.txt", randomParamValue.ToString() + "\n");
                    if (row.GetRowId() == 25)
                        File.AppendAllText(@"C:\Users\user\Desktop\miniproj\mainproject\DownSampling\TestingGround\sample\simtest25.txt", randomParamValue.ToString() + "\n");
                    AppendValue(randomParamValue, row, ref finalSequence);

                }
            }
            if (_packetCounter > _curWindowOscillation)
                RestardPacketCounter();
            else _packetCounter++;
        }
    }
}
