using simulator_libary;
using simulator_libary.Enums;
using simulator_libary.generator;
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
        private readonly Dictionary<IcdTypes, Channel> _icdDictionary;
        private readonly SocketConnection _telemetryConnection;
        public BitstreamService(SocketConnection telemetryConnection)
        {
            _telemetryConnection = telemetryConnection;
            _icdDictionary = new Dictionary<IcdTypes, Channel>();
            InitializeIcdDict();
        }
        public void InitializeIcdDict()
        {
            string FlightBoxUpJson = File.ReadAllText(Consts.ICD_REPO_PATH + IcdTypes.FlightBoxUpIcd.ToString() + Consts.ICD_FILE_TYPE);
            string FlightBoxDownJson = File.ReadAllText(Consts.ICD_REPO_PATH + IcdTypes.FlightBoxDownIcd.ToString() + Consts.ICD_FILE_TYPE);
            string FiberBoxUpJson = File.ReadAllText(Consts.ICD_REPO_PATH + IcdTypes.FiberBoxUpIcd.ToString() + Consts.ICD_FILE_TYPE);            
            string FiberBoxDownJson = File.ReadAllText(Consts.ICD_REPO_PATH + IcdTypes.FiberBoxDownIcd.ToString() + Consts.ICD_FILE_TYPE);

            IGeneratePacketBit FiberBoxUpGenerator = new FiberBoxPacketGenerator<FiberBoxIcd>(FiberBoxUpJson);
            IGeneratePacketBit FiberBoxDownGenerator = new FiberBoxPacketGenerator<FiberBoxIcd>(FiberBoxDownJson);
            IGeneratePacketBit FlightBoxUpGenerator = new FlightBoxPacketGenerator<FlightBoxIcd>(FlightBoxUpJson);
            IGeneratePacketBit FlightBoxDownGenerator = new FlightBoxPacketGenerator<FlightBoxIcd>(FlightBoxDownJson);

            _icdDictionary.Add(IcdTypes.FiberBoxDownIcd,new Channel(FiberBoxDownGenerator));
            _icdDictionary.Add(IcdTypes.FiberBoxUpIcd,new Channel(FiberBoxUpGenerator));
            _icdDictionary.Add(IcdTypes.FlightBoxDownIcd,new Channel(FlightBoxDownGenerator));
            _icdDictionary.Add(IcdTypes.FlightBoxUpIcd,new Channel(FlightBoxUpGenerator));

            foreach(Channel channel in _icdDictionary.Values)
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                channel.ChannelTokenSource = cts;
                channel.ChannelToken = cts.Token;
                cts.Cancel();
            }
        }
        public void ConnectTelemetry()
        {
            _telemetryConnection.Connect();
        }
        public ReturnStatus GetPacketData(GetSimulationDto getSimulationDto)
        {
            return BitStreamControl(Consts.ZERO_ERROR_DELAY, Consts.ZERO_ERROR_DELAY, _icdDictionary[getSimulationDto.IcdType], getSimulationDto.IcdType);
        }

        public ReturnStatus GetPacketErrorData(GetErrorSimulationDto getSimulationErroDto)
        {
            return BitStreamControl(getSimulationErroDto.PacketDelayAmount, getSimulationErroDto.PacketNoiseAmount, _icdDictionary[getSimulationErroDto.IcdType],getSimulationErroDto.IcdType);
        }

        public ReturnStatus StopSimulator(StopSimulatorDto stopSimulatorDto)
        {
            if (_icdDictionary[stopSimulatorDto.IcdType].ChannelToken.IsCancellationRequested)
                return ReturnStatus.AreadyStopped;
            _icdDictionary[stopSimulatorDto.IcdType].ChannelTokenSource.Cancel();
            return ReturnStatus.Succes;
        }

        private ReturnStatus BitStreamControl(int packetDelay, int packetNoise,Channel channel, IcdTypes icdType)
        {
            if (channel.ChannelToken.IsCancellationRequested)
            {
                channel.ChannelTokenSource = new CancellationTokenSource();
                channel.ChannelToken = channel.ChannelTokenSource.Token;
            }
            else return ReturnStatus.AlreadyRunning;

            Task.Run(async () => { await SendBitStream(packetDelay, packetNoise, channel, icdType); }, channel.ChannelToken);
            return ReturnStatus.Succes;
        }

        private async Task SendBitStream(int packetDelay, int packetNoise, Channel channel, IcdTypes icdType)
        {
            while (!channel.ChannelToken.IsCancellationRequested)
            {
                try
                {
                    byte[] packetValue = await channel.PacketGenerator.GeneratePacketBitData(packetDelay, packetNoise);
                    await _telemetryConnection.SendPacket(packetValue, icdType);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
    }
}
