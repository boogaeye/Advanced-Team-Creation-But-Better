using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEC;
namespace ATCBB.TeamAPI.CustomEventHelpers
{
    public class CustomRoundEnder
    {
        public static bool RoundEnded = false;
        public static List<RoundEndStats> LeaderboardStats = new List<RoundEndStats>();
        public class RoundEndStats
        {
            public RoundEndStats(string name, string val)
            {
                Name = name;
                Value = val;
            }
            public string Name;
            public string Value;
        }

        //public static void RoundStartHelper()
        //{
        //    if (TeamPlugin.Singleton.Config.CustomRoundEnder)
        //    {

        //    }
        //}

        public static void UpdateRoundStatus()
        {
            if (!TeamPlugin.Singleton.Config.CustomRoundEnder)
            {
                Log.Debug("Preventing UpdateRoundStatus CustomRoundEnder is false", TeamPlugin.Singleton.Config.Debug);
                return;
            }
            if (Round.ElapsedTime.TotalSeconds < 10)
            {
                Log.Debug($"Preventing Round Ending because it hasn't been 10 seconds in the round", TeamPlugin.Singleton.Config.Debug);
                return;
            }
            if (Round.IsLocked || Player.List.Count() == 1)
            {
                Log.Debug($"Round is locked or there is 1 player so preventing round end...", TeamPlugin.Singleton.Config.Debug);
                return;
            }
            foreach (LeaderboardHelper.TeamLeaderboard TL in TeamPlugin.Singleton.TeamEventHandler.Leaderboard.TeamLeaderboards)
            {
                if (TL.Team.DoesExist())
                {
                    bool CanWin = true;
                    Log.Debug($"Team {TL.Team.Name} does exist in current round", TeamPlugin.Singleton.Config.Debug);
                    foreach (LeaderboardHelper.TeamLeaderboard TL2 in TeamPlugin.Singleton.TeamEventHandler.Leaderboard.TeamLeaderboards)
                    {
                        if (!TL2.Team.Spectator)
                        {
                            if (TL.Team.ConfirmEnemyshipWithTeam(TL2.Team) && TL2.Team.DoesExist())
                            {
                                CanWin = false;
                                Log.Debug($"Team {TL.Team.Name} is enemies with {TL2.Team.Name}. They can no longer win.", TeamPlugin.Singleton.Config.Debug);
                                break;
                            }
                        }
                    }
                    if (TL.Team.Spectator)
                    {
                        CanWin = false;
                    }
                    if (CanWin)
                    {
                        EndRound(TL.Team.DisplayName);
                    }
                }
            }
        }

        public static void EndRound(string TeamWon)
        {
            if (!TeamPlugin.Singleton.Config.CustomRoundEnder) 
            {
                Log.Debug("Preventing Custom Round end CustomRoundEnder is false", TeamPlugin.Singleton.Config.Debug);
                return; 
            }
            RoundEnded = true;
            TeamPlugin.Singleton.TeamEventHandler.Leaderboard.DestroyTeamLeaders();
            if (TeamPlugin.assemblyTimer != null)
                Timing.KillCoroutines(RespawnTimer.EventHandler.timerCoroutine);
            LeaderboardStats.Add(new RoundEndStats("<color=yellow>Elapsed Time</color>", Round.ElapsedTime.ToString()));
            LeaderboardStats.Add(new RoundEndStats("<color=red>SCP Kills</color>", Round.KillsByScp.ToString()));
            LeaderboardStats.Add(new RoundEndStats("Kills", Round.Kills.ToString()));
            if (Round.Kills > 0)
                LeaderboardStats.Add(new RoundEndStats("Percent Kills:SCPKills", (Round.KillsByScp / Round.Kills).ToString() + "%"));
            if (Round.EscapedDClasses > 0)
                LeaderboardStats.Add(new RoundEndStats("<color=orange>DClass Escapes</color>", Round.EscapedDClasses.ToString()));
            if (Round.EscapedScientists > 0)
                LeaderboardStats.Add(new RoundEndStats("<color=yellow>Scientist Escapes</color>", Round.EscapedScientists.ToString()));
            foreach (LeaderboardHelper.TeamLeaderboard tl in TeamPlugin.Singleton.TeamEventHandler.Leaderboard.TeamLeaderboards)
            {
                if ((!tl.Team.VanillaTeam || tl.Team.Name == "MTF" || tl.Team.Name == "CHI") && tl.Stats.Count == 0)
                {
                    LeaderboardStats.Add(new RoundEndStats($"<color={tl.Team.Color}>{tl.Team.Name}</color> Did not spawn this round...", String.Empty));
                }
                foreach (RoundEndStats res in tl.Stats)
                {
                    LeaderboardStats.Add(res);
                }
            }
            LeaderboardStats.Add(new RoundEndStats("Rounds Completed", Round.UptimeRounds.ToString()));
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
