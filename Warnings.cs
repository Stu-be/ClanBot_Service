using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClanBot_Service
{
    static class Warnings
    {
        public static void RegisterListWarningsCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Warnings")
                .Description("!warnings - list all member warnings.")
                .AddCheck((command, user, channel) => !user.IsBot)
                .Do(async (e) =>
                {
                    try
                    {
                        string message = "**MEMBER WARNINGS** \n";

                        ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                        List<Warning> warnings = new List<Warning>();
                        warnings = (from w in dc.Warnings
                                    select w).ToList();
                        message += "```";
                        foreach (Warning w in warnings)
                        {
                            message += "| " + w.WarningName + " = " + w.WarningDescription + " ";
                        }
                        message += "|``` ```";
                        foreach (Warning w in warnings)
                        {
                            message += "" + w.WarningName + ": ";
                            List<UserWarning> userwarnings = new List<UserWarning>();
                            userwarnings = (from uw in dc.UserWarnings
                                            where uw.WarningId == w.WarningId
                                            select uw).Distinct().ToList();
                            userwarnings = userwarnings
                              .GroupBy(uw => uw.UserId)
                              .Select(g => g.First())
                              .ToList();
                            foreach (UserWarning uw in userwarnings)
                            {
                                User user = (from u in dc.Users
                                             where u.UserId == uw.UserId
                                             select u).FirstOrDefault();
                                int count = (from uwCount in dc.UserWarnings
                                             where uwCount.UserId == user.UserId
                                             && uwCount.WarningId == w.WarningId
                                             select uwCount).Count();
                                message += user.UserName + "(" + count + "), ";
                            }
                            message += "\n";
                        }
                        message += "```";
                        await e.Channel.SendMessage(message);
                    }
                    catch (Exception ex) { discord.Log.Error("RegisterGetClaimedBasesCommand", ex.Message); }
                });
        }

        public static void RegisterPlayerWarningCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Add Warning")
                .Description("!add warning [name] [warning] - add a player warning!")
                .AddCheck((command, user, channel) => !user.IsBot)
                .AddCheck((command, user, channel) => user.Roles.FirstOrDefault().Name == "@Co-Leader")
                .Parameter("name", ParameterType.Required)
                .Parameter("warning", ParameterType.Required)
                .Do(async (e) =>
                {
                    try
                    {
                        ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                        User user = (from u in dc.Users
                                     where u.UserName.ToLower().Equals(e.GetArg(0).ToLower())
                                     select u).First();
                        if (user != null)
                        {
                            Warning warning = (from w in dc.Warnings
                                               where w.WarningName.ToUpper() == e.GetArg(1).ToUpper()
                                               select w).First();
                            if (warning != null)
                            {
                                UserWarning uw = new UserWarning { UserId = user.UserId, WarningId = warning.WarningId, WarningDateTime = DateTime.UtcNow };
                                dc.UserWarnings.InsertOnSubmit(uw);
                                dc.SubmitChanges();
                                await e.Channel.SendMessage(warning.WarningName + " WARNING ADDED TO " + e.GetArg(0).ToUpper());
                            }
                            else
                            {
                                await e.Channel.SendMessage(e.GetArg(1).ToUpper() + " IS NOT A VALID WARNING CODE!");
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessage(e.GetArg(0).ToUpper() + " NOT FOUND!");
                        }
                    }
                    catch (Exception ex) { discord.Log.Error("RegisterGetClaimedBasesCommand", ex.Message); }
                });
        }

        public static void RegisterRemovePlayerWarningCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("Remove Warning")
                .Description("!remove warning [name] - remove players last warning!")
                .AddCheck((command, user, channel) => !user.IsBot)
                .AddCheck((command, user, channel) => user.Roles.FirstOrDefault().Name == "@Co-Leader")
                .Parameter("name", ParameterType.Required)
                .Do(async (e) =>
                {
                    try
                    {
                        ClasherDynastyDataContext dc = new ClasherDynastyDataContext();
                        User user = (from u in dc.Users
                                     where u.UserName.ToLower().Contains(e.GetArg(0).ToLower())
                                     select u).First();
                        if (user != null)
                        {

                            UserWarning uw = (from warning in dc.UserWarnings
                                              where warning.UserId == user.UserId
                                              orderby warning.WarningDateTime descending
                                              select warning).First();
                            if (uw != null)
                            {
                                dc.UserWarnings.DeleteOnSubmit(uw);
                                dc.SubmitChanges();
                                string warning = (from w in dc.Warnings where w.WarningId == uw.WarningId select w.WarningName).ToString();
                                await e.Channel.SendMessage(warning.ToUpper() + " WARNING REMOVED FROM " + e.GetArg(0).ToUpper());
                            }
                            else
                            {
                                await e.Channel.SendMessage("NO WARNINGS NOT FOUND FOR " + e.GetArg(0).ToUpper() + "!");
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessage(e.GetArg(0).ToUpper() + " NOT FOUND!");
                        }
                        await e.Channel.SendMessage("WARING LOGGED AGAINST " + e.GetArg(0).ToUpper());
                    }
                    catch (Exception ex) { discord.Log.Error("RegisterGetClaimedBasesCommand", ex.Message); }
                });
        }
    }
}
