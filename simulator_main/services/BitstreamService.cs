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

namespace simulator_main.services
{
    public class BitstreamService : IBitstreamService
    {
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
            IcdDictionary.Add(IcdTypes.FiberBoxUpIcd, typeof(FiberBoxIcd)); ;
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

            byte[] packetValue = await icdInstance.GeneratePacketBitData(jsonText, 0, 0);

            await TelemetryConnection.SendPacket(packetValue, icdType);
        }
        public async Task GetPacketErrorDataAsync(GetErrorSimulationDto getSimulationErroDto)
        {
           
            if (!Enum.TryParse(getSimulationErroDto.IcdName, out IcdTypes icdType))
                return;

            string jsonText = File.ReadAllText(ICD_REPO_PATH + getSimulationErroDto.IcdName + ICD_FILE_TYPE);

            Type genericIcdType = typeof(IcdPacketGenerator<>).MakeGenericType(IcdDictionary[icdType]);

            dynamic icdInstance = Activator.CreateInstance(genericIcdType);

            byte[] packetValue = await icdInstance.GeneratePacketBitData(jsonText, getSimulationErroDto.PacketDelayAmount, getSimulationErroDto.PacketNoiseAmount);
            await TelemetryConnection.SendPacket(packetValue, icdType);
        }
    }
}
