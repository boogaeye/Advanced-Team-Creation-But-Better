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
    public class FakeTermination : ICommand
    {
        public string Command => "FakeTermination";

        public string[] Aliases { get; } = { "fterm", "terminate" };

        public string Description => "Sends a cassie termination message for teams";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string Team = arguments.Array[1];
            string TermTeam = arguments.Array[2];
            TeamConfig c = TeamPlugin.Singleton.Config;
            if (sender.CheckPermission("ATC.main"))
            {
                try
                {
                    TeamPlugin.Singleton.TeamEventHandler.SlaughteredTeam(c.FindAT(Team), c.FindAT(TermTeam));
                    response = "Faked Team Termination";

                    return true;
                }
                catch (Exception)
                {
                    response = "Team or TerminatingTeam or Both Doesn't Exist";
                    return false;
                }
            }

            response = TeamPlugin.Singleton.Translation.NoPermissions;
            return false;
        }
    }
}