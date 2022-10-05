using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;
using static AdvancedTeamCreation.TeamAPI.CustomEvents.CustomRoundEnder;

namespace AdvancedTeamCreation.TeamAPI.Helpers
{
    public class LeaderboardHelper
    {
        public List<TeamLeaderboard> TeamLeaderboards = new List<TeamLeaderboard>();
        public static bool TeamsInstantiable = false;

        public class TeamLeaderboard
        {
            public AdvancedTeamSubclass SubclassDefault => Team.LastIndexSubclass;

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

            public RoundEndStats GetStat(string Name)
            {
                foreach (RoundEndStats res in Stats)
                {
                    if (res.Name == Name)
                    {
                        return res;
                    }
                }
                return null;
            }

            public void AddPlayer(Player p, AdvancedTeamSubclass advancedTeamSubclass)
            {
                PlayerPairs[p] = advancedTeamSubclass;
            }

            public void AddPlayer(Player p)
            {
                PlayerPairs[p] = SubclassDefault;
                Log.Debug($"{p.Nickname} added to {Team.Name} Leaderboard", TeamPlugin.Singleton.Config.Debug);
            }

            public void RemovePlayer(Player p)
            {
                PlayerPairs.Remove(p);
                Log.Debug($"{p.Nickname} removed to {Team.Name} Leaderboard", TeamPlugin.Singleton.Config.Debug);
            }
        }

        public void ClearPlayerFromLeaderBoards(Player ply)
        {
            if (!TeamsInstantiable) return;
            foreach (TeamLeaderboard tldr in TeamLeaderboards)
            {
                if (tldr.PlayerPairs.Keys.Contains(ply))
                {
                    tldr.RemovePlayer(ply);
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
            foreach (AdvancedTeam at in UnitHelper.Teams)
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