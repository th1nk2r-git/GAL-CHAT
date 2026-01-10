// MessageModel.cs
using System;

namespace Client.Models
{
    public class MessageModel
    {
        private string _sender = "";
        public string Sender
        {
            get => _sender;
            set => _sender = string.IsNullOrWhiteSpace(value) ? "未知用户" : value;
        }

        private string _content = "";
        public string Content
        {
            get => _content;
            set => _content = string.IsNullOrWhiteSpace(value) ? "[空消息]" : value;
        }

        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get => _timestamp;
            set => _timestamp = value == default ? DateTime.Now : value;
        }

        private bool _isSelf;
        public bool IsSelf
        {
            get => _isSelf;
            set => _isSelf = value;
        }

        // 用于显示格式化时间
        public string DisplayTime => Timestamp.ToString("HH:mm");
    }
}