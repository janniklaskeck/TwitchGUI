using System;
using System.Windows;

namespace TwitchGUI
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public static readonly int MaxMinWidth = 450;
        public static readonly int MaxMinHeight = 100;

        public NotificationWindow(string title, string description)
        {
            InitializeComponent();
            ShowInTaskbar = false;
            TitleTextBlock.Text = string.Format("{0} came online!", title);
            DescriptionTextBlock.Text = description;
        }

        private void NotificationHidden(object sender, EventArgs args)
        {
            NotificationsUtils.NotificationClosed(this);
            Close();
        }

        public void SetPositionIndex(int index)
        {
            var workingArea = SystemParameters.WorkArea;
            Left = workingArea.Right - MaxMinWidth - 10;
            Top = workingArea.Bottom - MaxMinHeight - MaxMinHeight * index - 5 * index;
        }
    }
}
