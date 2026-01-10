using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server.Service
{
    internal class Dispatcher
    {
        // 将消息分发到相应的服务处理函数
        static internal void Dispatch(Socket client, dynamic packet)
        {
            string type = packet.GetProperty("type").GetString()!;
            switch (type)
            {
                case "login":
                    UserService.HandleUserLogin(client, packet);
                    break;

                case "register":
                    UserService.HandleUserRegister(client, packet);
                    break;

                case "search_user":
                    SearchService.HandleSearchUser(client, packet);
                    break;

                case "get_friend_list":

                    break;

                case "get_group_list":

                    break;

                default:
                    var json = JsonSerializer.Serialize(packet, NetworkService.Options);
                    Console.WriteLine($"Unknown packet: {json}");
                    break;
            }
        }
    }
}
