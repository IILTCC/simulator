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
            // initial packet data information 0,1 location - size and 2 - type
            byte[] packetData = new byte[3];

            byte[] packetSize = BitConverter.GetBytes(packet.Length);
            byte[] packetType = BitConverter.GetBytes((int)(type));

            packetData[0] = packetSize[0];
            packetData[1] = packetSize[1];
            packetData[2] = packetType[0];

            await telemetryDevice.SendAsync(packetData,packetData.Length);
            await telemetryDevice.SendAsync(packet,packet.Length);
        }

    }
}
