using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Client.Service
{
    class Dispatcher
    {
        static public void Dispatch(String jsonString)
        {
            var message = JsonSerializer.Deserialize<dynamic>(jsonString)!;
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
