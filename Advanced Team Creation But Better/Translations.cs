using Exiled.API.Interfaces;
using System.Collections.Generic;

namespace AdvancedTeamCreation
{
    public class Translations : ITranslation
    {
        public string NoPermissions { get; set; } = "You can't use that here";
        public string TeamIsFriendlyHint { get; set; } = "<color=green>(user)((role)) is friendly from (dist) units away</color>";
        public string TeamIsHostileHint { get; set; } = "<color=red>(user) is hostile</color>";
        public string TeamIsNeutralHint { get; set; } = "<color=grey>(user)((role)) is neutral</color>";
        public string TeamIsRequiredHint { get; set; } = "<color=yellow>Help (user)((role)) escape!</color>";
        public string FriendlyTeamHeader { get; set; } = "<align=right><size=45%><color=green>Friendly Teams:</color></size></align>";
        public string FriendlyTeamListed { get; set; } = "<align=right><size=45%>(TEAM)</size></align>";
        public string HostileTeamHeader { get; set; } = "<align=right><size=45%><color=red>Hostile Teams:</color></size></align>";
        public string HostileTeamListed { get; set; } = "<align=right><size=45%>(TEAM)</size></align>";
        public string NeutralTeamHeader { get; set; } = "<align=right><size=45%><color=grey>Neutral Teams:</color></size></align>";
        public string NeutralTeamListed { get; set; } = "<align=right><size=45%>(TEAM)</size></align>";
        public string EscapeTeamHeader { get; set; } = "<align=right><size=45%><color=yellow>Help These Teams(Escape):</color></size></align>";
        public string EscapeTeamListed { get; set; } = "<align=right><size=45%><color=yellow>(TEAMNoColor)</color></size></align>";
        public string ElapsedTimeStat { get; set; } = "<color=yellow>Elapsed Time</color>";
        public string ScpKillsStat { get; set; } = "<color=red>SCP Kills</color>";
        public string KillsStat { get; set; } = "Kills";
        public string ScpPercentKillsStat { get; set; } = "Percent Kills:SCPKills";
        public string DClassEscapeeStat { get; set; } = "<color=orange>DClass Escapes</color>";
        public string ScientistEscapeeStat { get; set; } = "<color=yellow>Scientist Escapes</color>";
        public string RoundsStat { get; set; } = "Rounds Completed";
        public string RoundWonStat { get; set; } = "{TeamWon} Wins\n\n- Stats -\n";
        public string TeamKillsStat { get; set; } = "{Team} killed";

        public Dictionary<Team, string> TeamCassieSlaughter { get; set; } = new Dictionary<Team, string>()
        {
            { Team.CDP, "pitch_0.5 .g4 .g4 .g4 pitch_1 all {Terminated} terminated by {Terminating} . allremaining awaitingrecontainment jam_1_5 {SCPLeft} scpsubjects" },
        };

        public Dictionary<Team, string> TeamCassieSlaughterSubtitles { get; set; } = new Dictionary<Team, string>()
        {
            { Team.CDP, "all {Terminated} terminated by {Terminating} all remaining personal are advised to proceed with standard evacuation protocols until an MTF squad reaches your destination. awaiting recontainment of {SCPLeft} scp subjects." }
        };
    }
}