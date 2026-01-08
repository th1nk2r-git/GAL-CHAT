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
using HandyControl.Data;

namespace Client
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            String inputID = UserIDTextBox.Text;
            String inputPasswd = PasswordTextBox.Password;

            if (inputID.IsWhiteSpace())
            {
                HandyControl.Controls.Growl.Error(
                    new GrowlInfo
                    {
                        Message = "用户ID不能为空",
                        WaitTime = 1,
                        IsCustom = true
                    }
                );
                return;
            }

            if (inputPasswd.IsWhiteSpace())
            {
                HandyControl.Controls.Growl.Error(
                    new GrowlInfo
                    {
                        Message = "密码不能为空",
                        WaitTime = 1,
                        IsCustom = true
                    }
                );
                return;
            }

            var packet = new
            {
                type = "login",
                data = new
                {
                    id = inputID,
                    password = inputPasswd
                }
            };
            NetworkService.Send(packet);
        }
    }
}
