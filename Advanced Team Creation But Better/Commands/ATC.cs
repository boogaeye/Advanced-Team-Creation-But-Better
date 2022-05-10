using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace ATCBB.Commands
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
                    response = "You need to enter an argument\n<b>forcenextteam</b>\n<b>forceteam</b>\n<b>teamsalive</b>\n<b>reload</b>";
                    return true;
                }
                else if (arguments.Contains("reload"))
                {
                    if (sender.CheckPermission("ATC.reload"))
                    {
                        TeamPlugin.Singleton.Config.LoadTeamConfigs();
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
