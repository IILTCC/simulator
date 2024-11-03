using simulator_libary.icds;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace simulator_libary
{
    public class SocketConnection
    {
        private const int PACKET_HEADER_SIZE = 25;
        private const string TIMESTAMP_FORMAT = "dd,MM,yyyy,HH,mm,ss,ffff";

        private UdpClient networkCard;
                private readonly SimulatorSettings _simulatorSettings;
        public SocketConnection(SimulatorSettings simulatorSettings)
        {
            _simulatorSettings = simulatorSettings;
        }

        public void Connect()
        {
            byte[] ipBytes = new byte[4];
            for (int ipIndex = 0; ipIndex < 4; ipIndex++)
                ipBytes[ipIndex] = (byte)int.Parse(_simulatorSettings.DefaultGatewayIp.Split(".")[ipIndex]);
            IPAddress ipAddr = new IPAddress(ipBytes);

            networkCard = new UdpClient(AddressFamily.InterNetwork);
            networkCard.Connect(ipAddr, _simulatorSettings.SimulatorPort);
        }

        public async Task SendPacket(byte[]packet,IcdTypes type)
        {
            byte[] finalPacket = new byte[PACKET_HEADER_SIZE + packet.Length];
            byte[] packetType = BitConverter.GetBytes((int)(type));
            string timestamp = DateTime.Now.ToString(TIMESTAMP_FORMAT);
            byte[] timestampBytes = Encoding.ASCII.GetBytes(timestamp);

            int headerOffset = 0;
            // initial packet data information 0,1 - size and 2 - type
            finalPacket[headerOffset++] = packetType[0];
            for (int timeIndex = 0; timeIndex < timestampBytes.Length; timeIndex++)
                finalPacket[timeIndex + headerOffset] = timestampBytes[timeIndex];

            for (int packetIndex = 0; packetIndex < packet.Length; packetIndex++)
                finalPacket[packetIndex + PACKET_HEADER_SIZE] = packet[packetIndex];

            await networkCard.SendAsync(finalPacket, finalPacket.Length);
        }
    }
}
