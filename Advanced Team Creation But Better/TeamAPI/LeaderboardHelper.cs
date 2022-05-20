using Exiled.API.Features;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ATCBB.TeamAPI.CustomEventHelpers.CustomRoundEnder;

namespace ATCBB.TeamAPI
{
    public class LeaderboardHelper
    {
        public List<TeamLeaderboard> TeamLeaderboards = new List<TeamLeaderboard>();
        public static bool TeamsInstantiable = false;
        public class TeamLeaderboard
        {
            public static readonly AdvancedTeamSubclass DEFAULTSUBCLASS = new AdvancedTeamSubclass() { Name = "DEFAULT", AdvancedTeam = "DEFAULT" };
            public TeamLeaderboard(AdvancedTeam at)
            {
                Team = at;
            }
            public AdvancedTeam Team;
            public Dictionary<Player, AdvancedTeamSubclass> PlayerPairs = new Dictionary<Player, AdvancedTeamSubclass>();
            public List<RoundEndStats> Stats = new List<RoundEndStats>();
            public void UpdateStat(string Name, string Value)
            {
                foreach (RoundEndStats res in Stats)
                {
                    if (res.Name == Name)
                    {
                        Stats.Remove(res);
                        Stats.Add(new RoundEndStats(Name, Value));
                        return;
                    }
                }
                Stats.Add(new RoundEndStats(Name, Value));
            }
            public void AddPlayer(Player p, AdvancedTeamSubclass advancedTeamSubclass)
            {
                PlayerPairs[p] = advancedTeamSubclass;
            }

            public void AddPlayer(Player p)
            {
                PlayerPairs[p] = DEFAULTSUBCLASS;
            }

            public void RemovePlayer(Player p)
            {
                PlayerPairs.Remove(p);
            }
        }

        public void ClearPlayerFromLeaderBoards(Player ply)
        {
            if (!TeamsInstantiable) return;
            foreach (TeamLeaderboard tldr in TeamLeaderboards)
            {
                if (tldr.PlayerPairs.Keys.Contains(ply))
                {
                    tldr.PlayerPairs.Remove(ply);
                }
            }
        }

        public TeamLeaderboard GetTeamLeaderboard(string name)
        {
            foreach (TeamLeaderboard tldr in TeamLeaderboards)
            {
                if (tldr.Team.Name == name)
                {
                    return tldr;
                }
            }
            return null;
        }

        public void SetUpTeamLeaders()
        {
            foreach (AdvancedTeam at in TeamPlugin.Singleton.Config.Teams)
            {
                TeamLeaderboards.Add(new TeamLeaderboard(at));
                Log.Debug($"Made Team Leaderboard {at.Name}", TeamPlugin.Singleton.Config.Debug);
            }
            TeamsInstantiable = true;
        }

        public void DestroyTeamLeaders()
        {
            TeamLeaderboards.Clear();
            Log.Debug("Cleared Team Leaderboards", TeamPlugin.Singleton.Config.Debug);
            TeamsInstantiable = false;
        }
    }
}
