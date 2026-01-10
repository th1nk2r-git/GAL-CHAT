using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class AddGroupWindow : Window
    {
        public AddGroupWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var chatWindow = new ChatWindow();
            chatWindow.Show();
            this.Close();
        }
    }
}