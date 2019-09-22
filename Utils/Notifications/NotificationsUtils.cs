using System.Collections.Generic;

namespace TwitchGUI
{
    public class NotificationsUtils
    {
        private static readonly List<NotificationWindow> activeNotifications = new List<NotificationWindow>();

        public static void ShowChannelOnlineNotification(TwitchChannel channel)
        {
            var window = new NotificationWindow(channel.DisplayName, channel.Description);

            ShowNewNotification(window);
        }

        private static void ShowNewNotification(NotificationWindow window)
        {
            window.SetPositionIndex(activeNotifications.Count);
            activeNotifications.Add(window);
            window.Show();
        }

        public static void NotificationClosed(NotificationWindow window)
        {
            activeNotifications.Remove(window);
        }

        public static void CloseAllNotifications()
        {
            foreach(var window in activeNotifications)
            {
                window.Close();
            }
        }
    }
}
