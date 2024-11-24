using simulator_libary.icds;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace simulator_libary
{
    public class SocketConnection
    {
        private Dictionary<IcdTypes, UdpClient> _networkCards;
        private readonly SimulatorSettings _simulatorSettings;
        public SocketConnection(SimulatorSettings simulatorSettings)
        {
            _simulatorSettings = simulatorSettings;
            _networkCards = new Dictionary<IcdTypes, UdpClient>();
        }

        public void Connect()
        {
            byte[] ipBytes = new byte[Consts.IP_SIZE];
            for (int ipIndex = 0; ipIndex < Consts.IP_SIZE; ipIndex++)
                ipBytes[ipIndex] = (byte)int.Parse(_simulatorSettings.DefaultGatewayIp.Split(".")[ipIndex]);
            IPAddress ipAddr = new IPAddress(ipBytes);

            foreach(IcdTypes icdType in Enum.GetValues(typeof(IcdTypes)))
            {
                UdpClient networkCard = new UdpClient(AddressFamily.InterNetwork);
                networkCard.Connect(ipAddr, _simulatorSettings.SimulatorPort + (int)icdType);
                _networkCards.Add(icdType,networkCard);
            }
        }

        public async Task SendPacket(byte[] packet, IcdTypes type)
        {
            byte[] finalPacket = new byte[Consts.PACKET_HEADER_SIZE + packet.Length];
            byte[] packetType = new byte[Consts.TYPE_SIZE] { (byte)((int)(type)) };
            string timestamp = DateTime.Now.ToString(Consts.TIMESTAMP_FORMAT);
            byte[] timestampBytes = Encoding.ASCII.GetBytes(timestamp);

            List<byte[]> packetParameters = new List<byte[]>() { packetType,timestampBytes,packet};
            int packetOffset = 0;
            foreach(byte[] parameter in packetParameters)
            {
                for (int paramIndex = 0; paramIndex < parameter.Length; paramIndex++)
                    finalPacket[packetOffset++] = parameter[paramIndex];
            }

            await _networkCards[type].SendAsync(finalPacket,finalPacket.Length);
        }
    }
}
