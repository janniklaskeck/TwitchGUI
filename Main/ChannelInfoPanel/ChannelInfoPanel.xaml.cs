using System;
using System.Windows.Controls;

namespace TwitchGUI
{
    /// <summary>
    /// Interaction logic for ChannelInfoPanel.xaml
    /// </summary>
    public partial class ChannelInfoPanel : UserControl
    {
        public ChannelInfoPanel()
        {
            InitializeComponent();
            ChannelList.onSelectionChanged += UpdateInfo;
            MainWindow.onChannelsUpdateFinished += UpdateInfo;
        }

        private void UpdateInfo()
        {
            var channel = ChannelList.SelectedChannel;
            infoTexts.SetTexts(channel);
            if (channel == null)
                return;
            image.Dispatcher.BeginInvoke(new Action(() => image.Source = channel.PreviewImage));
            image.UpdateLayout();
        }
    }
}
