using System;
using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.Loader;
using System.IO;
using AdvancedTeamCreation.TeamAPI;
using PlayerRoles;

namespace AdvancedTeamCreation.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CreateSubTeam : ICommand
    {
        public string Command => "createsubteam";

        public string[] Aliases { get; } = { "cst" };

        public string Description => "Creates a sub team for a team";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string Team = arguments.Array[1];
            string SubTeamName = arguments.Array[2];
            if (sender.CheckPermission("ATC.main"))
            {
                if (Directory.Exists(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, Team)))
                {
                    response = $"Created SubTeam Folder";
                    AdvancedTeamSubclass at = new AdvancedTeamSubclass() { Name = SubTeamName, HP = 100, MaxHP = 100, Model = RoleTypeId.ClassD };
                    Directory.CreateDirectory(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, Team));
                    File.Create(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, Team, SubTeamName + ".yml")).Close();
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