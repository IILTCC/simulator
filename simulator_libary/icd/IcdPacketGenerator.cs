﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulator_main.icd
{
    public class IcdPacketGenerator<icd> where icd : BaseIcd
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
        public string GeneratePacketBitData(string json)
        {
            List<icd> icdRows;
            try
            {
                icdRows = JsonConvert.DeserializeObject<List<icd>>(json);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
                return string.Empty;
            }
            // create byte array as amount of bytes needed, divide by 9 is there for end case of icd ending with size greater than 8
            int sequenceArraySize = icdRows[icdRows.Count - 1].Location + 1 + icdRows[icdRows.Count - 1].Size / 9;
            byte[] finalSequence = new byte[sequenceArraySize];

            foreach(icd row in icdRows)
            {
                int randomParamValue = rnd.Next(row.Min, row.Max + 1);
                byte[] currentValue = GetAccurateByte(randomParamValue);
                
                if (row.Mask != string.Empty) 
                {
                    byte mask = Convert.ToByte(row.Mask, 2);
                    // push currentValue to be in mask range
                    while ((mask & 0b00000001) != 1)
                    {
                        mask = (byte)(mask >> 1);
                        // 0 - assuming if there is a mask, max size of value was less then 8 bits
                        currentValue[0] = (byte)(currentValue[0] << 1);
                    }
                }
                for(int i = 0;i<currentValue.Length;i++)
                    // append current value of all sizes to final sequence
                    finalSequence[row.Location+i] = (byte)(finalSequence[row.Location+i] | currentValue[i]);
            }

            string returnString = string.Empty;
            foreach(var item in finalSequence)
                returnString+= Convert.ToString(item, 2).PadLeft(8, '0') + " ";
            returnString =returnString.Remove(returnString.Length - 1);
            return returnString;
        }            
    }
}