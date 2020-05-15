using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TwitchGUI
{
    public class TwitchAPIInterface
    {
        private static readonly string TwitchApiBaseUrl = "https://api.twitch.tv/helix/";

        private static readonly HttpClient client = new HttpClient();
        private static readonly string redirectURL = "http://127.0.0.1:8088/";

        private static TwitchOAuthData data;
        private static Thread loginListener;

        static TwitchAPIInterface()
        {
        }

        private TwitchAPIInterface() { }

        public static async void Login()
        {
            if (Settings.Instance.RefreshToken != null)
            {
                var dayTicks = new TimeSpan(1, 0, 0, 0).Ticks;
                bool isTokenPseudoValid = DateTime.Now.Ticks - dayTicks < Settings.Instance.RefreshTokenDate.Ticks;
                if (isTokenPseudoValid)
                {
                    await RefreshAccessToken();
                    MainWindow.UpdateChannels();
                    return;
                }
            }
            string url = string.Format("https://id.twitch.tv/oauth2/authorize" +
                "?client_id={0}" +
                "&redirect_uri={1}" +
                "&response_type=code", Settings.Instance.ClientID, redirectURL);
            if (StartListenerThread())
            {
                System.Diagnostics.Process.Start(url);
            }
        }

        private static bool StartListenerThread()
        {
            int isReady = 0;
            loginListener = new Thread(() =>
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(redirectURL);
                try
                {
                    listener.Start();
                }
                catch(HttpListenerException exc)
                {
                    Console.WriteLine(exc);
                    isReady = 2;
                    return;
                }
                isReady = 1;

                HttpListenerContext context = listener.GetContext();

                string codeString = context.Request.Url.PathAndQuery;
                codeString = codeString.Remove(0, 1); // remove leading slash
                string code = HttpUtility.ParseQueryString(codeString).Get("code");
                if (code != null && code.Length > 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GetAccessToken(code);
                    });

                    // Get a response stream and write the response to it.
                    HttpListenerResponse response = context.Response;
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Success!");
                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                listener.Stop();
            });
            loginListener.Start();
            while (true)
            {
                if (isReady == 1)
                    return true;
                else if (isReady == 2)
                    return false;
            }
        }

        private static async Task GetAccessToken(string code)
        {
            string url = string.Format("https://id.twitch.tv/oauth2/token" +
                "?client_id={0}" +
                "&client_secret={1}" +
                "&code={2}" +
                "&grant_type=authorization_code" +
                "&redirect_uri={3}", Settings.Instance.ClientID, Settings.Instance.ClientSecret, code, redirectURL);
            var response = await client.PostAsync(url, null);
            var responseString = await response.Content.ReadAsStringAsync();
            data = JsonConvert.DeserializeObject<TwitchOAuthData>(responseString);
            Settings.Instance.RefreshToken = data.refresh_token;
            Settings.Instance.RefreshTokenDate = DateTime.Now;
            UpdateClientHeaders();
            MainWindow.UpdateChannels();
        }

        private static async Task RefreshAccessToken()
        {
            string url = string.Format("https://id.twitch.tv/oauth2/token" +
                "?grant_type=refresh_token" +
                "&client_id={0}" +
                "&client_secret={1}" +
                "&refresh_token={2}", Settings.Instance.ClientID, Settings.Instance.ClientSecret, Settings.Instance.RefreshToken);
            var response = await client.PostAsync(url, null);
            var responseString = await response.Content.ReadAsStringAsync();
            data = JsonConvert.DeserializeObject<TwitchOAuthData>(responseString);
            Settings.Instance.RefreshToken = data.refresh_token;
            Settings.Instance.RefreshTokenDate = DateTime.Now;
            UpdateClientHeaders();
        }

        public static async Task UpdateChannels(List<TwitchChannel> channels)
        {
            List<TwitchChannel> newChannels = channels.Where(c => c.UserId.Length == 0 || c.Login.Length == 0).ToList();
            await FetchChannelInformation(newChannels);
            await FetchStreamInformation(channels);
        }

        private static async Task FetchChannelInformation(List<TwitchChannel> channels)
        {
            if (channels.Count == 0)
                return;
            string userNameListString = string.Empty;
            foreach (var channel in channels)
            {
                if (userNameListString.Length > 0)
                    userNameListString += "&";
                if (channel.Login.Length == 0)
                {
                    userNameListString += "id=" + channel.UserId;
                }
                else
                {
                    userNameListString += "login=" + channel.Login;
                }
            }

            string url = TwitchApiBaseUrl + "users?" + userNameListString;
            var response = await HTTPGetSafe(url);
            var users = JsonConvert.DeserializeObject<TwitchUsers>(response);

            foreach (var channel in channels)
            {
                var matchingUser = users.data.FirstOrDefault(tu =>
                (channel.Login.Equals(tu.login) && channel.Login.Length > 0) || channel.UserId == tu.id);
                if (matchingUser == null)
                {
                    Settings.Instance.RemoveChannel(channel);
                    continue;
                }
                channel.UserId = matchingUser.id;
                channel.Login = matchingUser.login;
                channel.DisplayName = matchingUser.display_name;
            }
        }

        private static async Task FetchStreamInformation(List<TwitchChannel> channels)
        {
            if (channels.Count == 0)
                return;
            string userIdListString = string.Empty;
            foreach (var channel in channels)
            {
                if (userIdListString.Length > 0)
                    userIdListString += "&user_id=";
                userIdListString += channel.UserId;
            }

            string url = TwitchApiBaseUrl + "streams?user_id=" + userIdListString;
            var response = await HTTPGetSafe(url);
            var streams = JsonConvert.DeserializeObject<TwitchStreams>(response);

            foreach (var channel in channels)
            {
                var matchingStream = streams.data.FirstOrDefault(ts => ts.user_id.Equals(channel.UserId));
                channel.IsOnline = matchingStream != null;
                if (channel.IsOnline)
                {
                    channel.Description = matchingStream.title;
                    channel.PreviewImageURL = matchingStream.thumbnail_url;
                    channel.Viewers = matchingStream.viewer_count;
                    channel.GameId = matchingStream.game_id;
                    channel.StartedAt = matchingStream.started_at;
                    var previewImage = await DownloadPreviewImage(channel);
                    channel.PreviewImage = previewImage;
                }
            }

            var onlineChannels = channels.Where(c => c.IsOnline).ToList();
            await FetchGameInformation(onlineChannels);
        }

        private static async Task<BitmapSource> DownloadPreviewImage(TwitchChannel channel)
        {
            string previewUrl = channel.PreviewImageURL;
            previewUrl = previewUrl.Replace("{width}", "1600");
            previewUrl = previewUrl.Replace("{height}", "900");

            BitmapSource bitmap = null;
            using (var response = await client.GetAsync(new Uri(previewUrl)))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = new MemoryStream())
                    {
                        await response.Content.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        bitmap = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                }
            }
            return bitmap;
        }

        private static async Task FetchGameInformation(List<TwitchChannel> channels)
        {
            if (channels.Count == 0)
                return;
            string gameIdListString = string.Empty;
            foreach (var channel in channels)
            {
                if (gameIdListString.Length > 0)
                    gameIdListString += "&id=";
                gameIdListString += channel.GameId;
            }

            string url = TwitchApiBaseUrl + "games?id=" + gameIdListString;
            var response = await HTTPGetSafe(url);
            var games = JsonConvert.DeserializeObject<TwitchGames>(response);

            foreach (var twitchGame in games.data)
            {
                var matchingChannels = channels.Where(c => c.GameId.Equals(twitchGame.id));
                foreach (var matchingChannel in matchingChannels)
                {
                    matchingChannel.GameName = twitchGame.name;
                }
            }
        }

        public static async Task<List<TwitchChannel>> GetFollowedChannels(string login)
        {
            List<TwitchChannel> singleEntryList = new List<TwitchChannel> { new TwitchChannel("", login) };
            await FetchChannelInformation(singleEntryList);
            string userId = singleEntryList[0].UserId;
            if (string.IsNullOrEmpty(userId))
                return null;

            string url = TwitchApiBaseUrl + "users/follows?from_id=" + userId;
            var response = await HTTPGetSafe(url);
            var followedChannels = JsonConvert.DeserializeObject<TwitchFollowedChannels>(response);

            var channels = new List<TwitchChannel>();
            foreach (var followedChannel in followedChannels.data)
            {
                var newChannel = new TwitchChannel(followedChannel.to_id, string.Empty)
                {
                    DisplayName = followedChannel.to_name
                };
                channels.Add(newChannel);
            }
            return channels;
        }

        private static async Task<string> HTTPGetSafe(string url)
        {
            var response = await client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshAccessToken();
            }
            return await response.Content.ReadAsStringAsync();
        }

        private static void UpdateClientHeaders()
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Client-ID", Settings.Instance.ClientID);
            client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", data.access_token));
        }

        private class TwitchOAuthData
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public int expires_in { get; set; }
            public List<string> scope { get; set; }
            public string token_type { get; set; }
        }
    }
}
