using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace simulator_libary.generator
{
    public abstract class BasePacketGenerator<IcdType> : IGeneratePacketBit where IcdType:IParameterIcd
    {
        protected static Random rnd;
        protected List<IcdType> _icdRows;
        protected Dictionary<int,int> _prevValue;
        protected int _packetCounter;
        protected int _curWindowOscillation;
        public BasePacketGenerator(string json)
        {
            rnd = new Random();
            _prevValue = new Dictionary<int, int>();
            _packetCounter = 0;
            try
            {
                _icdRows = JsonConvert.DeserializeObject<List<IcdType>>(json);
            }
            catch (Exception e)
            {
                return;
            }
        }

        public void RestardPacketCounter()
        {
            _curWindowOscillation =  rnd.Next(Consts.SIMULATOR_OSCILATION_WINDOW_MIN, Consts.SIMULATOR_OSCILATION_WINDOW_MAX);
            _packetCounter = 0;
        }

        // gets a value and returns a byte array in the exact length needed (in 8 bits per item)
        public byte[] GetAccurateByte(int value, int size)
        {
            byte[] startValue = BitConverter.GetBytes(value);
            int finalValueSize = size / Consts.BYTE_SIZE + (size % Consts.BYTE_SIZE != 0 ? 1 : 0);
            byte[] finalValue = new byte[finalValueSize];

            int i = 0;
            for (int j = 0; j < finalValue.Length; i++, j++)
                finalValue[j] = startValue[i];

            return finalValue;
        }

        public void CreateMask(string rowMask, ref byte currentValue)
        {
            if (rowMask != string.Empty)
            {
                byte mask = Convert.ToByte(rowMask, 2);
                // push currentValue to be in mask range
                while ((mask & 0b00000001) != 1)
                {
                    mask = (byte)(mask >> 1);
                    currentValue = (byte)(currentValue << 1);
                }
            }
        }
        public abstract void GenerateByteArray(List<IcdType> icdRows, ref byte[] finalSequence, List<IcdType> errorLocations);

        public int GetParamValue(IcdType row, ref List<IcdType> errorLocations)
        {
            int randomParamValue = rnd.Next(row.GetMin(), row.GetMax() + 1);
            if(_prevValue.ContainsKey(row.GetRowId()))
            {
                bool isUp = false;
                if (_packetCounter<_curWindowOscillation/2)
                    isUp = rnd.NextDouble() >= Consts.SIMULATOR_JUMP_UP;
                
                else if(_packetCounter>_curWindowOscillation/2 && _packetCounter<_curWindowOscillation)
                    isUp = rnd.NextDouble() >= Consts.SIMULATOR_JUMP_DOWN;

                float currWindow = (float)(rnd.Next(0,Consts.SIMULATOR_RAND_WINDOW))/100;
                currWindow = (row.GetMax() - row.GetMin()) * currWindow;
                if ((row.GetMax() - row.GetMin()) * currWindow < 1)
                    currWindow = 1;

                if (isUp) // true == up
                {
                    randomParamValue = (int)(_prevValue[row.GetRowId()] + currWindow);
                    if (randomParamValue > row.GetMax())
                        randomParamValue = row.GetMax();
                }
                else
                {
                    randomParamValue = (int)(_prevValue[row.GetRowId()] - currWindow);
                    if (randomParamValue < row.GetMin())
                        randomParamValue = row.GetMin();
                }

            }
            if (errorLocations.Count > 0 && row.GetRowId() == errorLocations[0].GetRowId())
            {
                randomParamValue = InduceError(row);
                errorLocations.RemoveAt(0);
            }
            return randomParamValue;
        }

        public void AppendValue(int randomParamValue, IcdType row, ref byte[] finalSequence)
        {
            byte[] currentValue = GetAccurateByte(randomParamValue, row.GetSize());

            CreateMask(row.GetMask(), ref currentValue[0]);

            for (int i = 0; i < currentValue.Length; i++)
                // append current value of all sizes to final sequence
                finalSequence[row.GetLocation() + i] = (byte)(finalSequence[row.GetLocation() + i] | currentValue[i]);
        }
        public void BurnValue(int paramValue, IcdType row,ref byte[] finalSequence)
        {
            if (paramValue > row.GetMax())
                paramValue = row.GetMax();
            byte[] currentValue = GetAccurateByte(paramValue, row.GetSize());

            CreateMask(row.GetMask(), ref currentValue[0]);

            for (int i = 0; i < currentValue.Length; i++)
                // append current value of all sizes to final sequence
                finalSequence[row.GetLocation() + i] = (byte)(currentValue[i]);
        }
        public int InduceError(IcdType row)
        {
            // caluculate actual max size
            int physicalUpperLimit;
            int physicalLowerLimit;
            int rndValue;

            if (row.GetMin() < 0 || row.GetMax() < 0)
            {
                physicalUpperLimit = (int)(Math.Pow(2, row.GetSize() - 1));
                physicalLowerLimit = (int)(-Math.Pow(2, row.GetSize() - 1)) - 1;
            }
            else
            {
                physicalLowerLimit = 0;
                physicalUpperLimit = (int)(Math.Pow(2, row.GetSize()));
            }

            // check where we leave a gap between actual size and request size
            if (row.GetMax() == physicalUpperLimit)
                rndValue = rnd.Next(physicalLowerLimit, row.GetMin());
            else rndValue = rnd.Next(row.GetMax() + 1, physicalUpperLimit);
            return rndValue;
        }

        public List<IcdType> ErrorArrayLocation(List<IcdType> icdRows, int packetNoise)
        {
            List<IcdType> validLocations = new List<IcdType>();
            foreach (IcdType icdRow in icdRows)
            {
                if ((icdRow.GetMax() - icdRow.GetMin() + 1) != Math.Pow(2, icdRow.GetSize()))
                    if(icdRow.GetCorrValue() == -1 && icdRow.GetError()==string.Empty)
                    validLocations.Add(icdRow);
            }

            // reduce the array to the desired amount
            while (validLocations.Count > packetNoise)
            {
                int randomLocatoin = rnd.Next(0, validLocations.Count);
                validLocations.RemoveAt(randomLocatoin);
            }

            return validLocations;
        }
        // packet noise - amount of parameters errors, packet delay - delay in miliseconds
        public async Task<byte[]> GeneratePacketBitData(int packetDelay, int packetNoise)
        {
            // create byte array as amount of bytes needed, divide by 9 is there for end case of icd ending with size greater than 8
            int sequenceArraySize = _icdRows[_icdRows.Count - 1].GetLocation() + 1 + _icdRows[_icdRows.Count - 1].GetSize() / Consts.LAST_ROW_DIVIDER;
            byte[] finalSequence = new byte[sequenceArraySize];

            GenerateByteArray(_icdRows, ref finalSequence, ErrorArrayLocation(_icdRows, packetNoise));

            if (packetDelay > 0)
            {
                return await Task.Run(async () =>
                {
                    int rndDelay = rnd.Next(-(int)Consts.PACKET_DELAY_RANDOMNESS * packetDelay, (int)Consts.PACKET_DELAY_RANDOMNESS * packetDelay);
                    await Task.Delay(packetDelay + rndDelay);
                    return (finalSequence);
                });
            }
            return (finalSequence);
        }
    }
}
