using MySql.Data;
using MySql.Data.MySqlClient;
using Server.Service;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server
{
    internal class Program
    {
        // 异步处理客户端的请求
        async static private Task HandleClientAsync(Socket client)
        {
            try
            {
                using (client)
                {
                    while (true)
                    {
                        byte[] lengthBuffer = new byte[4];
                        int totalRead = 0;
                        while (totalRead < 4)
                        {
                            int read = await client.ReceiveAsync(lengthBuffer.AsMemory(totalRead, 4 - totalRead), SocketFlags.None);
                            if (read == 0)
                            {
                                throw new SocketException((int)SocketError.ConnectionReset);
                            }
                            totalRead += read;
                        }

                        int dataLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBuffer, 0));
                        if (dataLength <= 0 || dataLength > 1024 * 1024)
                        {
                            Console.WriteLine($"Invalid packet length: {dataLength}");
                            return;
                        }

                        byte[] buffer = new byte[dataLength];
                        totalRead = 0;
                        while (totalRead < dataLength)
                        {
                            int read = await client.ReceiveAsync(buffer.AsMemory(totalRead, dataLength - totalRead), SocketFlags.None);
                            totalRead += read;
                        }
                        String json = Encoding.UTF8.GetString(buffer);
                        Console.WriteLine($"Received packet: {json}");

                        var packet = JsonSerializer.Deserialize<dynamic>(json)!;
                        Dispatcher.Dispatch(client, packet);
                    }
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
            {
                Console.WriteLine($"Client disconnected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                try { client?.Shutdown(SocketShutdown.Both); }
                catch { }
                client?.Close();
            }
        }
    

        static async Task Main()
        {
            // 连接Mysql数据库
            try
            {
                MysqlService.ConnectToDB("180.160.173.240", "241310422", "GALCHAT", "3306", "241310422");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to Mysql: " + ex.Message);
                return;
            }
            Console.WriteLine("Connected to Mysql");

            // 启动服务器
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ushort port = 1234;
            try
            {
                socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, port));
                socket.Listen();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to start server: " + ex.Message);
                return;
            }
            Console.WriteLine("Server is running on " + port + "……");

            // 设置停止服务器的令牌
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            // 轮询接受客户端连接
            while (!cts.Token.IsCancellationRequested)
            {
                Socket client = await socket.AcceptAsync(cts.Token);

                Console.WriteLine("Client connected: " + client.RemoteEndPoint?.ToString());

                await HandleClientAsync(client);
            }
            socket.Close();
        }
    }
}
