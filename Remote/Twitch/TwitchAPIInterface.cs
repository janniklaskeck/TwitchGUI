using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TwitchGUI
{
    public class TwitchAPIInterface
    {
        private static readonly string TwitchApiBaseUrl = "https://api.twitch.tv/helix/";
        private static readonly string TwitchGUIClientId = "rfpepzumaxd1iija3ip3fixao6z13pj";

        private static readonly HttpClient client = new HttpClient();

        static TwitchAPIInterface()
        {
            client.DefaultRequestHeaders.Add("Client-ID", TwitchGUIClientId);
        }

        private TwitchAPIInterface() { }

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
            var response = await client.GetStringAsync(url);
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
            var response = await client.GetStringAsync(url);
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
            var response = await client.GetStringAsync(url);
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
            var response = await client.GetStringAsync(url);
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
    }
}
