﻿using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClanBot_Service
{
    static class Help
    {
        public static void RegisterHelpCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Help")
                .Description("!help - get a list of all clanbot commands.")
                .AddCheck((command, user, channel) => !user.IsBot) 
                .Do(async (e) =>
                {
                    string text = "**CLANBOT COMMANDS**\n```";

                    foreach (Discord.Commands.Command c in commands.AllCommands)
                    {
                        if (c.IsHidden)
                            text += "" + c.Description.ToLower() + " **admin only**\n";
                        else
                            text += "" + c.Description.ToLower() + "\n";
                    }
                    text += "```";
                    await e.Channel.SendMessage(text);

                });
        }
    }
}
