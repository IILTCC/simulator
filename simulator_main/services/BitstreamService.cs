using simulator_libary;
using simulator_libary.icds;
using simulator_main.dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace simulator_main.services
{
    public class BitstreamService : IBitstreamService
    {


        private CancellationTokenSource bitStreamCancelTokenSource;
        private CancellationToken bitStreamCancelToken;
        private readonly Dictionary<IcdTypes, Type> IcdDictionary;
        private readonly SocketConnection TelemetryConnection;
        public BitstreamService(SocketConnection telemetryConnection)
        {
            TelemetryConnection = telemetryConnection;
            IcdDictionary = new Dictionary<IcdTypes, Type>();
            InitializeIcdDict();

            bitStreamCancelTokenSource = new CancellationTokenSource();
            bitStreamCancelToken = bitStreamCancelTokenSource.Token;
            bitStreamCancelTokenSource.Cancel();
        }
        public void InitializeIcdDict()
        {
            IcdDictionary.Add(IcdTypes.FlightBoxDownIcd, typeof(FlightBoxIcd));
            IcdDictionary.Add(IcdTypes.FlightBoxUpIcd, typeof(FlightBoxIcd));
            IcdDictionary.Add(IcdTypes.FiberBoxDownIcd, typeof(FiberBoxIcd));
            IcdDictionary.Add(IcdTypes.FiberBoxUpIcd, typeof(FiberBoxIcd));
        }
        public void ConnectTelemetry()
        {
            TelemetryConnection.Connect();
        }
        public void GetPacketData(GetSimulationDto getSimulationDto)
        {
            // parses the string to the enum equivlent if no enum exists with this name return
            if (!Enum.TryParse(getSimulationDto.IcdName, out IcdTypes icdType))
                return;

            string jsonText = File.ReadAllText(Consts.ICD_REPO_PATH + getSimulationDto.IcdName + Consts.ICD_FILE_TYPE);
            Type genericIcdType = typeof(IcdPacketGenerator<>).MakeGenericType(IcdDictionary[icdType]);
            dynamic icdInstance = Activator.CreateInstance(genericIcdType, jsonText);
            BitStreamControl(Consts.ZERO_ERROR_DELAY, Consts.ZERO_ERROR_DELAY, icdInstance, icdType);
        }

        public void GetPacketErrorData(GetErrorSimulationDto getSimulationErroDto)
        {
            if (!Enum.TryParse(getSimulationErroDto.IcdName, out IcdTypes icdType))
                return;

            string jsonText = File.ReadAllText(Consts.ICD_REPO_PATH + getSimulationErroDto.IcdName + Consts.ICD_FILE_TYPE);
            Type genericIcdType = typeof(IcdPacketGenerator<>).MakeGenericType(IcdDictionary[icdType]);
            dynamic icdInstance = Activator.CreateInstance(genericIcdType, jsonText);
            BitStreamControl(getSimulationErroDto.PacketDelayAmount, getSimulationErroDto.PacketNoiseAmount, icdInstance, icdType);
        }

        public void StopSimulator()
        {
            if (bitStreamCancelToken.IsCancellationRequested)
                return;
            bitStreamCancelTokenSource.Cancel();
        }

        private void BitStreamControl(int packetDelay, int packetNoise, dynamic icdInstance, IcdTypes icdType)
        {
            if (bitStreamCancelTokenSource.IsCancellationRequested)
            {
                bitStreamCancelTokenSource = new CancellationTokenSource();
                bitStreamCancelToken = bitStreamCancelTokenSource.Token;
            }
            else return;
            Task.Run(async () => { await SendBitStream(packetDelay, packetNoise, icdInstance, icdType, bitStreamCancelToken); }, bitStreamCancelToken);
        }

        private async Task SendBitStream(int packetDelay, int packetNoise, dynamic icdInstance, IcdTypes icdType, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    byte[] packetValue = await icdInstance.GeneratePacketBitData(packetDelay, packetNoise);
                    await TelemetryConnection.SendPacket(packetValue, icdType);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
    }
}
