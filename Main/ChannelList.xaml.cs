using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace TwitchGUI
{
    /// <summary>
    /// Interaction logic for ChannelList.xaml
    /// </summary>
    public partial class ChannelList : UserControl
    {
        public static TwitchChannel SelectedChannel { get; private set; }

        public static Action onSelectionChanged;

        public ChannelList()
        {
            InitializeComponent();
            ChannelListBox.ItemsSource = Settings.Instance.Channels;
            MainWindow.onChannelsUpdateFinished += () =>
            {
                var view = CollectionViewSource.GetDefaultView(Settings.Instance.Channels);
                using (view.DeferRefresh())
                {
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription("IsOnline", ListSortDirection.Descending));
                    view.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
                }
                ChannelListBox.Items.Refresh();
            };
        }

        private void ChannelListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedChannel = ChannelListBox.SelectedItem as TwitchChannel;
            onSelectionChanged?.Invoke();
        }

        private void StartStream(object sender, EventArgs args)
        {
            StreamlinkUtils.StartLiveStream(SelectedChannel);
        }
    }
}
