using System.Windows;

namespace TwitchGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitialUpdate();
        }

        private async void InitialUpdate()
        {
            Settings.LoadSettings();
            Settings.Instance.channels.Add(new TwitchChannel("", "nuamor"));
            await TwitchAPIInterface.UpdateChannels(Settings.Instance.channels);
            Settings.SaveSettings();
        }
    }
}
