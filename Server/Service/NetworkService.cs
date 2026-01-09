using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Server.Service
{
    static internal class NetworkService
    {

        static private readonly JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };
        static internal JsonSerializerOptions Options { get { return options; } }

        // 发送消息到指定Socket
        static internal void Send(Socket socket, dynamic packet)
        {
            String json = JsonSerializer.Serialize(packet, options);
            Console.WriteLine("Sending packet: " + json);
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(json);
                byte[] lengthPrefix = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length));

                byte[] buffer = new byte[4 + data.Length];
                Buffer.BlockCopy(lengthPrefix, 0, buffer, 0, 4);
                Buffer.BlockCopy(data, 0, buffer, 4, data.Length);

                socket.Send(buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to Send packet: " + ex.Message);
            }
        }
    }
}
