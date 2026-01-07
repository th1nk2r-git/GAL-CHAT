using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;

namespace Server.Service
{
    internal class Dispatcher
    {
        static internal String Dispatch(Socket client, String json)
        {
            var message = JsonSerializer.Deserialize<dynamic>(json)!;
            string type = message.GetProperty("type").GetString()!;
            String reply = "";
            switch (type)
            {
                case "login":
                    UserService.HandleUserLogin(client, json);
                    break;

                case "register":
                    UserService.HandleUserRegister(client, json);
                    break;

                default:
                    Console.WriteLine($"Unknown Message: {json}");
                    break;
            }
            return reply;
        }
    }
}
