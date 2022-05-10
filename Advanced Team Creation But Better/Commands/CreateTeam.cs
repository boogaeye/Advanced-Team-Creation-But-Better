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
    public class CreateTeam : ICommand
    {
        public string Command => "createteam";

        public string[] Aliases { get; } = { "ct" };

        public string Description => "Creates a team for the game";


        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender.CheckPermission("ATC.main"))
            {
                response = $"Created Team Folder {arguments.Array[1]}";
                AdvancedTeam at = new AdvancedTeam() { Name = arguments.Array[1], SpawnRoom = Exiled.API.Enums.RoomType.Surface };
                Directory.CreateDirectory(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, arguments.Array[1]));
                File.Create(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, arguments.Array[1], "TeamConfig.yml")).Close();
                File.WriteAllText(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, arguments.Array[1], "TeamConfig.yml"), Loader.Serializer.Serialize(at));
                return true;
            }

            response = TeamPlugin.Singleton.Translation.NoPermissions;
            return false;
        }
    }
}
