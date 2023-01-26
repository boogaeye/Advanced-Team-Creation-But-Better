using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using System.ComponentModel;
using AdvancedTeamCreation.TeamAPI.CustomConfig;
using Respawning;
using AdvancedTeamCreation.TeamAPI.Helpers;
using PlayerRoles;

namespace AdvancedTeamCreation.TeamAPI
{
    public class AdvancedTeam
    {
        public static readonly string DEFAULTAnnounce = "DEFAULT";
        public bool Spectator;
        public bool VanillaTeam;
        public string Name = "No Team";
        public AdvancedTeamSubclass LastIndexSubclass;

        [Description("What team should it only spawn as if its none it will spawn as either or")]
        public SpawnableTeamType BindedTeam { get; set; } = SpawnableTeamType.None;

        public string CassieAnnouncement { get; set; } = DEFAULTAnnounce;
        public string CassieAnnouncementSubtitles { get; set; } = DEFAULTAnnounce;

        [Description("Cassie announcement for when the team fully is killed (Make this empty if you don't want it to play one)")]
        public string CassieSlaughtered { get; set; } = "{Terminated} terminated by {Terminating}";

        public string CassieSlaughteredSubtitles { get; set; } = "{Terminated} terminated by {Terminating}";

        public int Chance { get; set; } = 75;
        public int ChanceForHiddenMtfNato { get; set; } = 0;
        public string Color { get; set; } = "yellow";
        public string DisplayName { get; set; } = "<color=black>NAN</color>";

        [Description("Defines what classes are able to escape if they are not then they will become whatever teams escape default is (Chaos, NTF Specialist)")]
        public RoleTypeId[] EscapableClasses { get; set; } = { RoleTypeId.Scientist, RoleTypeId.ClassD };

        [Description("Defines what class an escapee will become when escaping")]
        public string EscapeClass { get; set; } = "Cadet";

        [Description("Does the CASSIE Announcement play before spawning")]
        public bool PlayBeforeSpawning { get; set; } = false;

        public LeaderboardConfigHelper RoundEnderConfig { get; set; } = new LeaderboardConfigHelper();

        [Description("Used for when and Scp gets terminated by this team typically if it was the global occult coalition it would look like 'G O C'")]
        public string SaidName { get; set; } = "Unspecified";

        public string[] SpawnOrder { get; set; } = { "Commander:1", "Officer:3", "Cadet:5" };
        public RoomType SpawnRoom { get; set; } = RoomType.Surface;

        public bool ConfirmEnemyshipWithTeam(AdvancedTeam at)
        {
            return (RoundEnderConfig.RequiredTeams.Contains(at.Name) || at.RoundEnderConfig.RequiredTeams.Contains(Name)) && !ConfirmRequiredshipWithTeam(at);
        }

        public bool ConfirmFriendshipWithTeam(AdvancedTeam at)
        {
            return (RoundEnderConfig.FriendlyTeams.Contains(at.Name) || at.RoundEnderConfig.FriendlyTeams.Contains(Name) || at.Name == Name) && !ConfirmRequiredshipWithTeam(at);
        }

        public bool ConfirmNeutralshipWithTeam(AdvancedTeam at)
        {
            return !(RoundEnderConfig.RequiredTeams.Contains(at.Name) || at.RoundEnderConfig.RequiredTeams.Contains(Name)) && !(RoundEnderConfig.FriendlyTeams.Contains(at.Name) || at.RoundEnderConfig.FriendlyTeams.Contains(Name) || at.Name == Name);
        }

        public bool ConfirmRequiredshipWithTeam(AdvancedTeam at)
        {
            return (RoundEnderConfig.RequiredTeams.Contains(at.Name) || at.RoundEnderConfig.RequiredTeams.Contains(Name)) && (RoundEnderConfig.FriendlyTeams.Contains(at.Name) || at.RoundEnderConfig.FriendlyTeams.Contains(Name) || at.Name == Name);
        }

        public bool DoesExist()
        {
            return RespawnHelper.Leaderboard.TeamLeaderboards.First(e => e.Team.Name == Name).PlayerPairs.Keys.Any();
        }

        public AdvancedTeam[] GetAllFriendlyTeams()
        {
            List<AdvancedTeam> atlist = new List<AdvancedTeam>();
            foreach (AdvancedTeam at in UnitHelper.Teams)
            {
                if (ConfirmFriendshipWithTeam(at) && !at.Spectator)
                {
                    atlist.Add(at);
                }
            }
            return atlist.ToArray();
        }

        public AdvancedTeam[] GetAllHostileTeams()
        {
            List<AdvancedTeam> atlist = new List<AdvancedTeam>();
            foreach (AdvancedTeam at in UnitHelper.Teams)
            {
                if (ConfirmEnemyshipWithTeam(at) && !ConfirmFriendshipWithTeam(at) && !at.Spectator)
                {
                    atlist.Add(at);
                }
            }
            return atlist.ToArray();
        }

        public AdvancedTeam[] GetAllNeutralTeams()
        {
            List<AdvancedTeam> atlist = new List<AdvancedTeam>();
            foreach (AdvancedTeam at in UnitHelper.Teams)
            {
                if (ConfirmNeutralshipWithTeam(at) && !at.Spectator)
                {
                    atlist.Add(at);
                }
            }
            return atlist.ToArray();
        }

        public AdvancedTeam[] GetAllRequiredTeams()
        {
            List<AdvancedTeam> atlist = new List<AdvancedTeam>();
            foreach (AdvancedTeam at in UnitHelper.Teams)
            {
                if (ConfirmRequiredshipWithTeam(at) && !at.Spectator)
                {
                    atlist.Add(at);
                }
            }
            return atlist.ToArray();
        }
    }
}