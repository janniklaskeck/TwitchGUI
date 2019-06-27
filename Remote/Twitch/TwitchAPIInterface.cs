using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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
            List<TwitchChannel> newChannels = channels.Where(c => c.UserId.Length == 0).ToList();
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
                userNameListString += channel.Login;
            }

            string url = TwitchApiBaseUrl + "users?login=" + userNameListString;
            var response = await client.GetStringAsync(url);
            var users = JsonConvert.DeserializeObject<TwitchUsers>(response);

            foreach (var channel in channels)
            {
                var matchingUser = users.data.FirstOrDefault(tu => channel.Login.Equals(tu.login));
                channel.UserId = matchingUser.id;
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
                    userIdListString += "&";
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
                }
            }

            var onlineChannels = channels.Where(c => c.IsOnline).ToList();
            await FetchGameInformation(onlineChannels);
        }

        private static async Task FetchGameInformation(List<TwitchChannel> channels)
        {
            if (channels.Count == 0)
                return;
            string gameIdListString = string.Empty;
            foreach (var channel in channels)
            {
                if (gameIdListString.Length > 0)
                    gameIdListString += "&";
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
    }
}
