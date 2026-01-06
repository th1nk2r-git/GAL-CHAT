using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Net;
using Client.Service;

namespace Client
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        [GeneratedRegex("^[a-zA-Z0-9_]{5,15}$")]
        private static partial Regex UserIdRegex();

        [GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{7,15}$")]
        private static partial Regex PasswordRegex();

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            String inputID = UserIDTextBox.Text;
            String inputPasswd = PasswordTextBox.Password;

            if (!UserIdRegex().IsMatch(inputID))
            {
                MessageBox.Show("用户ID为大小写英文字符、数字、下划线的任意组合\n" +
                                "用户ID的长度不少于5且不超过15",
                                "格式错误",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (!PasswordRegex().IsMatch(inputPasswd))
            {
                MessageBox.Show("格式错误\n" +
                                "密码必须包含大小写英文字符、数字\n" +
                                "密码的长度不少于7且不超过15",
                                "格式错误",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            var jsonObject = new
            {
                type = "login",
                data = new
                {
                    userID = inputID,
                    password = inputPasswd
                }
            };
            String json = JsonSerializer.Serialize(jsonObject);
            NetworkService.Instance.Send(json);
        }
    }
}
