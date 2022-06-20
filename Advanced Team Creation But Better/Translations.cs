using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using Exiled.API;
using System.IO;
using Exiled.Loader;
using Exiled.API.Features;
using ATCBB.TeamAPI;

namespace ATCBB
{
    public class Translations : ITranslation
    {
        public string NoPermissions { get; set; } = "You can't use that here";
        public string TeamIsFriendlyHint { get; set; } = "<color=green>(user)((role)) is friendly from (dist) units away</color>";
        public string TeamIsHostileHint { get; set; } = "<color=red>(user) is hostile</color>";
        public string TeamIsNeutralHint { get; set; } = "<color=grey>(user)((role)) is neutral</color>";
        public string TeamIsRequiredHint { get; set; } = "<color=yellow>Help (user)((role)) escape!</color>";
        public string TopTeamList { get; set; } = "<align=right><size=45%>You are the (TEAM)</size></align>";
        public string FriendlyTeamHeader { get; set; } = "<align=right><size=45%><color=green>Friendly Teams:</color></size></align>";
        public string FriendlyTeamListed { get; set; } = "<align=right><size=45%>(TEAM)</size></align>";
        public string HostileTeamHeader { get; set; } = "<align=right><size=45%><color=red>Hostile Teams:</color></size></align>";
        public string HostileTeamListed { get; set; } = "<align=right><size=45%>(TEAM)</size></align>";
        public string NeutralTeamHeader { get; set; } = "<align=right><size=45%><color=grey>Neutral Teams:</color></size></align>";
        public string NeutralTeamListed { get; set; } = "<align=right><size=45%>(TEAM)</size></align>";
        public string EscapeTeamHeader { get; set; } = "<align=right><size=45%><color=yellow>Help These Teams(Escape):</color></size></align>";
        public string EscapeTeamListed { get; set; } = "<align=right><size=45%><color=yellow>(TEAMNoColor)</color></size></align>";
    }
}