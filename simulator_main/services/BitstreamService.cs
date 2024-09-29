using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using simulator_main;
using simulator_main.dtos;
using simulator_libary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using simulator_libary.icds;
using System.Threading;

namespace simulator_main.services
{
    public class BitstreamService : IBitstreamService
    {
        CancellationTokenSource bitStreamCancelTokenSource ;
        CancellationToken bitStreamCancelToken;
        private Dictionary<IcdTypes, Type> IcdDictionary;
        const string ICD_REPO_PATH = "./icd_repo/";
        const string ICD_FILE_TYPE = ".json";
        private SocketConnection TelemetryConnection;
        public BitstreamService(SocketConnection telemetryConnection)
        {
            TelemetryConnection = telemetryConnection;
            this.IcdDictionary = new Dictionary<IcdTypes, Type >();
            IcdDictionary.Add(IcdTypes.FlightBoxDownIcd,typeof(FlightBoxIcd));
            IcdDictionary.Add(IcdTypes.FlightBoxUpIcd, typeof(FlightBoxIcd));
            IcdDictionary.Add(IcdTypes.FiberBoxDownIcd, typeof(FiberBoxIcd));
            IcdDictionary.Add(IcdTypes.FiberBoxUpIcd, typeof(FiberBoxIcd));
            bitStreamCancelTokenSource = new CancellationTokenSource();
            bitStreamCancelToken = bitStreamCancelTokenSource.Token;
            bitStreamCancelTokenSource.Cancel();
        }
        public async Task ConnectTelemetry()
        {
            await TelemetryConnection.ConnectAsync();
        }
        public async Task GetPacketDataAsync(GetSimulationDto getSimulationDto)
        {
            // parses the string to the enum equivlent if no enum exists with this name return
            if (!Enum.TryParse(getSimulationDto.IcdName, out IcdTypes icdType))
                return;
            string jsonText = File.ReadAllText(ICD_REPO_PATH + getSimulationDto.IcdName + ICD_FILE_TYPE);


            Type genericIcdType = typeof(IcdPacketGenerator<>).MakeGenericType(IcdDictionary[icdType]);

            dynamic icdInstance = Activator.CreateInstance(genericIcdType);

            BitStreamControl(jsonText, 0, 0, icdInstance, icdType);
        }
        public async Task GetPacketErrorDataAsync(GetErrorSimulationDto getSimulationErroDto)
        {
           
            if (!Enum.TryParse(getSimulationErroDto.IcdName, out IcdTypes icdType))
                return;

            string jsonText = File.ReadAllText(ICD_REPO_PATH + getSimulationErroDto.IcdName + ICD_FILE_TYPE);

            Type genericIcdType = typeof(IcdPacketGenerator<>).MakeGenericType(IcdDictionary[icdType]);

            dynamic icdInstance = Activator.CreateInstance(genericIcdType);

            BitStreamControl(jsonText, getSimulationErroDto.PacketDelayAmount, getSimulationErroDto.PacketNoiseAmount, icdInstance, icdType);
        }
        private void BitStreamControl(string jsonText, int packetDelay,int packetNoise,dynamic icdInstance,IcdTypes icdType)
        {
            // if runnig stop, if stopped start sending
            if (bitStreamCancelTokenSource.IsCancellationRequested)
            {
                bitStreamCancelTokenSource = new CancellationTokenSource();
                bitStreamCancelToken = bitStreamCancelTokenSource.Token;
            }
            else
            {
                bitStreamCancelTokenSource.Cancel();
                return;
            }

            Task.Run(async () => { await SendBitStream(jsonText,packetDelay,packetNoise,icdInstance,icdType, bitStreamCancelToken); },bitStreamCancelToken);   
        }
        private async Task SendBitStream(string jsonText, int packetDelay, int packetNoise, dynamic icdInstance, IcdTypes icdType,CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                try
                {
                    byte[] packetValue = await icdInstance.GeneratePacketBitData(jsonText,packetDelay,packetNoise);
                    await TelemetryConnection.SendPacket(packetValue, icdType);
                }catch(Exception)
                {
                    break;
                }
            }
        }
    }
}
