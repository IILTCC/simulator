using simulator_libary.icds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace simulator_libary
{
    public class SocketConnection
    {
        const int PACKET_HEADER_SIZE = 3;
        const int PORT = 50000;
        UdpClient telemetryDevice;

        public  void ConnectAsync()
        {
            IPAddress ipAddr = new IPAddress(new byte[] { 192,168,33,241});

            telemetryDevice = new UdpClient(AddressFamily.InterNetwork);
            telemetryDevice.Connect(ipAddr, PORT);
        }
        public async Task SendPacket(byte[]packet,IcdTypes type)
        {
            byte[] finalPacket = new byte[PACKET_HEADER_SIZE + packet.Length];

            byte[] packetSize = BitConverter.GetBytes(packet.Length);
            byte[] packetType = BitConverter.GetBytes((int)(type));

            // initial packet data information 0,1 location - size and 2 - type
            finalPacket[0] = packetSize[0];
            finalPacket[1] = packetSize[1];
            finalPacket[2] = packetType[0];

            for (int i = 0; i < packet.Length; i++)
                finalPacket[i + PACKET_HEADER_SIZE] = packet[i];

            await telemetryDevice.SendAsync(finalPacket, finalPacket.Length);
        }

    }
}
