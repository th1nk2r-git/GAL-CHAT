using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Server.Service
{
    static internal class NetworkService
    {
        static internal void Send(Socket socket, String json)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(json);
                byte[] lengthPrefix = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length));

                byte[] packet = new byte[4 + data.Length];
                Buffer.BlockCopy(lengthPrefix, 0, packet, 0, 4);
                Buffer.BlockCopy(data, 0, packet, 4, data.Length);

                socket.Send(packet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to Send Message: " + ex.Message);
            }
        }
    }
}
