using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;

namespace Client.Service
{
    static internal class NetworkService
    {
        static private readonly String _ip = "127.0.0.1";
        static private readonly int _port = 1234;

        static private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static internal Socket Socket { get { return _socket; } }

        static private readonly JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };
        static internal JsonSerializerOptions Options { get { return options; } }

        static internal bool Connect()
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

        static internal void Send(dynamic packet)
        {
            string json = JsonSerializer.Serialize(packet, options);
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(json);
                byte[] lengthPrefix = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length));

                byte[] buffer = new byte[4 + data.Length];
                Buffer.BlockCopy(lengthPrefix, 0, buffer, 0, 4);
                Buffer.BlockCopy(data, 0, buffer, 4, data.Length);

                _socket.Send(buffer);
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
        static internal void Close()
        {
            _socket.Close();
        }
    }
}
