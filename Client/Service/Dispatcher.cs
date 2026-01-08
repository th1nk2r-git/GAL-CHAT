using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Client.Service
{
    static internal class Dispatcher
    {
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(
                    "喵",
                    "登录成功",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            });
        }

        // 处理登录失败响应
        static private void HandleLoginFailure(dynamic packet)
        {
            String message = packet.GetProperty("data").GetProperty("message").GetString()!;
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(
                    message, 
                    "登录失败", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error
                );
            });
        }

        // 处理注册成功响应
        static private void HandleRegisterSuccess(dynamic packet)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                String message = packet.GetProperty("data").GetProperty("message").GetString()!;
                MessageBox.Show(
                    message,
                    "注册成功",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
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
                MessageBox.Show(
                    message,
                    "注册失败",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            });
        }
    }
}
