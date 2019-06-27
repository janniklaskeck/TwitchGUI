using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
