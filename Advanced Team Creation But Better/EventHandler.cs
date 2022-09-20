using AdvancedTeamCreation.TeamAPI;
using AdvancedTeamCreation.TeamAPI.CustomEvents;
using AdvancedTeamCreation.TeamAPI.Events;
using AdvancedTeamCreation.TeamAPI.Extentions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Events.Patches.Generic;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedTeamCreation.TeamAPI.Helpers;

namespace AdvancedTeamCreation
{
    public class EventHandler
    {
        public CoroutineHandle coro;
        public Dictionary<Player, TimeSpan> PlayerTimesAlive = new Dictionary<Player, TimeSpan>();

        public EventHandler(Plugin<TeamConfig, Translations> Plugin)
        {
            plugin = Plugin;
        }

        public void EscapingEvent(EscapingEventArgs ev)
        {
            if (RespawnHelper.LastTeamSpawned.EscapableClasses.Contains(ev.Player.Role))
            {
                ev.IsAllowed = false;
                ev.Player.ChangeAdvancedRole(RespawnHelper.LastTeamSpawned, UnitHelper.FindAST(RespawnHelper.LastTeamSpawned.Name, RespawnHelper.LastTeamSpawned.EscapeClass), Extentions.InventoryDestroyType.Drop, true);
                MEC.Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.ShowPlayerTeamDisplay();
                    CustomRoundEnder.UpdateRoundStatus();
                }
                );
            }
            else
            {
                RespawnHelper.Leaderboard.ClearPlayerFromLeaderBoards(ev.Player);
                ev.Player.InfoArea = PlayerInfoArea.CustomInfo;
                ev.Player.CustomInfo = string.Empty;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
                MEC.Timing.CallDelayed(0.1f, () =>
                    {
                        ev.Player.ChangeAdvancedTeam(UnitHelper.FindAT(ev.NewRole.GetTeam().ToString()));
                        ev.Player.ShowPlayerTeamDisplay();
                        CustomRoundEnder.UpdateRoundStatus();
                    }
                    );
            }
        }

        public void MapGenerated()
        {
            if (coro != null)
                Timing.KillCoroutines(coro);
            PlayerTimesAlive.Clear();
            coro = Timing.RunCoroutine(TeamLister());
            plugin.Config.LoadTeamConfigs();
            RespawnHelper.Leaderboard.SetUpTeamLeaders();
            MessageHelper.ResetSpawns();
            RespawnHelper.SetReferance(null);
            RespawnHelper.ResetCassie();
            MEC.Timing.CallDelayed(1, () =>
            {
                if (TeamPlugin.assemblyTimer != null)
                {
                    HudHelper.NormalTime = RespawnTimer.API.API.TimerView;
                    Log.Debug($"Caught Normal Timer {HudHelper.NormalTime.DuringRespawnString.Length}", plugin.Config.Debug);
                    HudHelper.LoadTimerConfig(null);
                }
            });
        }

        public void MtfRespawnCassie(AnnouncingNtfEntranceEventArgs ev)
        {
            MessageHelper.IncrementSpawns();
            if (RespawnHelper.CassieHelper != null && !RespawnHelper.CassieHelper.VanillaTeam)
            {
                Log.Debug("Cassie Helper is stable", plugin.Config.Debug);
                if (RespawnHelper.CassieHelper.ChanceForHiddenMtfNato < MessageHelper.HiddenInterference)
                {
                    UnitHelper.ChangeUnitNameOnAdvancedTeam(MessageHelper.Spawns, RespawnHelper.CassieHelper, $"{ev.UnitName}-{ev.UnitNumber}");
                }
                else
                {
                    ev.IsAllowed = false;
                    UnitHelper.ChangeUnitNameOnAdvancedTeam(MessageHelper.Spawns, TeamPlugin.Singleton.Translation.InterferenceText);
                    RespawnHelper.ResetCassie();
                    return;
                }
            }
            if (RespawnHelper.CassieHelper != null && RespawnHelper.CassieHelper.CassieAnnouncement != AdvancedTeam.DEFAULTAnnounce)
            {
                ev.IsAllowed = false;
                Log.Debug("Playing Team Announcement from MtfCassie!", plugin.Config.Debug);
                MessageHelper.PlayTeamAnnouncement(RespawnHelper.CassieHelper, ev.UnitName, ev.UnitNumber);
                MessageHelper.ResetTeamAnnouncement();
            }
            if (plugin.Config.TeamsListPromptsAtAnnouncement)
            {
                HudHelper.ShowAllPlayersTeamDisplay(10);
            }
            RespawnHelper.ResetCassie();
        }

        public void PlayerDead(DyingEventArgs ev)
        {
            if (ev.Killer == null)
            {
                Log.Debug($"There is no Killer so using Target instead", plugin.Config.Debug);
                Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} how many players are left {RespawnHelper.Leaderboard.GetTeamLeaderboard(ev.Target.GetAdvancedTeam().Name).PlayerPairs.Count - 1}", plugin.Config.Debug);
                if (RespawnHelper.Leaderboard.GetTeamLeaderboard(ev.Target.GetAdvancedTeam().Name).PlayerPairs.Count - 1 == 0)
                {
                    var e2 = new TeamEvents.AdvancedTeamSlaughteredEventArgs(ev.Target.GetAdvancedTeam(), ev.Target.GetAdvancedTeam());
                    e2.Slaughter();
                    Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} sent slaughter event", plugin.Config.Debug);
                    if (e2.IsAllowed && ev.Target.GetAdvancedTeam().Name != "SCP")
                    {
                        if (e2.AdvancedTeam.CassieSlaughtered != string.Empty)
                        {
                            Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} sent Termination Cassie", plugin.Config.Debug);
                            MessageHelper.SlaughteredTeam(e2.AdvancedTeam, e2.TerminatingTeam);
                        }
                    }
                }
                return;
            }
            if (!ev.Killer.IsConnected && !ev.Target.IsConnected) return;
            Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} how many players are left {RespawnHelper.Leaderboard.GetTeamLeaderboard(ev.Target.GetAdvancedTeam().Name).PlayerPairs.Count - 1}", plugin.Config.Debug);
            if (RespawnHelper.Leaderboard.GetTeamLeaderboard(ev.Target.GetAdvancedTeam().Name).PlayerPairs.Count - 1 == 0)
            {
                var e2 = new TeamEvents.AdvancedTeamSlaughteredEventArgs(ev.Target.GetAdvancedTeam(), ev.Killer.GetAdvancedTeam());
                e2.Slaughter();
                Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} sent slaughter event", plugin.Config.Debug);
                if (e2.IsAllowed && ev.Target.GetAdvancedTeam().Name != "SCP")
                {
                    if (e2.AdvancedTeam.CassieSlaughtered != string.Empty)
                    {
                        Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} sent Termination Cassie", plugin.Config.Debug);
                        var Lead = RespawnHelper.Leaderboard.GetTeamLeaderboard(ev.Killer.GetAdvancedTeam().Name);
                        Lead.UpdateStat(TeamPlugin.Singleton.Translation.TeamKillsStat.Replace("{Team}", ev.Killer.GetAdvancedTeam().Name), (int.Parse(Lead.GetStat(TeamPlugin.Singleton.Translation.TeamKillsStat.Replace("{Team}", ev.Killer.GetAdvancedTeam().Name)).Value) + 1).ToString());
                        MessageHelper.SlaughteredTeam(e2.AdvancedTeam, e2.TerminatingTeam);
                    }
                }
            }
            var e1 = new TeamEvents.AdvancedTeamDeadPlayerEventArgs(ev.Target.GetAdvancedTeam(), ev.Target, ev.Killer);
            e1.Kill();
            if (!e1.IsAllowed)
            {
                ev.IsAllowed = false;
                return;
            }
            nickHelper = $"{ev.Target.Nickname}({ev.Target.GetAdvancedTeam().DisplayName})";
            if (ev.Target.IsScp)
            {
                NineTailedFoxAnnouncer.ConvertSCP(ev.Target.Role.Type, out string nSpace, out string wSpace);
                MessageHelper.CustomTeamScpTermination(wSpace, ev.Killer.GetAdvancedTeam());
            }
        }

        public void PlayerHurt(HurtingEventArgs ev)
        {
            if (ev.Attacker == null) return;
            if (ev.Attacker.IsScp) return;
            if (!ev.Attacker.IsConnected && !ev.Target.IsConnected) return;
            if (ev.Attacker.GetAdvancedTeam().ConfirmFriendshipWithTeam(ev.Target.GetAdvancedTeam()))
            {
                ev.Attacker.ShowHint("<color=red>Don't damage a friendly team</color>");
            }
            //if TeamDamageHandler then return to prevent looping
            if (ev.Handler.As<TeamDamageHandler>() != null)
                if (ev.Handler.As<TeamDamageHandler>()._deathReason == "Enemy") return;
            if (!IndividualFriendlyFire.CheckFriendlyFirePlayerHitbox(ev.Attacker.ReferenceHub, ev.Target.ReferenceHub))
            {
                if (ev.Target.GetAdvancedTeam().ConfirmEnemyshipWithTeam(ev.Attacker.GetAdvancedTeam()))
                {
                    ev.Attacker.ShowHitMarker();
                    if (ev.Amount >= ev.Target.Health)
                    {
                        ev.Target.Kill($"{ev.Attacker}({ev.Attacker.GetAdvancedTeam().DisplayName}) killed you with {ev.Handler.Type}", ev.Attacker);
                    }
                    else
                    {
                        ev.Target.Hurt(ev.Handler.Damage, ev.Attacker);
                    }
                    //Log.Debug($"Allowing {ev.Attacker.Nickname} to damage {ev.Target.Nickname} because of their opposing teams", TeamPlugin.Singleton.Config.Debug);
                }
            }
            else
            {
                if (ev.Target.GetAdvancedTeam().ConfirmFriendshipWithTeam(ev.Attacker.GetAdvancedTeam()))
                {
                    if (ev.Target.GetAdvancedTeam().VanillaTeam)
                    {
                        if (Server.FriendlyFire)
                        {
                            if (ev.Attacker.FriendlyFireMultiplier.ContainsKey(ev.Target.Role.Type))
                                ev.Handler.Damage *= ev.Attacker.FriendlyFireMultiplier[ev.Target.Role.Type];
                            else
                                ev.Handler.Damage *= 0.3f;
                            ev.Target.Hurt(ev.Handler.Damage, ev.Attacker);

                            //Log.Debug($"Allowing {ev.Attacker.Nickname} to damage {ev.Target.Nickname} because of their friendly teams (vanilla)", TeamPlugin.Singleton.Config.Debug);
                        }
                        ev.IsAllowed = false;
                    }
                    else
                    {
                        if (Server.FriendlyFire)
                        {
                            ev.Handler.Damage *= 0.3f;
                            ev.Target.Hurt(ev.Handler.Damage, ev.Attacker);

                            //Log.Debug($"Allowing {ev.Attacker.Nickname} to damage {ev.Target.Nickname} because of their friendly teams", TeamPlugin.Singleton.Config.Debug);
                        }
                        ev.IsAllowed = false;
                    }
                }
            }
        }

        public void RagdollSpawn(SpawningRagdollEventArgs ev)
        {
            if (ev.Owner.GetAdvancedTeam().VanillaTeam) return;
            ev.IsAllowed = false;
            var LatestestRagdollInfo = new RagdollInfo(Server.Host.ReferenceHub, ev.DamageHandlerBase, ev.Role, ev.Position, ev.Rotation, nickHelper, ev.CreationTime);
            new Exiled.API.Features.Ragdoll(LatestestRagdollInfo, true);
        }

        public void RoleChange(ChangingRoleEventArgs ev)
        {
            PlayerTimesAlive[ev.Player] = Round.ElapsedTime;
            if (ev.Reason != Exiled.API.Enums.SpawnReason.Respawn && ev.Reason != Exiled.API.Enums.SpawnReason.Escaped)
            {
                RespawnHelper.Leaderboard.ClearPlayerFromLeaderBoards(ev.Player);
                ev.Player.ChangeAdvancedTeam(UnitHelper.FindAT(ev.NewRole.GetTeam().ToString()));
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

        public void RoundEnd(RoundEndedEventArgs ev)
        {
            RespawnHelper.Leaderboard.DestroyTeamLeaders();
        }

        // Preventing conflicts with plugins like EndConditions
        public void RoundEnding(EndingRoundEventArgs ev)
        {
            if (!plugin.Config.CustomRoundEnder) return;
            ev.IsRoundEnded = false;
            ev.IsAllowed = false;
        }

        public void Scp106DistressHelper(ContainingEventArgs ev)
        {
            if (ev.Player.GetAdvancedTeam().ConfirmFriendshipWithTeam(UnitHelper.FindAT("SCP")))
            {
                if (!Server.FriendlyFire) return;
                ev.IsAllowed = false;
            }
        }

        public void Scp106FemurBreakerPreventer(EnteringFemurBreakerEventArgs ev)
        {
            if (ev.Player.GetAdvancedTeam().SpawnRoom == RoomType.Hcz106 || ev.Player.GetAdvancedTeam().ConfirmFriendshipWithTeam(UnitHelper.FindAT("SCP")))
            {
                ev.IsAllowed = false;
            }
        }

        public void ScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (ev.Killer == null) return;
            if (ev.Handler.TargetFootprint.Role.GetTeam() != Team.SCP) return;
            if (plugin.Config.TeamsListPromptsAtAnnouncement)
            {
                HudHelper.ShowAllPlayersTeamDisplay(10);
            }
            if (!ev.Killer.GetAdvancedTeam().VanillaTeam)
            {
                ev.IsAllowed = false;
            }
        }

        public void TeamSpawning(RespawningTeamEventArgs ev)
        {
            if (RespawnHelper.ReferancedTeam == null)
            {
                Log.Warn("Possible Force Team Spawn...");
                return;
            }
            if (RespawnHelper.ReferancedTeam.VanillaTeam || RespawnHelper.ReferancedTeam == null)
            {
                HudHelper.LoadTimerConfig(null);
                foreach (Player p in ev.Players)
                {
                    switch (ev.NextKnownTeam)
                    {
                        case Respawning.SpawnableTeamType.ChaosInsurgency:
                            p.ChangeAdvancedTeam(UnitHelper.FindAT("CHI"));

                            break;

                        case Respawning.SpawnableTeamType.NineTailedFox:
                            p.ChangeAdvancedTeam(UnitHelper.FindAT("MTF"));
                            break;
                    }
                }
                RespawnHelper.SetReferance(null);
                return;
            }
            if (!RespawnHelper.ReferancedTeam.PlayBeforeSpawning && !RespawnHelper.ReferancedTeam.VanillaTeam && ev.NextKnownTeam != Respawning.SpawnableTeamType.NineTailedFox)
            {
                Log.Debug("Playing Team Announcement from Team Spawn!", plugin.Config.Debug);
                MessageHelper.PlayTeamAnnouncement(RespawnHelper.ReferancedTeam);
            }

            Dictionary<string, int> Helper = new Dictionary<string, int>();
            foreach (string t in RespawnHelper.ReferancedTeam.SpawnOrder)
            {
                Helper[t.Split(':')[0]] = int.Parse(t.Split(':')[1]);
            }
            foreach (Player p in ev.Players)
            {
                if (Helper.Values.Count == 0)
                {
                    p.SetRole(RoleType.Spectator, SpawnReason.Died);
                    p.Broadcast(new Exiled.API.Features.Broadcast("You couldn't be respawned because this team has limited players", 5), true);
                    break;
                }
                else if (Helper.First().Value < 1)
                {
                    Helper.Remove(Helper.First().Key);
                }
                else
                {
                    Helper[Helper.First().Key]--;
                }
                p.ChangeAdvancedRole(RespawnHelper.ReferancedTeam, UnitHelper.FindAST(RespawnHelper.ReferancedTeam.Name, Helper.First().Key), Extentions.InventoryDestroyType.Destroy, true);
            }
            RespawnHelper.SetReferance(null);
            if (TeamPlugin.assemblyTimer != null)
            {
                HudHelper.LoadTimerConfig(null);
            }
            MessageHelper.ResetTeamAnnouncement();
        }

        private string nickHelper;
        private Plugin<TeamConfig> plugin;

        private IEnumerator<float> TeamLister()
        {
            while (true)
            {
                yield return Timing.WaitUntilTrue(() => Round.IsStarted);
                yield return Timing.WaitForSeconds(0.25f);
                foreach (Player ply in Player.List)
                {
                    if (!ply.GetAdvancedTeam().Spectator)
                    {
                        if (!plugin.Config.TeamsListPromptsAtAnnouncement)
                        {
                            if (PlayerTimesAlive[ply].TotalSeconds + 3 < Round.ElapsedTime.TotalSeconds)
                            {
                                if (plugin.Config.ShowEnemyTeamsForTime == -1)
                                    ply.ShowPlayerTeamDisplay();
                                else
                                    ply.ShowPlayerTeamDisplay(1, PlayerTimesAlive[ply].TotalSeconds + plugin.Config.ShowEnemyTeamsForTime < Round.ElapsedTime.TotalSeconds);
                            }
                        }
                        if (plugin.Config.ShowFriendlyHint)
                            HudHelper.RaycastHelper(ply);
                    }
                }
            }
        }
    }
}