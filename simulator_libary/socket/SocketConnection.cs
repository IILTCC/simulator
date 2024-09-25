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
        TcpClient telemetryDevice;
        NetworkStream stream;

        public async Task ConnectAsync()
        {
            Console.WriteLine("-----------");
            IPAddress ipAddr = new IPAddress(new byte[] { 127, 0, 0, 1 });

            telemetryDevice = new TcpClient(AddressFamily.InterNetwork);
            await telemetryDevice.ConnectAsync(ipAddr,PORT);
            stream = telemetryDevice.GetStream();
        }
        public async Task SendPacket(byte[]packet,IcdTypes type)
        {
            Console.WriteLine("sent packet with length "+packet.Length);
            byte[] packetData = new byte[3];
            byte[] packetSize = BitConverter.GetBytes(packet.Length);
            byte[] packetType = BitConverter.GetBytes((int)(type));
            packetData[0] = packetSize[0];
            packetData[1] = packetSize[1];
            packetData[2] = packetType[0];
            Console.WriteLine("packet data data"+packetData[0]+packetData[1]+packetData[2]);
            await stream.WriteAsync(packetData);
            await stream.WriteAsync(packet);
        }

    }
}
