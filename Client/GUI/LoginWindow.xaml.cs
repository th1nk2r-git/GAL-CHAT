using System.Windows;
using Client.Service;
using HandyControl.Data;

namespace Client
{
    public partial class LoginWindow : System.Windows.Window
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
