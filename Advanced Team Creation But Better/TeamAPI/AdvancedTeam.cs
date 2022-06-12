using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API;
using Exiled.API.Features;
using System.ComponentModel;
using ATCBB.TeamAPI.CustomConfig;

namespace ATCBB.TeamAPI
{
    public class AdvancedTeam
    {
        public static readonly string DEFAULTAnnounce = "DEFAULT";
        public string Name { get; set; } = "No Team";
        public string DisplayName { get; set; } = "<color=black>NAN</color>";
        public RoomType SpawnRoom { get; set; } = RoomType.Surface;
        public int Chance { get; set; } = 75;
        public string[] SpawnOrder { get; set; } = { "Commander:1", "Officer:3", "Cadet:5" };
        [Description("Does the CASSIE Announcement play before spawning")]
        public bool PlayBeforeSpawning { get; set; } = false;
        public string CassieAnnouncement { get; set; } = DEFAULTAnnounce;
        public string CassieAnnouncementSubtitles { get; set; } = DEFAULTAnnounce;
        public int ChanceForHiddenMtfNato { get; set; } = 0;
        public LeaderboardConfigHelper RoundEnderConfig { get; set; } = new LeaderboardConfigHelper();
        public string Color { get; set; } = "yellow";
        [Description("Defines what class an escapee will become when escaping")]
        public string EscapeClass { get; set; } = "Cadet";
        [Description("Defines what classes are able to escape if they are not then they will become whatever teams escape default is (Chaos, NTF Specialist)")]
        public RoleType[] EscapableClasses { get; set; } = { RoleType.Scientist, RoleType.ClassD };

        public bool VanillaTeam;
        public bool Spectator;

        public bool ConfirmFriendshipWithTeam(AdvancedTeam at)
        {
            return RoundEnderConfig.FriendlyTeams.Contains(at.Name) || at.RoundEnderConfig.FriendlyTeams.Contains(Name);
        }

        public bool ConfirmEnemyshipWithTeam(AdvancedTeam at)
        {
            return RoundEnderConfig.RequiredTeams.Contains(at.Name) || at.RoundEnderConfig.RequiredTeams.Contains(Name);
        }

        public bool DoesExist()
        {
            return TeamPlugin.Singleton.TeamEventHandler.Leaderboard.TeamLeaderboards.First(e => e.Team.Name == Name).PlayerPairs.Keys.Any();
        }
    }
}