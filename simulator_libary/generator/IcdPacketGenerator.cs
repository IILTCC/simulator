using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class IcdPacketGenerator<IcdType> where IcdType : IBaseIcd
    {
        private static Random rnd;
        public IcdPacketGenerator()
        {
            rnd = new Random();
        }

        // gets a value and returns a byte array in the exact length needed (in 8 bits per item)
        private byte[] GetAccurateByte(int value,int size)
        {
            byte[] startValue = BitConverter.GetBytes(value);

            int finalValueSize = size / 8 + (size % 8 != 0 ? 1 : 0);

            // creats an array for final size and adds one length for uncounted bits (less than 8)
            byte[] finalValue = new byte[finalValueSize];
            int i = 0;
            for (int j = 0; j < finalValue.Length; i++ ,j++)
                finalValue[j] = startValue[i];

            return finalValue;
        }
        // turns a byte array into a continues string
        private string ByteArrayToString(byte[] byteArray)
        {
            string returnString = string.Empty;
            foreach (byte item in byteArray)
                returnString += Convert.ToString(item, 2).PadLeft(8, '0');
            return returnString;
        }
        private void CreateMask(string rowMask, ref byte currentValue)
        {
            if (rowMask != string.Empty)
            {
                byte mask = Convert.ToByte(rowMask, 2);
                // push currentValue to be in mask range
                while ((mask & 0b00000001) != 1)
                {
                    mask = (byte)(mask >> 1);
                    // 0 - assuming if there is a mask, max size of value was less then 8 bits
                    currentValue = (byte)(currentValue << 1);
                }
            }
        }      
        private void GenerateByteArray(List<IcdType> icdRows, ref byte[] finalSequence,List<IcdType> errorLocations)
        {
            int corValue = -1;
            foreach (IcdType row in icdRows)
            {
                //ilegal icd
                if (row.GetCorrValue() != -1 && corValue == -1)
                    return;
                // check if no error in line and that cor value is good
                if ((row.GetCorrValue() == -1 || row.GetCorrValue() == corValue) && row.GetError()==string.Empty)
                {
                    int randomParamValue = rnd.Next(row.GetMin(), row.GetMax() + 1);
                    // creates error at this row id if demanded by errorLocation list
                    if (errorLocations.Count>0 && row.GetRowId() == errorLocations[0].GetRowId())
                    {
                        randomParamValue = InduceError(row);
                        errorLocations.RemoveAt(0);
                    }
                    // if this row contains corralator update cor value
                    if (row.IsRowCorIdentifier())
                        corValue = randomParamValue;

                    // parses the requested data to byte array
                    byte[] currentValue = GetAccurateByte(randomParamValue,row.GetSize());
                    
                    CreateMask(row.GetMask(),ref currentValue[0]);

                    for (int i = 0; i < currentValue.Length; i++)
                        // append current value of all sizes to final sequence
                        finalSequence[row.GetLocation() + i] = (byte)(finalSequence[row.GetLocation() + i] | currentValue[i]);
                }
            }
        }

        // create local erros in one location
        private int InduceError(IcdType row)
        {
            // caluculate actual max size
            int physicalUpperLimit;
            int physicalLowerLimit;
            int rndValue;
            
            if (row.GetMin() < 0 || row.GetMax() < 0)
            {
                physicalUpperLimit = (int)(Math.Pow(2,row.GetSize()-1));
                physicalLowerLimit = (int)(-Math.Pow(2, row.GetSize() - 1))-1;
            }
            else
            {
                physicalLowerLimit = 0;
                physicalUpperLimit = (int)(Math.Pow(2, row.GetSize()));
            }
            // check where we leave a gap between actual size and request size
            if (row.GetMax() == physicalUpperLimit)
                rndValue = rnd.Next(physicalLowerLimit, row.GetMin());
            else rndValue = rnd.Next(row.GetMax(), physicalUpperLimit);
            return rndValue;

        }


        // create an array to where create a packet error
        private List<IcdType> ErrorArrayLocation(List<IcdType> icdRows,int packetNoise)
        {
            List<IcdType> validLocations = new List<IcdType>();
            foreach (IcdType icdRow in icdRows)
            {
                if ((icdRow.GetMax() - icdRow.GetMin() + 1) != Math.Pow(2, icdRow.GetSize()))
                    validLocations.Add(icdRow);
            }

            // reduce the array to the desired amount
            while(validLocations.Count > packetNoise)
            {
                int randomLocatoin = rnd.Next(0, validLocations.Count);
                validLocations.RemoveAt(randomLocatoin);
            }

            return validLocations;
        }
        public async Task<string> GeneratePacketBitData(string json, int packetDelay, int packetNoise)
        {
            const int PACKET_DELAY_RANDOMNESS = 100;
            List<IcdType> icdRows;
            try
            {
                icdRows = JsonConvert.DeserializeObject<List<IcdType>>(json);
            }
            catch (Exception)
            {
                return string.Empty;
            }
            // create byte array as amount of bytes needed, divide by 9 is there for end case of icd ending with size greater than 8
            int sequenceArraySize = icdRows[icdRows.Count - 1].GetLocation() + 1 + icdRows[icdRows.Count - 1].GetSize() / 9;
            byte[] finalSequence = new byte[sequenceArraySize];

            GenerateByteArray(icdRows, ref finalSequence, ErrorArrayLocation(icdRows, packetNoise));

            if (packetDelay > 0)
            {
                return await Task.Run(async () =>
                {
                    await Task.Delay(packetDelay + rnd.Next(-PACKET_DELAY_RANDOMNESS,PACKET_DELAY_RANDOMNESS));
                    return ByteArrayToString(finalSequence);
                });
            }
            return ByteArrayToString(finalSequence);

        }            
    }
}
