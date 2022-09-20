using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.Loader;
using System.IO;
using AdvancedTeamCreation.TeamAPI;
using AdvancedTeamCreation.TeamAPI.Extentions;
using Exiled.API.Features;
using AdvancedTeamCreation.TeamAPI.Helpers;

namespace AdvancedTeamCreation.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceSpawn : ICommand
    {
        public string Command => "forcespawn";

        public string[] Aliases { get; } = { "fspawn" };

        public string Description => "Forces A Spawn Wave";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string Team = arguments.Array[1];
            if (sender.CheckPermission("ATC.forcespawn"))
            {
                try
                {
                    if (UnitHelper.FindAT(Team).VanillaTeam)
                    {
                        response = "This is a vanilla team use forcespawn for this...";
                        return false;
                    }

                    response = $"Next spawn will be {Team}";
                    RespawnHelper.SetReferance(UnitHelper.FindAT(Team));
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