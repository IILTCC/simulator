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
        private UdpClient networkCard;
                private readonly SimulatorSettings _simulatorSettings;
        public SocketConnection(SimulatorSettings simulatorSettings)
        {
            _simulatorSettings = simulatorSettings;
        }

        public void Connect()
        {
            byte[] ipBytes = new byte[Consts.IP_SIZE];
            for (int ipIndex = 0; ipIndex < Consts.IP_SIZE; ipIndex++)
                ipBytes[ipIndex] = (byte)int.Parse(_simulatorSettings.DefaultGatewayIp.Split(".")[ipIndex]);
            IPAddress ipAddr = new IPAddress(ipBytes);

            networkCard = new UdpClient(AddressFamily.InterNetwork);
            networkCard.Connect(ipAddr, _simulatorSettings.SimulatorPort);
        }

        public async Task SendPacket(byte[]packet,IcdTypes type)
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

            await networkCard.SendAsync(finalPacket, finalPacket.Length);
        }
    }
}
