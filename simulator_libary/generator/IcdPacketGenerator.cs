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
        private byte[] GetAccurateByte(int value)
        {
            byte[] startValue = BitConverter.GetBytes(value);
            int size = 0;
            for(int i =0;i<startValue.Length;i++)
                if (startValue[i] != 0)
                    size++;
            // size==0? value 0 will still result in array with one item with zeros in it
            Array.Resize<byte>(ref startValue, size==0?1:size);
            return startValue;
        }
        private string ByteArrayToString(byte[] byteArray)
        {
            string returnString = string.Empty;
            foreach (byte item in byteArray)
                returnString += Convert.ToString(item, 2).PadLeft(8, '0');
            returnString = returnString.Remove(returnString.Length - 1);
            return returnString;
        }
        private void GenerateByteArray(List<IcdType> icdRows, ref byte[] finalSequence)
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
                    if (row.IsRowCorIdentifier())
                        corValue = randomParamValue;
                    
                    byte[] currentValue = GetAccurateByte(randomParamValue);

                    if (row.GetMask() != string.Empty)
                    {
                        byte mask = Convert.ToByte(row.GetMask(), 2);
                        // push currentValue to be in mask range
                        while ((mask & 0b00000001) != 1)
                        {
                            mask = (byte)(mask >> 1);
                            // 0 - assuming if there is a mask, max size of value was less then 8 bits
                            currentValue[0] = (byte)(currentValue[0] << 1);
                        }
                    }
                    for (int i = 0; i < currentValue.Length; i++)
                        // append current value of all sizes to final sequence
                        finalSequence[row.GetLocation() + i] = (byte)(finalSequence[row.GetLocation() + i] | currentValue[i]);
                }
            }
        }
        public string GeneratePacketBitData(string json)
        {
            List<IcdType> icdRows;
            try
            {
                icdRows = JsonConvert.DeserializeObject<List<IcdType>>(json);
            }
            catch(Exception)
            {
                return string.Empty;
            }
            // create byte array as amount of bytes needed, divide by 9 is there for end case of icd ending with size greater than 8
            int sequenceArraySize = icdRows[icdRows.Count - 1].GetLocation() + 1 + icdRows[icdRows.Count - 1].GetSize() / 9;
            byte[] finalSequence = new byte[sequenceArraySize];

            GenerateByteArray(icdRows,ref finalSequence);

            return ByteArrayToString(finalSequence);

        }            
    }
}
