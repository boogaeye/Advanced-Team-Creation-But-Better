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
    public class UpdateTeam : ICommand
    {
        public string Command => "updateteam";

        public string[] Aliases { get; } = { "ut" };

        public string Description => "Updates a team for the game";


        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string teamName = arguments.Array[1];
            if (sender.CheckPermission("ATC.main"))
            {
                response = $"Updated Team Folder {arguments.Array[1]}";
                AdvancedTeam at = TeamPlugin.Singleton.Config.FindAT(teamName);
                if (Directory.Exists(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, teamName)))
                {
                    File.WriteAllText(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, teamName, "TeamConfig.yml"), Loader.Serializer.Serialize(at));

                    return true;
                }
                return false;
            }

            response = TeamPlugin.Singleton.Translation.NoPermissions;
            return false;
        }
    }
}
