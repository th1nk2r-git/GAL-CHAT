using Google.Protobuf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server.Service
{
    internal class UserService
    {
        // 生成随机字符串作为登录凭证
        static internal String GenerateLoginToken(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var tokenChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                tokenChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(tokenChars);
        }

        // 处理用户登录请求
        static internal void HandleUserLogin(Socket client, dynamic packet)
        {
            var data = packet.GetProperty("data");
            String id = data.GetProperty("id").GetString()!;
            String password = data.GetProperty("password").GetString()!;
            Console.WriteLine($"Login attempt: ID={id}, Password={password}");

            var table = MysqlService.ExecuteDataTable(
                "SELECT * FROM user_table WHERE user_id = @id AND user_password = @password;",
                [
                    new MySqlParameter("@id", id),
                    new MySqlParameter("@password", password)
                ]
            );

            // 用户登录成功, 发送登录成功响应
            if (table != null && table.Rows.Count > 0)
            {
                Console.WriteLine("Login succeed");
                var loginToken = ActiveUserManager.Instance.GetTokenByUserID(id);
                if (loginToken == "")
                {
                    loginToken = GenerateLoginToken(32);
                }

                ActiveUserManager.Instance.AddUserSession(
                    loginToken,
                    id,
                    DateTime.Now,
                    client
                );

                var successResponse = new
                {
                    type = "login",
                    status = "success",

                    data = new
                    {
                        token = loginToken,
                        id = table.Rows[0]["user_id"].ToString(),
                        name = table.Rows[0]["user_name"].ToString(),
                        message = "登录成功"
                    }
                };
                NetworkService.Send(client, successResponse);
                return;
            }

            // 用户登录失败，发送登录失败响应
            Console.WriteLine("Login failed: Wrong id or wrong password");
            var failureResponse = new
            {
                type = "login",
                status = "failure",
                data = new
                {
                    message = "用户名或密码错误"
                }
            };
            NetworkService.Send(client, failureResponse);
        }

        // 处理用户注册请求
        static internal void HandleUserRegister(Socket client, dynamic packet)
        {
            var data = packet.GetProperty("data");
            String id = data.GetProperty("id").GetString()!;
            String password = data.GetProperty("password").GetString()!;
            String name = data.GetProperty("name").GetString()!;
            String sex = data.GetProperty("sex").GetString()!;

            Console.WriteLine($"Register attempt: ID={id}, Name={name}, Password={password}");

            var table = MysqlService.ExecuteDataTable(
                "SELECT * FROM user_table WHERE user_id = @id;",
                [
                    new MySqlParameter("@id", id)
                ]
            );

            // 用户已存在，发送注册失败响应
            if (table != null && table.Rows.Count > 0)
            {
                Console.WriteLine("Register failed: ID has existed");
                var failureResponse = new
                {
                    type = "register",
                    status = "failure",
                    data = new
                    {
                        message = "用户ID已存在"
                    }
                };
                NetworkService.Send(client, failureResponse);
                return;
            }

            // 插入新用户数据
            int result = MysqlService.ExecuteNonQuery(
                "INSERT INTO user_table (user_id, user_name, user_password, user_sex)" +
                "VALUES (@id, @name, @password, @sex);",
                [
                    new MySqlParameter("@id", id),
                    new MySqlParameter("@name", name),
                    new MySqlParameter("@password", password),
                    new MySqlParameter("@sex", sex),
                ]
            );

            // 注册失败，发送注册失败响应
            if (result == -1)
            {
                Console.WriteLine("Register failed: Unknown Err");
                var failureResponse = new
                {
                    type = "register",
                    status = "failure",
                    data = new
                    {
                        message = "注册发生异常"
                    }
                };
                NetworkService.Send(client, failureResponse);
                return;
            }

            // 注册成功，发送注册成功响应
            Console.WriteLine("Register succeed");
            var successResponse = new
            {
                type = "register",
                status = "success",
                data = new
                {
                    message = " 点击确定返回"
                }
            };
            NetworkService.Send(client, successResponse);
        }
    }
}
