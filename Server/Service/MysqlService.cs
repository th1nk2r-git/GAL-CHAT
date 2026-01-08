using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Server.Service
{
    static internal class MysqlService
    {

        static private MySqlConnection connection = new ();

        // 连接到Mysql数据库
        static internal void ConnectToDB(String host, String user, String database, String port, String password)
        {
            string connectionString = $"server={host};user={user};database={database};port={port};password={password}";
            connection = new MySqlConnection(connectionString);
            connection.Open ();
        }

        // 执行非查询SQL语句，返回受到影响的行数
        static internal int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            try
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to execute sql: {ex.Message}");
                return -1;
            }
        }

        // 执行查询SQL语句，返回标量值
        static internal object? ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            try
            {
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to execute sql: {ex.Message}");
                return null;
            }
        }

        // 执行查询SQL语句，返回表格
        static internal DataTable ExecuteDataTable(string sql, params MySqlParameter[] parameters)
        {
            var dataTable = new DataTable();
            try
            {
                using var command = new MySqlCommand(sql, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                using var adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to execute sql: {ex.Message}");
            }

            return dataTable;
        }
    }
}
