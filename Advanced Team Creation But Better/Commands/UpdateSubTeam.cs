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

namespace ATCBB.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class UpdateSubTeam : ICommand
    {
        public string Command => "createsubteam";

        public string[] Aliases { get; } = { "cst" };

        public string Description => "Updates a sub team for a team";


        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string Team = arguments.Array[1];
            string SubTeamName = arguments.Array[2];
            if (sender.CheckPermission("ATC.main"))
            {
                if (Directory.Exists(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, Team)))
                {
                    response = $"Updated SubTeam Folder";
                    AdvancedTeamSubclass at = TeamPlugin.Singleton.Config.FindAST(Team, SubTeamName);
                    File.WriteAllText(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, Team, SubTeamName + ".yml"), Loader.Serializer.Serialize(at));
                    return true;
                }
                else
                {
                    response = $"Directory Doesn't exist";
                    return false;
                }
            }

            response = TeamPlugin.Singleton.Translation.NoPermissions;
            return false;
        }
    }
}
