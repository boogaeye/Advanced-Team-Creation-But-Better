using System;
using CommandSystem;
using Exiled.Permissions.Extensions;
using AdvancedTeamCreation.TeamAPI.Helpers;

namespace AdvancedTeamCreation.Commands
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
            if (sender.CheckPermission("ATC.main"))
            {
                try
                {
                    MessageHelper.SlaughteredTeam(UnitHelper.FindAT(Team), UnitHelper.FindAT(TermTeam));
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