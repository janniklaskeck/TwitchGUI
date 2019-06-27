namespace TwitchGUI
{
    public class TwitchChannel
    {
        public string UserId { get; set; }
        public string Login { get; set; }
        public string DisplayName { get; set; }
        public string GameId { get; set; }
        public string GameName { get; set; }
        public string Description { get; set; }
        public long Uptime { get; set; }
        public int Viewers { get; set; }
        private bool isOnline;
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
        public object PreviewImage { get; set; }

        public TwitchChannel(string userId, string login)
        {
            UserId = userId;
            Login = login;
        }

        public void ShowNotification()
        {
            // TODO
        }
    }
}
