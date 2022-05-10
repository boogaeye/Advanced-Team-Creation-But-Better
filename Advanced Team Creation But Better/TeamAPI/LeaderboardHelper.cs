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

        public class TeamLeaderboard
        {
            public TeamLeaderboard(AdvancedTeam at)
            {
                Team = at;
            }
            public AdvancedTeam Team;
            public List<Player> players = new List<Player>();
            public void AddPlayer(Player p)
            {
                players.Add(p);
            }

            public void RemovePlayer(Player p)
            {
                players.Remove(p);
            }
        }

        public void ClearPlayerFromLeaderBoards(Player ply)
        {
            foreach (TeamLeaderboard tldr in TeamLeaderboards)
            {
                if (tldr.players.Contains(ply))
                {
                    tldr.players.Remove(ply);
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
        }

        public void DestroyTeamLeaders()
        {
            TeamLeaderboards.Clear();
            Log.Debug("Cleared Team Leaderboards", TeamPlugin.Singleton.Config.Debug);
        }
    }
}
