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
using Respawning;
using Respawning.NamingRules;
using System.Text.RegularExpressions;

namespace ATCBB
{
    public class TeamEventHandler
    {
        private Plugin<TeamConfig> plugin;
        public LeaderboardHelper Leaderboard = new LeaderboardHelper();
        public AdvancedTeam ReferancedTeam, CassieHelper, LastTeamSpawned;

        public TeamEventHandler(Plugin<TeamConfig> Plugin)
        {
            plugin = Plugin;
        }

        public void RoleChange(ChangingRoleEventArgs ev)
        {
            
            if (ev.Reason != Exiled.API.Enums.SpawnReason.Respawn)
            {
                Leaderboard.ClearPlayerFromLeaderBoards(ev.Player);
                ev.Player.ChangeAdvancedTeam(plugin.Config.FindAT(ev.NewRole.GetTeam().ToString()));
                ev.Player.CustomInfo = string.Empty;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
            }
        }

        public void MapGenerated()
        {
            Leaderboard.SetUpTeamLeaders();
            Respawns = 0;
        }

        public void RoundEnd(RoundEndedEventArgs ev)
        {
            Leaderboard.DestroyTeamLeaders();
        }

        public void EscapingEvent(EscapingEventArgs ev)
        {
            if (LastTeamSpawned.EscapableClasses.Contains(ev.Player.Role))
            {
                ev.IsAllowed = false;
                ev.Player.ChangeAdvancedRole(LastTeamSpawned, plugin.Config.FindAST(LastTeamSpawned.Name, LastTeamSpawned.EscapeClass), Extentions.InventoryDestroyType.Drop, true);
            }
        }

        public void MtfRespawnCassie(AnnouncingNtfEntranceEventArgs ev)
        {
            if (CassieHelper != null && !CassieHelper.VanillaTeam)
                ChangeUnitNameOnAdvancedTeam(Respawns, CassieHelper, $"{ev.UnitName}-{ev.UnitNumber}");
            if (CassieHelper != null && CassieHelper.CassieAnnouncement != AdvancedTeam.DEFAULTAnnounce)
            {
                ev.IsAllowed = false;
                if (CassieHelper.CassieAnnouncement != String.Empty)
                {
                    if (!CassieHelper.PlayBeforeSpawning)
                        Cassie.MessageTranslated(CassieHelper.CassieAnnouncement.Replace("{SCPLeft}", ev.ScpsLeft.ToString().Replace("{Unit}", $"NATO_{ev.UnitName[0].ToString().ToLower()}").Replace("{UnitNum}", ev.UnitNumber.ToString())), CassieHelper.CassieAnnouncementSubtitles.Replace("{SCPLeft}", ev.ScpsLeft.ToString()).Replace("{Unit}", ev.UnitName).Replace("{UnitNum}", ev.UnitNumber.ToString()));
                }
            }
            Respawns++;
            CassieHelper = null;
        }
        bool PlayedAlready = false;
        int ScpsLeft => Player.List.Where(e => e.IsScp && e.Role.Type != RoleType.Scp0492).Count();

        public void ChangeUnitNameOnAdvancedTeam(int index, AdvancedTeam Name, string UnitName)
        {
            string unit = RespawnManager.Singleton.NamingManager.AllUnitNames[index].UnitName;
            RespawnManager.Singleton.NamingManager.AllUnitNames.Remove(RespawnManager.Singleton.NamingManager.AllUnitNames[index]);
            UnitNamingRules.AllNamingRules[SpawnableTeamType.NineTailedFox].AddCombination($"<color={Name.Color}>{Name.Name}-{UnitName}</color>", SpawnableTeamType.NineTailedFox);
        }

        int Respawns = 0;

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
                Cassie.MessageTranslated(ReferancedTeam.CassieAnnouncement.Replace("{SCPLeft}", ScpsLeft.ToString()), ReferancedTeam.CassieAnnouncementSubtitles.Replace("{SCPLeft}", ScpsLeft.ToString()));
            
            Dictionary<string, int> Helper = new Dictionary<string, int>();
            foreach (string t in ReferancedTeam.SpawnOrder)
            {
                Helper[t.Split(':')[0]] = int.Parse(t.Split(':')[1]);
            }
            foreach (Player p in ev.Players)
            {
                p.ChangeAdvancedRole(ReferancedTeam, TeamPlugin.Singleton.Config.FindAST(ReferancedTeam.Name, Helper.First().Key), Extentions.InventoryDestroyType.Destroy, true);
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
            LastTeamSpawned = ReferancedTeam;
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
                Cassie.MessageTranslated(ReferancedTeam.CassieAnnouncement.Replace("{SCPLeft}", ScpsLeft.ToString()), ReferancedTeam.CassieAnnouncementSubtitles.Replace("{SCPLeft}", ScpsLeft.ToString()));
                PlayedAlready = true;
            }
            CassieHelper = ReferancedTeam;

            if (TeamPlugin.assemblyTimer != null && !ev.AdvancedTeam.VanillaTeam && ev.SupposedTeam != Respawning.SpawnableTeamType.None)
            {
                switch (ev.SupposedTeam)
                {
                    case Respawning.SpawnableTeamType.ChaosInsurgency:
                        RespawnTimer.RespawnTimer.Singleton.Translation.Ci = $"<color={ev.AdvancedTeam.Color}>{ev.AdvancedTeam.Name}</color>";
                        break;
                    case Respawning.SpawnableTeamType.NineTailedFox:
                        RespawnTimer.RespawnTimer.Singleton.Translation.Ntf = $"<color={ev.AdvancedTeam.Color}>{ev.AdvancedTeam.Name}</color>";
                        break;
                }
            }
        }
    }
}