using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server.Service
{
    static internal class SearchService
    {
        static internal void HandleSearchUser(Socket client, dynamic packet)
        {
            String name = packet.GetProperty("data").GetProperty("name").GetString()!;
            var table = MysqlService.ExecuteDataTable(
                "SELECT user_id, user_name FROM user_table WHERE user_name LIKE @name;",
                [
                    new MySql.Data.MySqlClient.MySqlParameter("@name", "%" + name + "%")
                ]
            );

            if (table != null && table.Rows.Count > 0)
            {
                List<Dictionary<String, Object>> users = new List<Dictionary<String, Object>>();
                foreach (System.Data.DataRow row in table.Rows)
                {

                }
                var responsePacket = new
                {
                    code = 0,  // 成功码
                    message = "查询成功",
                    data = new
                    {
                        count = users.Count,
                        users = users
                    }
                };
                NetworkService.Send(client, responsePacket);
            }
            else
            {
                var responsePacket = new
                {
                    
                };
                NetworkService.Send(client, responsePacket);
            }
        }
    }
}
