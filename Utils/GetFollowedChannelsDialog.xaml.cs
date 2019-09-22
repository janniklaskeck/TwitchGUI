using System.Collections.Generic;
using System.Windows;

namespace TwitchGUI
{
    /// <summary>
    /// Interaction logic for GetFollowedChannels.xaml
    /// </summary>
    public partial class GetFollowedChannelsDialog : Window
    {
        public List<TwitchChannel> FollowedChannels { get; private set; }

        public GetFollowedChannelsDialog()
        {
            InitializeComponent();
            channelNameTextBox.Focus();
        }

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(channelNameTextBox.Text))
            {
                FollowedChannels = await TwitchAPIInterface.GetFollowedChannels(channelNameTextBox.Text);
                MainWindow.UpdateChannels();
            }
            DialogResult = true;
        }
    }
}
