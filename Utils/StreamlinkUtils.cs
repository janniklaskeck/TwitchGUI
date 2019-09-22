using System.Diagnostics;

namespace TwitchGUI
{
    public class StreamlinkUtils
    {
        public static void StartLiveStream(TwitchChannel channel)
        {
            if (channel == null || !channel.IsOnline || string.IsNullOrEmpty(Settings.Instance.StreamlinkLocation))
                return;
            var process = new Process();
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = Settings.Instance.StreamlinkLocation,
                Arguments = GetStreamlinkArguments(channel)
            };
            process.StartInfo = startInfo;
            process.Start();
        }

        private static string GetStreamlinkArguments(TwitchChannel channel)
        {
            return "twitch.tv/" + channel.Login + " " + Settings.Instance.SelectedQuality;
        }
    }
}
