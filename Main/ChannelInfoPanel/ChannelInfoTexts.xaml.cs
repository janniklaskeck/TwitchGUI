using System.Windows.Controls;

namespace TwitchGUI.Main
{
    /// <summary>
    /// Interaction logic for ChannelInfoPanelInfo.xaml
    /// </summary>
    public partial class ChannelInfoTexts : UserControl
    {
        public ChannelInfoTexts()
        {
            InitializeComponent();
        }

        public void SetTexts(TwitchChannel channel)
        {
            if (channel == null || !channel.IsOnline)
            {
                string offlineChannelMessage = "Channel is offline";
                GameNameTextBlock.Text = offlineChannelMessage;
                UptimeTextBlock.Text = offlineChannelMessage;
                ViewerCountTextBlock.Text = "0";

                TitleTextBlock.Text = offlineChannelMessage;
            }
            else if (channel.IsOnline)
            {
                GameNameTextBlock.Text = channel.GameName;
                UptimeTextBlock.Text = channel.Uptime;
                ViewerCountTextBlock.Text = channel.Viewers.ToString();

                TitleTextBlock.Text = channel.Description;
            }
        }
    }
}
