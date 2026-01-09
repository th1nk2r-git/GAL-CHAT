using Client.Service;
using HandyControl.Data;
using System.Text.RegularExpressions;
using System.Windows;


namespace Client
{
    public partial class RegisterWindow : System.Windows.Window
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
                HandyControl.Controls.Growl.Error(
                    new GrowlInfo
                    {
                        Message = "用户ID为大小写英文字符、数字、下划线的任意组合\n" +
                                  "用户ID的长度不少于5且不超过15",

                        WaitTime = 1,
                        IsCustom = true
                    }
                );
                return;
            }

            if (!PasswordRegex().IsMatch(inputPasswd))
            {
                HandyControl.Controls.Growl.Error(
                    new GrowlInfo
                    {
                        Message = "密码必须包含大小写英文字符、数字\n" +
                                  "密码的长度不少于7且不超过15",

                        WaitTime = 1,
                        IsCustom = true
                    }
                );
                return;
            }

            if (!UserNameRegex().IsMatch(inputName))
            {
                HandyControl.Controls.Growl.Error(
                    new GrowlInfo
                    {
                        Message = "昵称不能包含除大小写英文字符、数字、下划线、中文以外的字符\n" +
                                  "昵称的长度不能超过15且昵称不能为空",

                        WaitTime = 1,
                        IsCustom = true
                    }
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
