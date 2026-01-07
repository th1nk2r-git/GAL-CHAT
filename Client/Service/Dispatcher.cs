using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Client.Service
{
    static internal class Dispatcher
    {
        static internal void Dispatch(String json)
        {
            var message = JsonSerializer.Deserialize<dynamic>(json)!;
            string type = message.GetProperty("type").GetString()!;
            string status = message.GetProperty("status").GetString()!;
            switch (type)
            {
                case "login":
                    if (status == "success")
                    {

                    }
                    else
                    {

                    }
                    break;
                case "register":
                    if (status == "success")
                    {

                    }
                    else
                    {

                    }
                    break;
                default:

                    break;
            }
        }
    }
}
