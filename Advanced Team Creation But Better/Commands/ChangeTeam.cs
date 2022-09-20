using System;
using CommandSystem;
using Exiled.Permissions.Extensions;
using AdvancedTeamCreation.TeamAPI.Extentions;
using Exiled.API.Features;
using AdvancedTeamCreation.TeamAPI.Helpers;

namespace AdvancedTeamCreation.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ChangeTeam : ICommand
    {
        public string Command => "changeteam";

        public string[] Aliases { get; } = { "cht" };

        public string Description => "Changes a player to a team for the game";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(arguments.Array[1]);
            string Team = arguments.Array[2];
            string SubTeam = arguments.Array[3];
            if (sender.CheckPermission("ATC.main"))
            {
                try
                {
                    if (UnitHelper.FindAT(Team).VanillaTeam)
                    {
                        response = "This is a vanilla team use forceclass for this...";
                        return false;
                    }
                    p.ChangeAdvancedRole(UnitHelper.FindAT(Team), UnitHelper.FindAST(Team, SubTeam));
                    response = "Changed Team";

                    return true;
                }
                catch (Exception)
                {
                    response = "Team or SubTeam Doesn't Exist";
                    return false;
                }
            }

            response = TeamPlugin.Singleton.Translation.NoPermissions;
            return false;
        }
    }
}