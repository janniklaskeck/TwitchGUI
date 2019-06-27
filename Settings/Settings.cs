using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TwitchGUI
{
    public class Settings
    {
        public List<TwitchChannel> channels = new List<TwitchChannel>();

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
            string settingsString = JsonConvert.SerializeObject(Instance);

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
            Instance.channels = loadedSettings.channels;
        }
    }
}
