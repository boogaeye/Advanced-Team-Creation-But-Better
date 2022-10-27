using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

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
        public string Stalemate { get; set; } = "STALEMATE";
        public string InterferenceText { get; set; } = "<color=black>INTERFERENCE</color>";

        [Description(": makes the words after it goto the next line")]
        public Dictionary<Team, string> TeamDescriptions { get; set; } = new Dictionary<Team, string>()
        {
            { Team.CDP, "You are a <color=orange>Class D</color>,:Find a <color=yellow>Keycard</color> and Escape!" },
            { Team.RSC, "You are a <color=yellow>Scientist</color>,:Upgrade your <color=yellow>Keycard</color>:Get help and Escape!" },
            { Team.SCP, "You are a <color=red>SCP</color>,:<color=red>Kill everyone...</color>" },
            { Team.CHI, "You are <color=green>The Insurgency</color>,:take <color=red>SCP</color> objects:Help <color=orange>Class D's</color> escape..." },
            { Team.MTF, "You are the <color=blue>Mobile Task Force</color>,:take out <color=red>SCP Subjects</color>:Terminate <color=green>Hostile</color> Forces:Help <color=yellow>Scientists</color> escape..." },
        };

        public Dictionary<Team, string> TeamDisplayers { get; set; } = new Dictionary<Team, string>()
        {
            { Team.CDP, "<color=orange>Class D Personal Escapee's</color>" },
            { Team.RSC, "<color=yellow>Research Personal Escapee's</color>" },
            { Team.SCP, "<color=red>SCP's</color>" },
            { Team.CHI, "<color=green>The Insurgency</color>" },
            { Team.MTF, "<color=blue>Mobile Task Force</color> and <color=grey>Facility Forces</color>" },
        };

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