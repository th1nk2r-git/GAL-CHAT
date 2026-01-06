using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {

        async static Task HandleClientAsync(Socket client)
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
                        if (dataLength <= 0 || dataLength > 1024 * 1024) // 添加合理的最大值限制
                        {
                            Console.WriteLine($"Invalid message length: {dataLength}");
                            return;
                        }

                        byte[] dataBuffer = new byte[dataLength];
                        totalRead = 0;
                        while (totalRead < dataLength)
                        {
                            int read = await client.ReceiveAsync(dataBuffer.AsMemory(totalRead, dataLength - totalRead), SocketFlags.None);
                            totalRead += read;
                        }
                        String message = Encoding.UTF8.GetString(dataBuffer);



                        Console.WriteLine($"Received message: {message}");
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

        static async Task Main(String[] args)
        {
            // 初始化服务器参数
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ushort port = 1234;
            if(args.Length == 1)
            {
                if(!ushort.TryParse(args[0], out port))
                {
                    Console.WriteLine("Invalid port number.");
                    return;
                }
            }
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: Server [port]");
                return;
            }

            // 启动服务器
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

            // 用于停止服务器的令牌
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
