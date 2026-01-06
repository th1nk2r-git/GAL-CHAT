using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Client.Service
{
    internal class NetworkService
    {
        private readonly String _ip;
        private readonly int _port;

        private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Socket Socket { get { return _socket; } }

        static private readonly NetworkService _instance = new ();
        static public NetworkService Instance { get { return _instance; } }

        private NetworkService()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string configPath = Path.Combine(currentDirectory, "config.json");
            string jsonContent = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<dynamic>(jsonContent)!;
            _ip = config.GetProperty("ip").GetString();
            _port = config.GetProperty("port").GetInt32();
        }

        public bool Connect()
        {
            try
            {
                _socket.Connect(new IPEndPoint(IPAddress.Parse(_ip), _port));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Send(String json)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(json);
                byte[] lengthPrefix = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length));

                byte[] packet = new byte[4 + data.Length];
                Buffer.BlockCopy(lengthPrefix, 0, packet, 0, 4);
                Buffer.BlockCopy(data, 0, packet, 4, data.Length);

                _socket.Send(packet);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("与服务器的连接已断开。\n" + ex.Message, "网络错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                });
            }
        }
        public void Close()
        {
            _socket.Close();
        }
    }
}
