using System.Collections.Generic;

namespace TwitchGUI
{
    public class TwitchGames
    {
        public List<TwitchGame> data;
    }

    public class TwitchGame
    {
        public string id;
        public string name;
        public string box_art_url;
    }
}
