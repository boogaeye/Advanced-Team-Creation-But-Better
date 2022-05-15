using Exiled.API.Features;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBB.TeamAPI
{
    public class LeaderboardHelper
    {
        public List<TeamLeaderboard> TeamLeaderboards = new List<TeamLeaderboard>();
        public static bool TeamsInstantiable = false;
        public class TeamLeaderboard
        {
            public static AdvancedTeamSubclass DEFAULTSUBCLASS = new AdvancedTeamSubclass() { Name = "DEFAULT", AdvancedTeam = "DEFAULT" };
            public TeamLeaderboard(AdvancedTeam at)
            {
                Team = at;
            }
            public AdvancedTeam Team;
            public Dictionary<Player, AdvancedTeamSubclass> PlayerPairs = new Dictionary<Player, AdvancedTeamSubclass>();
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
                Log.Debug($"Looking at {tldr.Team.Name} but im looking for {name}", TeamPlugin.Singleton.Config.Debug);
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
