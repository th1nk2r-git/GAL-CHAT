using Client.Service;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        [GeneratedRegex("^[a-zA-Z0-9_]{5,15}$")]
        private static partial Regex UserIdRegex();

        [GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{7,15}$")]
        private static partial Regex PasswordRegex();

        [GeneratedRegex(@"^[a-zA-Z0-9_\u4e00-\u9fa5]{1,15}$")]
        private static partial Regex UserNameRegex();

        private void ComfirmButton_Click(object sender, RoutedEventArgs e)
        {
            String inputID = UserIDTextBox.Text;
            String inputPasswd = PasswordTextBox.Text;
            String inputName = UserNameTextBox.Text;
            String inputSex = SexComboBox.Text;

            if (!UserIdRegex().IsMatch(inputID))
            {
                MessageBox.Show(
                    "用户ID为大小写英文字符、数字、下划线的任意组合\n" +
                    "用户ID的长度不少于5且不超过15",
                    "格式错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            if (!PasswordRegex().IsMatch(inputPasswd))
            {
                MessageBox.Show(
                    "密码必须包含大小写英文字符、数字\n" +
                    "密码的长度不少于7且不超过15",
                    "格式错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            if (!UserNameRegex().IsMatch(inputName))
            {
                MessageBox.Show(
                    "昵称不能包含除大小写英文字符、数字、下划线、中文以外的字符\n"+
                    "昵称的长度不能超过15且昵称不能为空",
                    "格式错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }

            var packet = new
            {
                type = "register",
                data = new
                {
                    id = inputID,
                    password = inputPasswd,
                    name = inputName,
                    sex = inputSex
                }
            };
            NetworkService.Send(packet);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            
            loginWindow.Show();
            this.Close();
        }
    }
}
