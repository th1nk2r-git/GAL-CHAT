using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace Client.Models
{
    public class UserSearchResult : INotifyPropertyChanged
    {
        private string _buttonText;
        private Brush _buttonColor;

        public string UserId { get; set; }
        public string Name { get; set; }
        public string Signature { get; set; }

        // 在线状态相关
        public string StatusText { get; set; }
        public Brush StatusColor { get; set; }

        // 按钮状态
        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                OnPropertyChanged();
            }
        }

        public Brush ButtonColor
        {
            get => _buttonColor;
            set
            {
                _buttonColor = value;
                OnPropertyChanged();
            }
        }

        // 可以添加头像属性，如果没有头像，使用默认样式
        public string AvatarInitials => Name?.Length > 0 ? Name[0].ToString().ToUpper() : "U";

        // 为了方便，添加命令属性（如果使用MVVM模式）
        public ICommand AddFriendCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}