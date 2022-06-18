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

namespace ATCBB.Commands
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
                    if (TeamPlugin.Singleton.Config.FindAT(Team).VanillaTeam)
                    {
                        response = "This is a vanilla team use forceclass for this...";
                        return false;
                    }
                    p.ChangeAdvancedRole(TeamPlugin.Singleton.Config.FindAT(Team), TeamPlugin.Singleton.Config.FindAST(Team, SubTeam));
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
