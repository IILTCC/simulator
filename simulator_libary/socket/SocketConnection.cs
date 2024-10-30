using Microsoft.Extensions.Configuration;
using simulator_libary.icds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace simulator_libary
{
    public class SocketConnection
    {
        const int PACKET_HEADER_SIZE = 27;
        UdpClient telemetryDevice;
        private readonly SimulatorSettings _simulatorSettings;
        public SocketConnection(SimulatorSettings simulatorSettings)
        {
            _simulatorSettings = simulatorSettings;
        }
        public void ConnectAsync()
        {
            byte[] ipBytes = new byte[4];
            for (int i = 0; i < 4; i++)
                ipBytes[i] = (byte)Int32.Parse(_simulatorSettings.DefaultGatewayIp.Split(".")[i]);
            IPAddress ipAddr = new IPAddress(ipBytes);

            telemetryDevice = new UdpClient(AddressFamily.InterNetwork);
            telemetryDevice.Connect(ipAddr, _simulatorSettings.SimulatorPort);
        }
        public async Task SendPacket(byte[]packet,IcdTypes type)
        {
            byte[] finalPacket = new byte[PACKET_HEADER_SIZE + packet.Length];

            byte[] packetSize = BitConverter.GetBytes(packet.Length);
            byte[] packetType = BitConverter.GetBytes((int)(type));

            string timestamp = DateTime.Now.ToString("dd,MM,yyyy,HH,mm,ss,ffff");
            byte[] timestampBytes = Encoding.ASCII.GetBytes(timestamp);

            // initial packet data information 0,1 location - size and 2 - type
            finalPacket[0] = packetSize[0];
            finalPacket[1] = packetSize[1];
            finalPacket[2] = packetType[0];
            for (int i = 0; i < timestampBytes.Length; i++)
                finalPacket[i+3] = timestampBytes[i];
            for (int i = 0; i < packet.Length; i++)
                finalPacket[i + PACKET_HEADER_SIZE] = packet[i];

            await telemetryDevice.SendAsync(finalPacket, finalPacket.Length);
        }

    }
}
