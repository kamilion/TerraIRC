using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Terraria;

namespace tMod_v3
{
    public static class IRC
    {
        public static StreamWriter writer;
        public static void send(string text)
        {
            writer.WriteLine(text);
            writer.Flush();
            if (TerraIRC.rawConsole == true)
            {
                Console.WriteLine("[TerraIRC] IRC Raw: " + text);
            }
        }

        public static void command(string cmd)
        {
            string[] array = cmd.Split(new char[]
			{
				' '
			});
            int num = 0;
            dynamic[] player = MainMod.Player;
            if (player == null) return;
            for (int i = 0; i < player.Length; i++)
            {
                dynamic player2 = player[i];
                if (player2.name.Length < 1)
                {
                    num = player2.whoAmi;
                    break;
                }
            }
            dynamic player3 = MainMod.Player[num];
            if (player3 == null) return;
            player3.name = "Console";
        }

        public static void connect(string server, int port, string authtype, string authpass, string nick, string user, string channel)
        {
            char oldChar = Convert.ToChar(1);
            try
            {
                Console.WriteLine("CONNETING!");
                TcpClient tcpClient = new TcpClient(server, port);
                NetworkStream stream = tcpClient.GetStream();
                StreamReader streamReader = new StreamReader(stream);
                IRC.writer = new StreamWriter(stream);
                string[] array = user.Split(new char[]
				{
					' '
				});
                IRC.send("USER " + array[0] + " 0 * :" + user);
                IRC.send("NICK " + nick);
                while (true)
                {
                    try
                    {
                        string text;
                        if ((text = streamReader.ReadLine()) == null)
                        {
                            IRC.writer.Close();
                            streamReader.Close();
                            tcpClient.Close();
                        }
                        else
                        {
                            string[] array2 = text.Split(new char[]
						{
							' '
						});
                            if (array2[1] == "376")
                            {
                                IRC.send("JOIN " + channel);
                            }
                            else
                            {
                                if (text.EndsWith("JOIN :" + channel))
                                {
                                    string str = text.Substring(1, text.IndexOf("!") - 1);
                                    dynamic[] player = MainMod.Player;
                                    try
                                    {
                                        if (player == null) return;
                                        for (int i = 0; i < player.Length; i++)
                                        {
                                            dynamic player2 = player[i];
                                            if (player2.active)
                                            {
                                                if (player2.name.Length > 0)
                                                {
                                                    Session.Sessions[player2.whoAmi].SendText(255, 255, 255, "[IRC] <" + str + "> has joined IRC");
                                                }
                                            }
                                        }
                                    }
                                    catch { }
                                }
                                else
                                {
                                    if (text.EndsWith("PART :" + channel))
                                    {
                                        string str = text.Substring(1, text.IndexOf("!") - 1);
                                        dynamic[] player3 = MainMod.Player;
                                        if (player3 == null) return;
                                        for (int j = 0; j < player3.Length; j++)
                                        {
                                            dynamic player4 = player3[j];
                                            if (player4.active)
                                            {
                                                if (player4.name.Length > 0)
                                                {
                                                    Session.Sessions[player4.whoAmi].SendText(255, 255, 255, "[IRC] <" + str + "> has left IRC");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (text.StartsWith("PING"))
                                        {
                                            IRC.send("PONG " + array2[1]);
                                        }
                                        else
                                        {
                                            if (array2[1] == "PRIVMSG" && array2[2].ToLower() == channel.ToLower())
                                            {
                                                Regex regex = new Regex(":(.*) PRIVMSG (.*) :(.*)");
                                                Match match = regex.Match(text);
                                                if (match.Success)
                                                {
                                                    if (match.Groups[3].Value.StartsWith(TerraIRC.commandPrefix))
                                                    {
                                                        string command = match.Groups[3].Value.Remove(0, 1).ToLower();
                                                        string[] commandArray = match.Groups[3].Value.Split(' ');
                                                        Console.WriteLine(match.Groups[3].Value);
                                                        switch (command)
                                                        {
                                                            case "players":
                                                                IRCCommands.Players();
                                                                break;

                                                            case "finger":
                                                                IRCCommands.Finger();
                                                                break;

                                                            case "terrairc":
                                                                IRCCommands.reloadSettings();
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        string nickname = text.Substring(1, text.IndexOf("!") - 1);
                                                        dynamic[] player5 = MainMod.Player;
                                                        if (player5 == null) return;
                                                        for (int k = 0; k < player5.Length; k++)
                                                        {
                                                            dynamic player6 = player5[k];
                                                            if (player6.active)
                                                            {
                                                                if (player6.name.Length > 0)
                                                                {
                                                                    string text2 = "[IRC] <" + nickname + "> " + match.Groups[3].Value;
                                                                    Session.Sessions[player6.whoAmi].SendText(255, 255, 255, text2.Replace(oldChar, '*'));
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch (EndOfStreamException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}