using Client;
using Client.Service;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace client
{
    public partial class App : Application
    {
        private void Main(object sender, StartupEventArgs e)
        {
            if (NetworkService.Instance.Connect() == false)
            {
                MessageBox.Show("无法连接到服务器", "连接错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Task.Run(() =>
            {
                Socket socket = NetworkService.Instance.Socket;
                while (true)
                {
                    byte[] lengthBuffer = new byte[4];
                    int totalRead = 0;
                    while (totalRead < 4)
                    {
                        int read = socket.Receive(lengthBuffer, totalRead, 4 - totalRead, SocketFlags.None);
                        if (read == 0)
                        {
                            throw new SocketException((int)SocketError.ConnectionReset);
                        }
                        totalRead += read;
                    }

                    int dataLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBuffer, 0));

                    byte[] dataBuffer = new byte[dataLength];
                    totalRead = 0;
                    while (totalRead < dataLength)
                    {
                        int read = socket.Receive(dataBuffer, totalRead, dataLength - totalRead, SocketFlags.None);
                        totalRead += read;
                    }

                    String json =  Encoding.UTF8.GetString(dataBuffer);
                    Client.Service.Dispatcher.Dispatch(json);
                }
            });
            var app = new App();
            var loginWindow = new LoginWindow();
            app.Run(loginWindow);
        }
    }
}