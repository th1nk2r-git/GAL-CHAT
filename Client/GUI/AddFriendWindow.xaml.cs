using Client.Service;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class AddFriendWindow : Window
    {
        public AddFriendWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var chatWindow = new ChatWindow();
            chatWindow.Show();
            this.Close();
        }

        private void btnSearchUser_Click(object sender, RoutedEventArgs e)
        {
            String searchName = txtSearchUser.Text;
            var packet = new
            {
                type = "search_user",
                data = new {
                    name = searchName
                }
            };
            NetworkService.Send(packet);
        }
    }
}