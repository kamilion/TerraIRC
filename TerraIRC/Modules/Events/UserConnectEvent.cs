using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace tMod_v3
{
    public class UserEvent : Event
    {
        public User user;
        public UserEvent(User user)
        {
            this.user = user;
        }
    }
    public class UserConnectEvent : UserEvent
    {
        public UserConnectEvent(User user) : base(user) { }
    }
}
