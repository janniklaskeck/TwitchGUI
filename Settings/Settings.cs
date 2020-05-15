using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace TwitchGUI
{
    public class Settings : INotifyPropertyChanged
    {
        public ObservableCollection<TwitchChannel> Channels { get; } = new ObservableCollection<TwitchChannel>();

        public string ClientID { get; set; }
        public string ClientSecret { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenDate { get; set; }

        public string StreamlinkLocation { get; set; }
        public int SelectedQualityIndex { get; set; } = 6;

        private static readonly List<string> qualityList = new List<string> {
            "audio_only","160p","360p","480p","720p","720p60","1080p60", "best"
        };

        public string SelectedQuality => qualityList[SelectedQualityIndex];

        public event PropertyChangedEventHandler PropertyChanged;

        public static Settings Instance { get; } = new Settings();

        private static string FullSettingsFolderPath
        {
            get
            {
                string parentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string separator = Path.DirectorySeparatorChar.ToString();
                string settingsFolderName = "TwitchGUI";
                return parentFolder + separator + settingsFolderName;
            }
        }

        private static string FullSettingsFilePath
        {
            get
            {
                string settingsFileName = "twitchGuiSettings.cfg";
                return FullSettingsFolderPath + Path.DirectorySeparatorChar.ToString() + settingsFileName;
            }
        }

        private Settings() { }

        public static void SaveSettings()
        {
            string settingsString = JsonConvert.SerializeObject(Instance, Formatting.Indented);

            try
            {
                if (!File.Exists(FullSettingsFilePath))
                {
                    Directory.CreateDirectory(FullSettingsFolderPath);
                    File.Create(FullSettingsFilePath);
                }
                File.WriteAllText(FullSettingsFilePath, settingsString);
            }
            catch (IOException exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }

        public static void LoadSettings()
        {
            if (!File.Exists(FullSettingsFilePath))
                return;
            string readSettings = File.ReadAllText(FullSettingsFilePath);
            var loadedSettings = JsonConvert.DeserializeObject<Settings>(readSettings);
            if (loadedSettings != null)
            {
                foreach (var loadedChannel in loadedSettings.Channels)
                {
                    Instance.Channels.Add(loadedChannel);
                }
                Instance.StreamlinkLocation = loadedSettings.StreamlinkLocation;
                Instance.SelectedQualityIndex = loadedSettings.SelectedQualityIndex;
                Instance.ClientID = loadedSettings.ClientID;
                Instance.ClientSecret = loadedSettings.ClientSecret;
                Instance.RefreshToken = loadedSettings.RefreshToken;
                Instance.RefreshTokenDate = loadedSettings.RefreshTokenDate;
            }
        }

        public void AddChannel(string login)
        {
            if (string.IsNullOrEmpty(login))
                return;
            TwitchChannel newChannel = new TwitchChannel("", login);
            if (Channels.FirstOrDefault(tc => tc.Login.ToLower().Equals(login.ToLower())) != null)
                return;
            Channels.Add(newChannel);
            SaveSettings();
            OnPropertyChanged("Channels");
        }

        public void AddChannel(TwitchChannel channel)
        {
            bool alreadyExistingChannelById = Channels.FirstOrDefault(tc => tc.UserId == channel.UserId) != null;
            bool alreadyExistingChannelByLogin = Channels.FirstOrDefault(tc => tc.Login.ToLower().Equals(channel.Login.ToLower())) != null;
            if (alreadyExistingChannelById || (alreadyExistingChannelByLogin && channel.Login.Length > 0))
                return;
            Channels.Add(channel);
            SaveSettings();
            OnPropertyChanged("Channels");
        }

        public void RemoveChannel(TwitchChannel twitchChannel)
        {
            if (twitchChannel == null)
                return;
            Channels.Remove(twitchChannel);
            SaveSettings();
            OnPropertyChanged("Channels");
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
