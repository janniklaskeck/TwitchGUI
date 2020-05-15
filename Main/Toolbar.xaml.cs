using System.Windows;
using System.Windows.Controls;

namespace TwitchGUI
{
    /// <summary>
    /// Interaction logic for Toolbar.xaml
    /// </summary>
    public partial class Toolbar : UserControl
    {
        public Toolbar()
        {
            InitializeComponent();
            QualityComboBox.SelectedIndex = Settings.Instance.SelectedQualityIndex;

            MainWindow.onChannelsUpdateStarted += () => ProgressCircle.Visibility = Visibility.Visible;
            MainWindow.onChannelsUpdateFinished += () => ProgressCircle.Visibility = Visibility.Hidden;
        }

        private void AddChannelButton_Click(object sender, RoutedEventArgs e)
        {
            var newChannelDialog = new NewChannelDialog();
            if (newChannelDialog.ShowDialog() == true)
            {
                Settings.Instance.AddChannel(newChannelDialog.Answer);
            }
        }

        private void RemoveChannelButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Instance.RemoveChannel(ChannelList.SelectedChannel);
        }

        private void AddFollowedChannelsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new GetFollowedChannelsDialog();
            if (dialog.ShowDialog() == true)
            {
                var followedChannels = dialog.FollowedChannels;
                if (followedChannels != null)
                {
                    foreach (var channel in followedChannels)
                    {
                        Settings.Instance.AddChannel(channel);
                    }
                }
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow();
            settings.ShowDialog();
        }

        private void QualityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Instance.SelectedQualityIndex = QualityComboBox.SelectedIndex;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            TwitchAPIInterface.Login();
        }
    }
}
