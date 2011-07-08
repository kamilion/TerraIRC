using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using tMod_v3.Events;
using System.IO;

namespace tMod_v3
{
    class IRCCommands
    {
        public static void Finger()
        {
            if (TerraIRC.enableFingering)
            {
                IRC.send("PRIVMSG " + TerraIRC.channel + " :....................../´¯/)");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :....................,/¯../");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :.................../..../");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :............./´¯/'...'/´¯¯`·¸");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :........../'/.../..../......./¨¯\\");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :........('(...´...´.... ¯~/'...')");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :.........\\.................'...../");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :..........''...\\.......... _.·´");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :............\\..............(");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :..............\\.............\\...");
            }
        }

        public static void Players()
        {
            string str = "";
            int pCount = 0;
            dynamic[] player5 = MainMod.Player;
            if (player5 == null) return;
            for (int k = 0; k < player5.Length; k++)
            {
                dynamic pActive = player5[k];
                if (pActive.active)
                {
                    if (pActive.name.Length > 0)
                    {
                        pCount++;
                        if (str == "")
                        {
                            str = str + pActive.name;
                        }
                        else
                        {
                            str = str + ", " + pActive.name;
                        }
                    }
                }
            }
            if (pCount == 0)
            {
                IRC.send("PRIVMSG " + TerraIRC.channel + " :No one is Terrariaing right now.");
            }
            else
            {
                IRC.send("PRIVMSG " + TerraIRC.channel + " :Current players: " + str);
            }
        }

        public static void reloadSettings()
        {
            TerraIRC.loadSettings();
        }
    }
}
