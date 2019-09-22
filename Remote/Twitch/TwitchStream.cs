using System.Collections.Generic;

namespace TwitchGUI
{
    public class TwitchStreams
    {
        public List<TwitchStream> data;
    }

    public class TwitchStream
    {
        public string user_id;
        public string game_id;
        public string title;
        public int viewer_count;
        public string language;
        public string thumbnail_url;
        public string started_at;
    }
}
