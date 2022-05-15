using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.Loader;
using System.IO;
using ATCBB.TeamAPI;
using ATCBB.TeamAPI.Extentions;
using Exiled.API.Features;
using static ATCBB.TeamAPI.LeaderboardHelper;

namespace ATCBB.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListTeams : ICommand
    {
        public string Command => "listteams";

        public string[] Aliases { get; } = { "lt" };

        public string Description => "Displays the list of teams";


        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string RHelper = "- Teams List -\n";
            foreach (TeamLeaderboard tb in TeamPlugin.Singleton.TeamEventHandler.Leaderboard.TeamLeaderboards)
            {
                RHelper += $"\n{tb.PlayerPairs.Count}< {tb.Team.Name} >";
                foreach (KeyValuePair<Player, AdvancedTeamSubclass> p in tb.PlayerPairs)
                {
                    RHelper += $"\n{p.Key.Id} | {p.Key.Nickname} | {p.Value.Name}";
                }
            }
            response = RHelper;
            return true;
        }
    }
}
