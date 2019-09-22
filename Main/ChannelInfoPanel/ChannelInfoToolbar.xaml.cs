using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace TwitchGUI.Main
{
    /// <summary>
    /// Interaction logic for ChannelInfoToolbar.xaml
    /// </summary>
    public partial class ChannelInfoToolbar : UserControl
    {
        private static readonly string OpenInBrowserTemplate = "https://twitch.tv/{0}";
        private static readonly string OpenChatTemplate = "https://twitch.tv/{0}/chat";

        public ChannelInfoToolbar()
        {
            InitializeComponent();
        }

        private void OpenInBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(string.Format(OpenInBrowserTemplate, ChannelList.SelectedChannel.Login));
        }

        private void OpenChatButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(string.Format(OpenChatTemplate, ChannelList.SelectedChannel.Login));
        }
        
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            StreamlinkUtils.StartLiveStream(ChannelList.SelectedChannel);
        }
    }
}
