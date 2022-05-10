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
                response = $"No Such Team...";
                foreach (AdvancedTeam at in TeamPlugin.Singleton.Config.Teams)
                {
                    if (at.Name == Team)
                    {
                        foreach (AdvancedTeamSubclass asc in TeamPlugin.Singleton.Config.SubTeams)
                        {
                            if (asc.Name == SubTeam)
                            {
                                p.ChangeAdvancedRole(at, asc);
                                response = "Player Team Changed";
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            response = TeamPlugin.Singleton.Translation.NoPermissions;
            return false;
        }
    }
}
