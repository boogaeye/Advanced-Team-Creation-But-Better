using System;
using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.Loader;
using System.IO;
using AdvancedTeamCreation.TeamAPI;
using AdvancedTeamCreation.TeamAPI.Helpers;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;

namespace AdvancedTeamCreation.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class UpdateAll : ICommand
    {
        public string Command => "updateall";

        public string[] Aliases { get; } = { "ua" };

        public string Description => "Updates all files for the game";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender.CheckPermission("ATC.main"))
            {
                response = $"Updated All Folders";
                foreach (AdvancedTeam at in UnitHelper.AdvancedTeamsOnly)
                {
                    File.WriteAllText(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, at.Name, "TeamConfig.yml"), Loader.Serializer.Serialize(at));
                    foreach (AdvancedTeamSubclass ast in UnitHelper.Subteams)
                    {
                        if (ast.AdvancedTeam == at.Name)
                        {
                            File.WriteAllText(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, at.Name, ast.Name + ".yml"), Loader.Serializer.Serialize(ast));
                            Log.Debug($"Updating Subteam {ast.Name}");
                        }
                    }
                    Log.Debug($"Updating Team {at.Name}");
                    return true;
                }

                return false;
            }

            response = TeamPlugin.Singleton.Translation.NoPermissions;
            return false;
        }
    }
}