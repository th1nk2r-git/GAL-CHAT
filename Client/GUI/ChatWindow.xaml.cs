// ChatWindow.xaml.cs
using Client.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Client
{
    public partial class ChatWindow : Window
    {
        public ObservableCollection<MessageModel> Messages { get; set; }

        public ChatWindow()
        {
            InitializeComponent();

            Messages = new ObservableCollection<MessageModel>();

            lstMessages.ItemsSource = Messages;
        }

        public void AddMessage(string sender, string content, bool isSelf)
        {
            var message = new MessageModel
            {
                Sender = sender,
                Content = content,
                Timestamp = DateTime.Now,
                IsSelf = isSelf
            };

            Dispatcher.Invoke(() =>
            {
                Messages.Add(message);
                lstMessages.ScrollIntoView(message);
            });
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string messageText = txtMessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(messageText))
            {
                AddMessage("我", messageText, true);

                txtMessageInput.Text = "";

            }
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var addGroupWindow = new AddGroupWindow();
            addGroupWindow.Show();
            this.Close();
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            var addFriendWindow = new AddFriendWindow();
            addFriendWindow.Show();
            this.Close();
        }
    }
}