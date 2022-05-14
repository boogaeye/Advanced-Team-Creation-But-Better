using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using Exiled.API;
using Exiled.Events.EventArgs;
using Exiled.API.Features;
using Exiled.API.Extensions;
using ATCBB.TeamAPI.Events;
using ATCBB.TeamAPI;
using ATCBB.TeamAPI.Extentions;

namespace ATCBB
{
    public class TeamEventHandler
    {
        private Plugin<TeamConfig> plugin;
        public LeaderboardHelper Leaderboard = new LeaderboardHelper();
        public AdvancedTeam ReferancedTeam, CassieHelper;

        public TeamEventHandler(Plugin<TeamConfig> Plugin)
        {
            plugin = Plugin;
        }

        public void RoleChange(ChangingRoleEventArgs ev)
        {
            
            if (ev.Reason != Exiled.API.Enums.SpawnReason.Respawn)
            {
                ev.Player.InfoArea |= PlayerInfoArea.Role;
                ev.Player.CustomInfo = String.Empty;
                Leaderboard.ClearPlayerFromLeaderBoards(ev.Player);
                ev.Player.ChangeAdvancedTeam(plugin.Config.FindAT(ev.NewRole.GetTeam().ToString()));
            }
        }

        public void MapGenerated()
        {
            Leaderboard.SetUpTeamLeaders();
        }

        public void RoundEnd(RoundEndedEventArgs ev)
        {
            Leaderboard.DestroyTeamLeaders();
        }

        public void MtfRespawnCassie(AnnouncingNtfEntranceEventArgs ev)
        {
            if (CassieHelper != null && CassieHelper.CassieAnnouncement != AdvancedTeam.DEFAULTAnnounce)
            {
                ev.IsAllowed = false;
                if (CassieHelper.CassieAnnouncement != String.Empty)
                {
                    if (!CassieHelper.PlayBeforeSpawning)
                        Cassie.MessageTranslated(CassieHelper.CassieAnnouncement.Replace("{SCPLeft}", ev.ScpsLeft.ToString()), CassieHelper.CassieAnnouncementSubtitles.Replace("{SCPLeft}", ev.ScpsLeft.ToString()));
                }
            }
            CassieHelper = null;
        }
        bool PlayedAlready = false;
        public void TeamSpawning(RespawningTeamEventArgs ev)
        {
            if (ReferancedTeam.VanillaTeam || ReferancedTeam == null)
            {
                RespawnTimer.RespawnTimer.Singleton.Translation.Ci = TeamPlugin.chaosTrans;
                RespawnTimer.RespawnTimer.Singleton.Translation.Ntf = TeamPlugin.mtfTrans;
                foreach (Player p in ev.Players)
                {
                    switch (ev.NextKnownTeam)
                    {
                        case Respawning.SpawnableTeamType.ChaosInsurgency:
                            p.ChangeAdvancedTeam(plugin.Config.FindAT("CHI"));
                            
                            break;
                        case Respawning.SpawnableTeamType.NineTailedFox:
                            p.ChangeAdvancedTeam(plugin.Config.FindAT("MTF"));
                            break;
                    }
                    
                }
                ReferancedTeam = null;
                return;
            }
            if (!ReferancedTeam.PlayBeforeSpawning && !ReferancedTeam.VanillaTeam && !PlayedAlready && ev.NextKnownTeam != Respawning.SpawnableTeamType.NineTailedFox)
                Cassie.MessageTranslated(ReferancedTeam.CassieAnnouncement, ReferancedTeam.CassieAnnouncementSubtitles);
            Dictionary<string, int> Helper = new Dictionary<string, int>();
            foreach (string t in ReferancedTeam.SpawnOrder)
            {
                Helper[t.Split(':')[0]] = int.Parse(t.Split(':')[1]);
            }
            foreach (Player p in ev.Players)
            {
                p.ChangeAdvancedRole(ReferancedTeam, TeamPlugin.Singleton.Config.FindAST(ReferancedTeam.Name, Helper.First().Key), true, true);
                if (Helper.Values.Count == 0)
                {
                    p.SetRole(RoleType.Spectator, Exiled.API.Enums.SpawnReason.None);
                }
                else if (Helper.First().Value - 1 < 1) 
                {
                    
                    Helper.Remove(Helper.First().Key);
                }
                else
                {
                    Helper[Helper.First().Key]--;
                }
            }
            ReferancedTeam = null;
            if (TeamPlugin.assemblyTimer != null)
            {
                RespawnTimer.RespawnTimer.Singleton.Translation.Ci = TeamPlugin.chaosTrans;
                RespawnTimer.RespawnTimer.Singleton.Translation.Ntf = TeamPlugin.mtfTrans;
            }
            PlayedAlready = false;
        }

        public void ReferancingTeam(TeamEvents.ReferancingTeamEventArgs ev)
        {
            Log.Debug($"Got Referance {ev.AdvancedTeam.Name}", plugin.Config.Debug);
            if (!ev.IsAllowed)
            {
                return;
            }
            if (ev.SupposedTeam == Respawning.SpawnableTeamType.None)
            {
                return;
            }
            ReferancedTeam = ev.AdvancedTeam;
            if (ReferancedTeam.PlayBeforeSpawning && !ReferancedTeam.VanillaTeam && !PlayedAlready)
            {
                Cassie.MessageTranslated(ReferancedTeam.CassieAnnouncement, ReferancedTeam.CassieAnnouncementSubtitles);
                PlayedAlready = true;
            }
            CassieHelper = ReferancedTeam;

            if (TeamPlugin.assemblyTimer != null && !ev.AdvancedTeam.VanillaTeam && ev.SupposedTeam != Respawning.SpawnableTeamType.None)
            {
                switch (ev.SupposedTeam)
                {
                    case Respawning.SpawnableTeamType.ChaosInsurgency:
                        RespawnTimer.RespawnTimer.Singleton.Translation.Ci = ev.AdvancedTeam.Name;
                        break;
                    case Respawning.SpawnableTeamType.NineTailedFox:
                        RespawnTimer.RespawnTimer.Singleton.Translation.Ntf = ev.AdvancedTeam.Name;
                        break;
                }
            }
        }
    }
}