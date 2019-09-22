using System;
using System.Windows;

namespace TwitchGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Settings.LoadSettings();
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.SaveSettings();
        }
    }
}
