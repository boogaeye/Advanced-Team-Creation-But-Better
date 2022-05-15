using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBB.TeamAPI.CustomEventHelpers
{
    public class CustomRoundEnder
    {
        public static bool RoundEnded = false;
        public static List<RoundEndStats> LeaderboardStats = new List<RoundEndStats>();
        public class RoundEndStats
        {
            public string Name;
            public string Value;
        }

        public void RoundStartHelper()
        {
            if (TeamPlugin.Singleton.Config.CustomRoundEnder)
            {
                Round.IsLocked = true;
            }
        }

        public void EndRound(string TeamWon)
        {
            RoundEnded = true;
            string HintText = $"{TeamWon} Wins\n\n- Stats -\n";
            foreach (RoundEndStats stats in LeaderboardStats)
            {
                HintText += $"{stats.Name}: {stats.Value}\n";
            }
            foreach (Player p in Player.List)
            {
                p.ShowHint(HintText, 10);
            }
            MEC.Timing.CallDelayed(10, () =>
            {
                Round.Restart();
                LeaderboardStats.Clear();
                RoundEnded = false;
            });
        }
    }
}
