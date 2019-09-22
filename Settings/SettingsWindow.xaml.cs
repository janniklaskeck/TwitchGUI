using Microsoft.Win32;
using System.Windows;

namespace TwitchGUI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void ChooseStreamlinkPath(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Choose Streamlink .exe file",
                InitialDirectory = @"C:\",
                RestoreDirectory = true,
                DefaultExt = "exe",
                Filter = "Executable files (*.exe)|*.exe",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (dialog.ShowDialog() == true)
            {
                Settings.Instance.StreamlinkLocation = dialog.FileName;
            }
        }
    }
}
