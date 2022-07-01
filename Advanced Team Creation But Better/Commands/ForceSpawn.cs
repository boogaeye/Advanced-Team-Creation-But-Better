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
    public class ForceSpawn : ICommand
    {
        public string Command => "forcespawn";

        public string[] Aliases { get; } = { "fspawn" };

        public string Description => "Forces A Spawn Wave";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string Team = arguments.Array[1];
            string SubTeam = arguments.Array[2];
            if (sender.CheckPermission("ATC.main"))
            {
                try
                {
                    if (TeamPlugin.Singleton.Config.FindAT(Team).VanillaTeam)
                    {
                        response = "This is a vanilla team use forcespawn for this...";
                        return false;
                    }

                    response = "Forced Spawn...";

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