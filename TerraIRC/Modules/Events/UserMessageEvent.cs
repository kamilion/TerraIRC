using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace tMod_v3
{
    public class UserMessageEvent : Event
    {
        public string message;
        public User sendee;
        public User sender;
        public UserMessageEvent(User sender, User sendee, string message)
        {
            this.message = message;
            this.sender = sender;
            this.sendee = sendee;
        }

    }
}
