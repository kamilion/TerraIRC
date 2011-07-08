using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using tMod_v3.Events;
using Terraria;
using System.Collections;

namespace tMod_v3
{
    internal class TerraIRC : Plugin
    {
        // Settings
        public static Dictionary<string, string> settings;
        public static string settingsPath = "terrairc.txt";

        // IRC fields
        public static string server;
        public static int port;
        public static string authtype;
        public static string authpass;
        public static string nickname;
        public static string username;
        public static string channel;
        public static bool enableServerLinking;
        private Thread irc;
        public static bool rawConsole;
        public static string commandPrefix;
        public static bool enableFingering;
        public static string serverLinking_server;
        public static string serverLinking_SID;
        public static string serverLinking_Pass;
        public static string serverLinking_Desc;
        public static string serverLinking_Protocol;

        public static Dictionary<string, User> Users;
        public static ArrayList Clients;
        public static Dictionary<string, Channel> Channels;

        public override string Name { get { return "TerraIRC"; } }
        public override Version Version { get { return new Version(1, 2); } }
        public override string Author { get { return "PwnCraft"; } }

        public TerraIRC()
        {
            loadSettings();
            Console.WriteLine("Connecting to " + server + ":" + port);
            Console.WriteLine("[TerraIRC] " + Version + " loaded!");

            MessageBufferMod.ChatMessageReceived += new PacketReceivedEventHandler(OnPlayerChat);
            MessageBufferMod.SpawnPlayerReceived += new PacketReceivedEventHandler(OnPlayerJoin);
            MessageBufferMod.PlayerDeathReceived += new PacketReceivedEventHandler(OnPlayerDeath);

            this.irc = new Thread(new ThreadStart(startIRC));
            this.irc.Start();
        }

        public void Unload()
        {
            IRC.send("QUIT :Unloaded!");
            irc.Abort();
            Console.WriteLine("[TerraIRC] Unloaded :(");
        }

        public void startIRC()
        {
            IRC.connect(server, port, authtype, authpass, nickname, username, channel);
        }

        public void OnPlayerChat(object sender, PacketReceivedEventArgs args)
        {
            string chatUser = Session.Sessions[args.Player].Username;
            string chatUserText = Encoding.ASCII.GetString(args.ReadBuffer, args.Start + 5, args.Length - 5);
            if (chatUserText.Substring(0, 1) == "/") return;
            IRC.send("PRIVMSG " + channel + " :(" + chatUser + ") " + chatUserText);
        }

        public void OnPlayerDeath(object sender, PacketReceivedEventArgs args)
        {
            string deadUser = Session.Sessions[args.Player].Username;
            IRC.send("PRIVMSG " + channel + " :" + deadUser + " was slain..");
        }

        public void OnPlayerJoin(object sender, PacketReceivedEventArgs args)
        {
            string joinUser = Session.Sessions[args.Player].Username;
            IRC.send("PRIVMSG " + channel + " :[" + joinUser + " connected]");
        }

        public void OnPlayerLeave(object sender, PacketReceivedEventArgs args)
        {
            string leaveUser = Session.Sessions[args.Player].Username;
            IRC.send("PRIVMSG " + channel + " :[" + leaveUser + " disconnected]");
        }

        protected static string getSetting(string setting, string settingValue)
        {
            try
            {
                string ret = settings[setting];
                return ret;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("[TerraIRC] Missing setting: " + setting);
                TextWriter t = new StreamWriter(settingsPath, true);
                t.WriteLine(setting + "=" + settingValue);
                t.Close();
                settings.Add(setting, settingValue);
                return settingValue;
            }
        }

        public static void loadSettings()
        {
            settings = new Dictionary<string, string>();
            if (!File.Exists(settingsPath))
            {
                TextWriter writer = new StreamWriter(settingsPath);
                writer.WriteLine("#server linking allows you to connect terraria server clients directly to a TS6 server, join irc.pwncraft.net #pwncraft for help on setting up");
                writer.WriteLine("serverLinking=false");
                writer.WriteLine("#only charybdis is supported now, it should work on ratbox and other charybdis based ircds");
                writer.WriteLine("serverLinking-Protocol=charybdis");
                writer.WriteLine("serverLinking-server=localhost");
                writer.WriteLine("serverLinking-SID=30X");
                writer.WriteLine("serverLinking-Pass=asdf");
                writer.WriteLine("serverLinking-Desc=TerraIRC - Terraria <-> IRC Bridge");
                writer.WriteLine("server=irc.pwncraft.net");
                writer.WriteLine("port=6667");
                writer.WriteLine("# use either nickserv, x or none. refer to your ircops for more information");
                writer.WriteLine("# this doesn't work in the current version 1.0");
                writer.WriteLine("authtype=x");
                writer.WriteLine("authpass=mypassword");
                writer.WriteLine("nickname=TerraIRC");
                writer.WriteLine("username=Terraria IRC Bot");
                writer.WriteLine("channel=#pwncraft");
                writer.WriteLine("raw-console=true");
                writer.WriteLine("irc-prefix=!");
                writer.WriteLine("fingering=true");
                writer.Close();
            }

            foreach (string str2 in File.ReadAllLines(settingsPath))
            {
                if (!str2.StartsWith("#"))
                {
                    settings.Add(str2.Split(new char[] { '=' })[0], str2.Split(new char[] { '=' })[1]);
                }
            }
            // IRC fields
            try
            {
                server = getSetting("server", "irc.pwncraft.net");
                port = int.Parse(getSetting("port", "6667"));
                authtype = getSetting("authtype", "x");
                authpass = getSetting("authpass", "mypassword");
                nickname = getSetting("nickname", "TerraIRC");
                username = getSetting("username", "Terraria IRC Bot");
                channel = getSetting("channel", "#pwncraft");
                rawConsole = bool.Parse(getSetting("raw-console", "true"));
                commandPrefix = getSetting("irc-prefix", "!");
                enableFingering = bool.Parse(getSetting("fingering", "true"));
                enableServerLinking = bool.Parse(getSetting("serverLinking", "false"));
                if (enableServerLinking)
                {
                    serverLinking_server = getSetting("serverLinking-server", "localhost");
                    serverLinking_Pass = getSetting("serverLinking-pass", "asdafs");
                    serverLinking_SID = getSetting("serverLinking-SID", "30X");
                    serverLinking_Desc = getSetting("serverLinking-Desc", "TerraIRC - Terraria <-> IRC Bridge");
                    serverLinking_Protocol = getSetting("serverLinking-Protocol", "charybdis").ToLower();
                    Users = new Dictionary<string, User>();
                    Channels = new Dictionary<string, Channel>();
                    Clients = new ArrayList();
                }

            }
            catch (NullReferenceException exception)
            {
                Console.WriteLine("[TerraIRC] Error: " + exception.ToString());
            }
        }
    }
}