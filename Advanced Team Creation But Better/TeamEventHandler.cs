﻿using System;
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
using ATCBB.TeamAPI.CustomEventHelpers;

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
            
            if (ev.Reason != Exiled.API.Enums.SpawnReason.Respawn && ev.Reason != Exiled.API.Enums.SpawnReason.Escaped)
            {
                Leaderboard.ClearPlayerFromLeaderBoards(ev.Player);
                ev.Player.ChangeAdvancedTeam(plugin.Config.FindAT(ev.NewRole.GetTeam().ToString()));
                ev.Player.CustomInfo = null;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
                if (ev.Reason != Exiled.API.Enums.SpawnReason.RoundStart)
                {
                    MEC.Timing.CallDelayed(0.1f, () =>
                    CustomRoundEnder.UpdateRoundStatus());
                }
            }
        }

        public void RagdollSpawn(SpawningRagdollEventArgs ev)
        {
            if (ev.Owner.GetAdvancedTeam().VanillaTeam) return;
            ev.IsAllowed = false;
            RagdollInfo info = new RagdollInfo(Server.Host.ReferenceHub, ev.DamageHandlerBase, ev.Role, ev.Position, ev.Rotation, $"{ev.Nickname}", ev.CreationTime);
            new Exiled.API.Features.Ragdoll(info, true);
        }

        public void PlayerDead(DyingEventArgs ev)
        {
            if (ev.Killer.GetAdvancedTeam().ConfirmFriendshipWithTeam(ev.Target.GetAdvancedTeam()) && ev.Killer.IsScp)
            {
                ev.Killer.ShowHint("<color=red>Don't damage a friendly team</color>");
                if (!plugin.Config.FriendlyFire)
                {
                    ev.IsAllowed = false;
                }
                else
                {
                    ev.IsAllowed = false;
                }
            }
        }

        public void PlayerHurt(HurtingEventArgs ev)
        {
            if (ev.Attacker.GetAdvancedTeam().ConfirmFriendshipWithTeam(ev.Target.GetAdvancedTeam()))
            {
                ev.Attacker.ShowHint("<color=red>Don't damage a friendly team</color>");
                if (!plugin.Config.FriendlyFire)
                {
                    ev.IsAllowed = false;
                }
                else
                {
                    ev.Target.Hurt(ev.Amount * 0.3f);
                    ev.IsAllowed = false;
                }
            }
        }

        public void MapGenerated()
        {
            plugin.Config.LoadTeamConfigs();
            Leaderboard.SetUpTeamLeaders();
            Respawns = 0;
            LastTeamSpawned = null;
            CassieHelper = null;
            ReferancedTeam = null;
        }

        public void RoundEnding(EndingRoundEventArgs ev)
        {
            ev.IsRoundEnded = !plugin.Config.CustomRoundEnder;
            ev.IsAllowed = !plugin.Config.CustomRoundEnder;
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
            else
            {
                Leaderboard.ClearPlayerFromLeaderBoards(ev.Player);
                ev.Player.ChangeAdvancedTeam(plugin.Config.FindAT(ev.NewRole.GetTeam().ToString()));
                ev.Player.InfoArea = PlayerInfoArea.CustomInfo;
                ev.Player.CustomInfo = string.Empty;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
                MEC.Timing.CallDelayed(0.1f, () =>
                    CustomRoundEnder.UpdateRoundStatus());
            }
        }

        public int HiddenInterference;

        public void MtfRespawnCassie(AnnouncingNtfEntranceEventArgs ev)
        {
            if (CassieHelper != null && !CassieHelper.VanillaTeam)
            {
                if (CassieHelper.ChanceForHiddenMtfNato < HiddenInterference)
                {
                    ChangeUnitNameOnAdvancedTeam(Respawns, CassieHelper, $"{ev.UnitName}-{ev.UnitNumber}");
                }
                else
                {
                    ev.IsAllowed = false;
                    ChangeUnitNameOnAdvancedTeam(Respawns, "<color=black>INTERFERENCE</color>");
                    Respawns++;
                    CassieHelper = null;
                    return;
                }
            }
            if (CassieHelper != null && CassieHelper.CassieAnnouncement != AdvancedTeam.DEFAULTAnnounce)
            {
                ev.IsAllowed = false;
                if (CassieHelper.CassieAnnouncement != String.Empty)
                {
                    if (!CassieHelper.PlayBeforeSpawning)
                        Cassie.MessageTranslated(CassieHelper.CassieAnnouncement.Replace("{SCPLeft}", ev.ScpsLeft.ToString()).Replace("{Unit}", $"NATO_{ev.UnitName[0].ToString().ToLower()}").Replace("{UnitNum}", Cassie.ConvertNumber(ev.UnitNumber)), CassieHelper.CassieAnnouncementSubtitles.Replace("{SCPLeft}", ev.ScpsLeft.ToString()).Replace("{Unit}", ev.UnitName).Replace("{UnitNum}", ev.UnitNumber.ToString()));
                }
            }
            Respawns++;
            CassieHelper = null;
        }
        bool PlayedAlready = false;
        int ScpsLeft => Player.List.Where(e => e.IsScp && e.Role.Type != RoleType.Scp0492).Count();

        public void ChangeUnitNameOnAdvancedTeam(int index, AdvancedTeam Name, string UnitName)
        {
            RespawnManager.Singleton.NamingManager.AllUnitNames.Remove(RespawnManager.Singleton.NamingManager.AllUnitNames[index]);
            UnitNamingRules.AllNamingRules[SpawnableTeamType.NineTailedFox].AddCombination($"<color={Name.Color}>{Name.Name}-{UnitName}</color>", SpawnableTeamType.NineTailedFox);
        }

        public void ChangeUnitNameOnAdvancedTeam(int index, string UnitName)
        {
            RespawnManager.Singleton.NamingManager.AllUnitNames.Remove(RespawnManager.Singleton.NamingManager.AllUnitNames[index]);
            UnitNamingRules.AllNamingRules[SpawnableTeamType.NineTailedFox].AddCombination(UnitName, SpawnableTeamType.NineTailedFox);
        }

        public void AddUnitNameOnAdvancedTeam(AdvancedTeam Name, string UnitName)
        {
            UnitNamingRules.AllNamingRules[SpawnableTeamType.NineTailedFox].AddCombination($"<color={Name.Color}>{Name.Name}-{UnitName}</color>", SpawnableTeamType.NineTailedFox);
        }

        int Respawns = 0;

        string[] Letters = new string[26]
        { 
            "Alpha",
            "Bravo",
            "Charlie",
            "Delta",
            "Echo",
            "Foxtrot",
            "Golf",
            "Hotel",
            "India",
            "Juliett",
            "Kilo",
            "Lima",
            "Mike",
            "November",
            "Oscar",
            "Papa",
            "Quebec",
            "Romeo",
            "Sierra",
            "Tango",
            "Uniform",
            "Victor",
            "Whiskey",
            "X-ray",
            "Yankee",
            "Zulu"
        };

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
            int UnitNum = new Random().Next(1, 9);
            string UnitName = Letters[new Random().Next(0, 26)];
            if (!ReferancedTeam.PlayBeforeSpawning && !ReferancedTeam.VanillaTeam && !PlayedAlready && ev.NextKnownTeam != Respawning.SpawnableTeamType.NineTailedFox)
                if (HiddenInterference < ReferancedTeam.ChanceForHiddenMtfNato)
                    Cassie.MessageTranslated(ReferancedTeam.CassieAnnouncement.Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Unit}", $"NATO_{UnitName[0]}").Replace("{UnitNum}", UnitNum.ToString()), ReferancedTeam.CassieAnnouncementSubtitles.Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Unit}", $"{UnitName}").Replace("{UnitNum}", UnitNum.ToString()));

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
            HiddenInterference = new Random().Next(0, 99);
            int UnitNumPre = new Random().Next(1, 9);
            string UnitNamePre = Letters[new Random().Next(0, 26)];
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
                if (HiddenInterference < ev.AdvancedTeam.ChanceForHiddenMtfNato)
                    Cassie.MessageTranslated(ReferancedTeam.CassieAnnouncement.Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Unit}", $"NATO_{UnitNamePre[0]}").Replace("{UnitNum}", UnitNumPre.ToString()), ReferancedTeam.CassieAnnouncementSubtitles.Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Unit}", $"{UnitNamePre}").Replace("{UnitNum}", UnitNumPre.ToString()));
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