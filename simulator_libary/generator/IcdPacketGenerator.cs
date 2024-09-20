﻿using Newtonsoft.Json;
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
            Console.WriteLine(size / 8 + (size % 8!=0?1:0));
            byte[] finalValue = new byte[size / 8 + (size % 8 != 0 ? 1 : 0)];
            int i = 0;
            for (int j = 0; j < finalValue.Length; i++ ,j++)
            {
                finalValue[j] = startValue[i];
            }
            return finalValue;
        }
        // turns a byte array into a continues string
        private string ByteArrayToString(byte[] byteArray)
        {
            string returnString = string.Empty;
            foreach (byte item in byteArray)
                returnString += Convert.ToString(item, 2).PadLeft(8, '0');
            returnString = returnString.Remove(returnString.Length - 1);
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
        private void printbyte(byte[] array)
        {
            foreach (var item in array)
            {
                Console.Write(Convert.ToString(item, 2).PadLeft(8, '0')+" ");
            }
            Console.WriteLine();
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
                    Console.WriteLine("_________________");
                    Console.WriteLine("id "+row.GetRowId());
                    Console.WriteLine("location "+row.GetLocation());
                    Console.WriteLine("min "+row.GetMin());
                    Console.WriteLine("max "+row.GetMax());
                    Console.WriteLine("location "+row.GetLocation());
                    Console.WriteLine("rnd10 "+randomParamValue);

                    if (errorLocations.Count>0&& row.GetRowId() == errorLocations[0].GetRowId())
                    {

                        Console.WriteLine();
                        Console.Write("createing error instead of "+randomParamValue+" " );
                        randomParamValue = InduceError(row);
                        errorLocations.RemoveAt(0);
                        Console.Write(randomParamValue);
                        Console.WriteLine();
                    }

                    if (row.IsRowCorIdentifier())
                        corValue = randomParamValue;
                    
                    byte[] currentValue = GetAccurateByte(randomParamValue,row.GetSize());
                    Console.WriteLine("rnd2");
                    printbyte(currentValue);
                    Console.WriteLine("mask ");
                    CreateMask(row.GetMask(),ref currentValue[0]);

                    for (int i = 0; i < currentValue.Length; i++)
                        // append current value of all sizes to final sequence
                        finalSequence[row.GetLocation() + i] = (byte)(finalSequence[row.GetLocation() + i] | currentValue[i]);
                }
            }
            printbyte(finalSequence);
        }
        // create local erros in one location
        private int InduceError(IcdType row)
        {

            //To do : fix not working with signed ie id 1


            int physicalUpperLimit;
            int physicalLowerLimit;
            int rndValue;
            if (row.GetMin() < 0 || row.GetMax() < 0)
            {
                // if signed
                physicalUpperLimit = (int)(Math.Pow(2,row.GetSize()-1));
                physicalLowerLimit = (int)(-Math.Pow(2, row.GetSize() - 1))-1;
            }
            else
            {
                physicalLowerLimit = 0;
                physicalUpperLimit = (int)(Math.Pow(2, row.GetSize()));
            }
            if (row.GetMax() == physicalUpperLimit)
                rndValue = rnd.Next(physicalLowerLimit, row.GetMin());
            else rndValue = rnd.Next(row.GetMax(), physicalUpperLimit);
            return rndValue;

        }
        // run on valid locations and create errors throughout the byte array
        //private void CreatePacketErrors(int packetNoise,ref byte[] finalSequence,List<IcdType> validLocations)
        //{
        //    // find all valid locations for changing values meaing parameteres that dont full
        //    // advantage of the bit range ie 11111111 should not be 255 to be valid

        //    for(int i = 0;i<packetNoise && validLocations.Count>0;i++)
        //    {
        //        int randomItem = rnd.Next(0,validLocations.Count);
        //        //byte[] tempArray = new byte[validLocations[randomItem].GetSize()/8];
        //        //for(int j = 0;j<tempArray.Length;j++)
        //        //{
        //        //    tempArray[j] = finalSequence[validLocations[randomItem].GetLocation() + j];
        //        //}
        //        InduceError(ref finalSequence,validLocations[randomItem]);
        //    }
        //}

        // create an array to where create a packet error while beintg sorted from small to big
        private List<IcdType> ErrorArrayLocation(List<IcdType> icdRows,int packetNoise)
        {
            // get all valid locations of where we can create error
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
            foreach(var item in validLocations)
            {
                Console.WriteLine("going to create error at id "+item.GetRowId());
            }

            return validLocations;
        }
        public string GeneratePacketBitData(string json,float packetDelay,int packetNoise)
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

            //InduceError(icdRows[0]);
            //InduceError(icdRows[12]);
            GenerateByteArray(icdRows, ref finalSequence, ErrorArrayLocation(icdRows,packetNoise));            

            return ByteArrayToString(finalSequence);

        }            
    }
}
