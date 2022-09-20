using System;
using System.Collections.Generic;
using CommandSystem;
using AdvancedTeamCreation.TeamAPI;
using Exiled.API.Features;
using AdvancedTeamCreation.TeamAPI.Helpers;
using static AdvancedTeamCreation.TeamAPI.Helpers.LeaderboardHelper;

namespace AdvancedTeamCreation.Commands
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
            foreach (TeamLeaderboard tb in RespawnHelper.Leaderboard.TeamLeaderboards)
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