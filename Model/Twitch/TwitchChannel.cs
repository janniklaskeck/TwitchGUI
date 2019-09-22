using Newtonsoft.Json;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TwitchGUI
{
    public class TwitchChannel
    {
        public string UserId { get; set; }
        public string Login { get; set; }
        private string displayName;
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(displayName))
                    return displayName;
                else if (string.IsNullOrEmpty(Login))
                {
                    return UserId;
                }
                else
                {
                    return Login;
                }
            }
            set { displayName = value; }
        }
        [JsonIgnore]
        public string GameId { get; set; }
        [JsonIgnore]
        public string GameName { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        [JsonIgnore]
        public string StartedAt { get; set; }
        [JsonIgnore]
        public string Uptime
        {
            get
            {
                var startDateTime = DateTime.Parse(StartedAt);
                var uptime = DateTime.Now - startDateTime;
                return string.Format("{0:00}:{1:00}:{2} Uptime", uptime.Hours, uptime.Minutes, uptime.Seconds);
            }
        }
        [JsonIgnore]
        public int Viewers { get; set; }
        private bool isOnline;
        [JsonIgnore]
        public bool IsOnline
        {
            get
            {
                return isOnline;
            }
            set
            {
                if (!isOnline && value)
                    ShowNotification();
                isOnline = value;
            }
        }
        public string PreviewImageURL { get; set; }
        [JsonIgnore]
        public ImageSource PreviewImage { get; set; }

        [JsonIgnore]
        public Brush BGColor
        {
            get
            {
                if (IsOnline)
                    return Brushes.DarkGreen;
                return Brushes.Transparent;
            }
        }

        [JsonIgnore]
        private bool showNotification;

        public TwitchChannel(string userId, string login)
        {
            UserId = userId;
            Login = login;
            MainWindow.onChannelsUpdateFinished += ChannelLoaded;
        }

        public void ShowNotification()
        {
            showNotification = true;

        }

        private void ChannelLoaded()
        {
            if (showNotification)
            {
                NotificationsUtils.ShowChannelOnlineNotification(this);
                showNotification = false;
            }
        }
    }
}
