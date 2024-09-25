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
        private Dictionary<string, (Type,IcdTypes)> IcdDictionary;
        const string ICD_REPO_PATH = "./icd_repo/";
        const string ICD_FILE_TYPE = ".json";
        private SocketConnection TelemetryConnection;
        public BitstreamService(SocketConnection telemetryConnection)
        {
            TelemetryConnection = telemetryConnection;
            this.IcdDictionary = new Dictionary<string, (Type,IcdTypes)>();
            IcdDictionary.Add("FlightBoxDownIcd", (typeof(FlightBoxIcd),IcdTypes.FlightBoxDown));
            IcdDictionary.Add("FlightBoxUpIcd", (typeof(FlightBoxIcd), IcdTypes.FlightBoxUp));
            IcdDictionary.Add("FiberBoxDownIcd", (typeof(FiberBoxIcd), IcdTypes.FiberBoxDown));
            IcdDictionary.Add("FiberBoxUpIcd", (typeof(FiberBoxIcd), IcdTypes.FiberBoxUp));
        }
        public async Task GetPacketDataAsync(GetSimulationDto getSimulationDto)
        {
            if (!IcdDictionary.ContainsKey(getSimulationDto.IcdName))
                return ;

            string jsonText = File.ReadAllText(ICD_REPO_PATH + getSimulationDto.IcdName + ICD_FILE_TYPE);

            
            Type genericIcdType = typeof(IcdPacketGenerator<>).MakeGenericType(IcdDictionary[getSimulationDto.IcdName].Item1);

            dynamic icdInstance = Activator.CreateInstance(genericIcdType);
            for(int i =0;i<100;i++)
            {

            byte[]packetValue = await icdInstance.GeneratePacketBitData(jsonText,0,0);
            await TelemetryConnection.SendPacket(packetValue,IcdDictionary[getSimulationDto.IcdName].Item2);
            }

        }
        public async Task GetPacketErrorDataAsync(GetErrorSimulationDto getSimulationErroDto)
        {
            if (!IcdDictionary.ContainsKey(getSimulationErroDto.IcdName))
                return ;
            
            string jsonText = File.ReadAllText(ICD_REPO_PATH + getSimulationErroDto.IcdName + ICD_FILE_TYPE);

            Type genericIcdType = typeof(IcdPacketGenerator<>).MakeGenericType(IcdDictionary[getSimulationErroDto.IcdName].Item1);

            dynamic icdInstance = Activator.CreateInstance(genericIcdType);

            byte[] packetValue= await icdInstance.GeneratePacketBitData(jsonText,getSimulationErroDto.PacketDelayAmount,getSimulationErroDto.PacketNoiseAmount);
            await TelemetryConnection.SendPacket(packetValue, IcdDictionary[getSimulationErroDto.IcdName].Item2);
        }
    }
}
