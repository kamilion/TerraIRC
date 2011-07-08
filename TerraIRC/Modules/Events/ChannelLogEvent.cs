using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tMod_v3
{
    public class ChannelLogEvent : Event
    {
        public string message;
        public ChannelLogEvent(string message)
        {
            this.message = message;
        }
    }
}
