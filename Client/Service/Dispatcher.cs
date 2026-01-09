using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Client.Service
{
    static internal class Dispatcher
    {
        static private String loginToken = "";

        static internal void Dispatch(dynamic packet)
        {
            string type = packet.GetProperty("type").GetString()!;
            string status = packet.GetProperty("status").GetString()!;
            switch (type)
            {
                case "login":
                    if (status == "success")
                    {
                        HandleLoginSuccess(packet);
                    }
                    else
                    {
                        HandleLoginFailure(packet);
                    }
                    break;

                case "register":
                    if (status == "success")
                    {
                        HandleRegisterSuccess(packet);
                    }
                    else
                    {
                        HandleRegisterFailure(packet);
                    }
                    break;
            }
        }

        // 处理登录成功响应
        static private void HandleLoginSuccess(dynamic packet)
        {
            String message = packet.GetProperty("data").GetProperty("message").GetString()!;
            loginToken = packet.GetProperty("data").GetProperty("token").GetString()!;
            Application.Current.Dispatcher.Invoke(() =>
            {
                HandyControl.Controls.Growl.Success(
                    new GrowlInfo
                    {
                        Message = message,
                        WaitTime = 1,
                    }
                );
                LoginWindow loginWindow = Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()!;
                var chatWindow = new ChatWindow();
                loginWindow.Close();
                chatWindow.Show();
            });
        }

        // 处理登录失败响应
        static private void HandleLoginFailure(dynamic packet)
        {
            String message = packet.GetProperty("data").GetProperty("message").GetString()!;
            Application.Current.Dispatcher.Invoke(() =>
            {
                HandyControl.Controls.Growl.Error(
                    new GrowlInfo
                    {
                        Message = message,
                        WaitTime = 1,
                        IsCustom = true
                    }
                );
            });
        }

        // 处理注册成功响应
        static private void HandleRegisterSuccess(dynamic packet)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                String message = packet.GetProperty("data").GetProperty("message").GetString()!;
                HandyControl.Controls.Growl.Success(
                    new GrowlInfo
                    {
                        Message = message,
                        WaitTime = 1,
                    }
                );
                RegisterWindow registerWindow = Application.Current.Windows.OfType<RegisterWindow>().FirstOrDefault()!;
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                registerWindow.Close();
            });
        }

        // 处理注册失败响应
        static private void HandleRegisterFailure(dynamic packet)
        {
            String message = packet.GetProperty("data").GetProperty("message").GetString()!;
            Application.Current.Dispatcher.Invoke(() =>
            {
                HandyControl.Controls.Growl.Error(
                    new GrowlInfo
                    {
                        Message = message,
                        WaitTime = 1,
                        IsCustom = true
                    }
                );
            });
        }
    }
}
