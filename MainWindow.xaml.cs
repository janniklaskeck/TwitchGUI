using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace TwitchGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();

        public static Action onChannelsUpdateStarted;
        public static Action onChannelsUpdateFinished;

        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(60);
            timer.Tick += UpdateChannels;
            timer.IsEnabled = true;
            UpdateChannels();
        }

        public static void UpdateChannels()
        {
            UpdateChannels(null, null);
        }

        private static async void UpdateChannels(object sender, EventArgs args)
        {
            onChannelsUpdateStarted?.Invoke();
            try
            {
                await TwitchAPIInterface.UpdateChannels(Settings.Instance.Channels.ToList());
                Console.WriteLine("Channels updated");
                onChannelsUpdateFinished?.Invoke();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            NotificationsUtils.CloseAllNotifications();
        }
    }
}
