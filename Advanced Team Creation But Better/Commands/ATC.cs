using System;
using System.Linq;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace AdvancedTeamCreation.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ATC : ParentCommand
    {
        public override string Command => "advanceteamcreation";

        public override string[] Aliases { get; } = { "atc", "adt", "adtm" };

        public override string Description => "Advanced Team Creation Main Command";

        public override void LoadGeneratedCommands()
        {
            throw new NotImplementedException();
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender.CheckPermission("ATC.main"))
            {
                if (arguments.Count == 0)
                {
                    response = "You need to enter an argument\n<b>forcespawn (TeamName)</b>\n<b>ListTeams</b>\n<b>SimulateRoundEnd (TeamName)</b>\n<b>reload</b>\n<b>CreateTeam (TeamName NO SPACES)</b>\n<b>CreateSubteam (TeamName NO SPACES) (SubteamName NO SPACES)</b>\n<b>UpdateTeam (TeamName)</b>\n<b>UpdateSubteam (TeamName) (SubteamName)</b>";
                    return true;
                }
                else if (arguments.Contains("reload"))
                {
                    if (sender.CheckPermission("ATC.reload"))
                    {
                        TeamPlugin.Singleton.OnReloaded();
                        response = "Done";
                        return true;
                    }
                    else
                    {
                        response = TeamPlugin.Singleton.Translation.NoPermissions;
                        return false;
                    }
                }
                else
                {
                    response = "Invalid argument";
                    return false;
                }
            }

            response = TeamPlugin.Singleton.Translation.NoPermissions;
            return false;
        }
    }
}