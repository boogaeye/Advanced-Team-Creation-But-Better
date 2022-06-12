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
using ATCBB.TeamAPI.CustomEventHelpers;

namespace ATCBB.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SimulateRoundEnd : ICommand
    {
        public string Command => "simulateroundend";

        public string[] Aliases { get; } = { "sre" };

        public string Description => "if custom round ender is enabled it will simulate a round end event";


        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string teamName = arguments.Array[1];
            if (sender.CheckPermission("ATC.main"))
            {
                response = $"Simulating Round Win for {arguments.Array[1]}";
                AdvancedTeam at = TeamPlugin.Singleton.Config.FindAT(teamName);
                CustomRoundEnder.EndRound(at.DisplayName);
                return true;
            }

            response = TeamPlugin.Singleton.Translation.NoPermissions;
            return false;
        }
    }
}
