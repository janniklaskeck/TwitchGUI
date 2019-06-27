using System.Collections.Generic;

namespace TwitchGUI
{
    public class TwitchUsers
    {
        public List<TwitchUser> data;
    }

    public class TwitchUser
    {
        public string id;
        public string login;
        public string display_name;
    }
}
