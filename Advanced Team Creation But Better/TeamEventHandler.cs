using ATCBB.TeamAPI;
using ATCBB.TeamAPI.CustomEventHelpers;
using ATCBB.TeamAPI.Events;
using ATCBB.TeamAPI.Extentions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Respawning;
using Respawning.NamingRules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ATCBB
{
    public class TeamEventHandler
    {
        #region Public Fields

        public CoroutineHandle coro;
        public int HiddenInterference;
        public LeaderboardHelper Leaderboard = new LeaderboardHelper();
        public Dictionary<Player, TimeSpan> PlayerTimesAlive = new Dictionary<Player, TimeSpan>();
        public AdvancedTeam ReferancedTeam, CassieHelper, LastTeamSpawned;
        public List<Player> TeamKillList = new List<Player>();

        #endregion Public Fields

        #region Public Constructors

        public TeamEventHandler(Plugin<TeamConfig> Plugin)
        {
            plugin = Plugin;
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddUnitNameOnAdvancedTeam(AdvancedTeam Name, string UnitName)
        {
            UnitNamingRules.AllNamingRules[SpawnableTeamType.NineTailedFox].AddCombination($"<color={Name.Color}>{Name.Name}-{UnitName}</color>", SpawnableTeamType.NineTailedFox);
        }

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

        public void CustomTeamScpTermination(string scpName, Exiled.API.Features.DamageHandlers.DamageHandlerBase info, AdvancedTeam at)
        {
            if (at.VanillaTeam) return;
            string text = scpName;
            text = "scp " + text + " CONTAINEDSUCCESSFULLY. containment unit " + at.SaidName;
            float num = ((AlphaWarheadController.Host.timeToDetonation <= 0f) ? 3.5f : 1f);
            Cassie.GlitchyMessage(text, UnityEngine.Random.Range(0.1f, 0.14f) * num, UnityEngine.Random.Range(0.07f, 0.08f) * num);
        }

        public void EscapingEvent(EscapingEventArgs ev)
        {
            if (LastTeamSpawned.EscapableClasses.Contains(ev.Player.Role))
            {
                ev.IsAllowed = false;
                ev.Player.ChangeAdvancedRole(LastTeamSpawned, plugin.Config.FindAST(LastTeamSpawned.Name, LastTeamSpawned.EscapeClass), Extentions.InventoryDestroyType.Drop, true);
                MEC.Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.ShowFriendlyTeamDisplay();
                    CustomRoundEnder.UpdateRoundStatus();
                }
                );
            }
            else
            {
                Leaderboard.ClearPlayerFromLeaderBoards(ev.Player);
                ev.Player.InfoArea = PlayerInfoArea.CustomInfo;
                ev.Player.CustomInfo = string.Empty;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
                MEC.Timing.CallDelayed(0.1f, () =>
                    {
                        ev.Player.ChangeAdvancedTeam(plugin.Config.FindAT(ev.NewRole.GetTeam().ToString()));
                        ev.Player.ShowFriendlyTeamDisplay();
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
            Leaderboard.SetUpTeamLeaders();
            Respawns = 0;
            LastTeamSpawned = null;
            CassieHelper = null;
            ReferancedTeam = null;
        }

        public void MtfRespawnCassie(AnnouncingNtfEntranceEventArgs ev)
        {
            Respawns++;
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
            if (plugin.Config.TeamsListPromptsAtAnnouncement)
            {
                ShowAllPlayersTeams(10);
            }
            CassieHelper = null;
        }

        public void SlaughteredTeam(AdvancedTeam at, AdvancedTeam TerminatingTeam)
        {
            Cassie.MessageTranslated(at.CassieSlaughtered.Replace("{Terminating}", TerminatingTeam.SaidName).Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Terminated}", at.SaidName), at.CassieSlaughteredSubtitles.Replace("{Terminating}", $"<color={TerminatingTeam.Color}>{TerminatingTeam.Name}</color>").Replace("{Terminated}", $"<color={at.Color}>{at.Name}</color>").Replace("{SCPLeft}", ScpsLeft.ToString()));
        }

        public void PlayerDead(DyingEventArgs ev)
        {
            if (ev.Killer == null)
            {
                Log.Debug($"There is no Killer so using Target instead", plugin.Config.Debug);

                goto TeamLabel;
            }
            if (!ev.Killer.IsConnected && !ev.Target.IsConnected) return;
            Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} how many players are left {Leaderboard.GetTeamLeaderboard(ev.Target.GetAdvancedTeam().Name).PlayerPairs.Count}", plugin.Config.Debug);
            if (Leaderboard.GetTeamLeaderboard(ev.Target.GetAdvancedTeam().Name).PlayerPairs.Count == 0)
            {
                var e2 = new TeamEvents.AdvancedTeamSlaughteredEventArgs(ev.Target.GetAdvancedTeam(), ev.Killer.GetAdvancedTeam());
                e2.Slaughter();
                Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} sent slaughter event", plugin.Config.Debug);
                if (e2.IsAllowed && ev.Target.GetAdvancedTeam().Name != "SCP")
                {
                    Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} sent Termination Cassie", plugin.Config.Debug);
                    SlaughteredTeam(e2.AdvancedTeam, e2.TerminatingTeam);
                }
            }
            var e1 = new TeamEvents.AdvancedTeamDeadPlayerEventArgs(ev.Target.GetAdvancedTeam(), ev.Target, ev.Killer);
            e1.Kill();
            if (!e1.IsAllowed)
            {
                ev.IsAllowed = false;
                return;
            }
            LastHurtingPlayer = ev.Killer;
            LastHurtedTeam = ev.Target.GetAdvancedTeam();
            nickHelper = $"{ev.Target.Nickname}({ev.Target.GetAdvancedTeam().Name})";
            if (ev.Target.IsScp)
            {
                NineTailedFoxAnnouncer.ConvertSCP(ev.Target.Role.Type, out string nSpace, out string wSpace);
                CustomTeamScpTermination(wSpace, ev.Handler, ev.Killer.GetAdvancedTeam());
            }
        TeamLabel:
            Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} how many players are left {Leaderboard.GetTeamLeaderboard(ev.Target.GetAdvancedTeam().Name).PlayerPairs.Count - 1}", plugin.Config.Debug);
            if (Leaderboard.GetTeamLeaderboard(ev.Target.GetAdvancedTeam().Name).PlayerPairs.Count - 1 == 0)
            {
                var e2 = new TeamEvents.AdvancedTeamSlaughteredEventArgs(ev.Target.GetAdvancedTeam(), ev.Target.GetAdvancedTeam());
                e2.Slaughter();
                Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} sent slaughter event", plugin.Config.Debug);
                if (e2.IsAllowed && ev.Target.GetAdvancedTeam().Name != "SCP")
                {
                    if (e2.AdvancedTeam.CassieSlaughtered != string.Empty)
                    {
                        Log.Debug($"The team {ev.Target.GetAdvancedTeam().Name} sent Termination Cassie", plugin.Config.Debug);
                        SlaughteredTeam(e2.AdvancedTeam, e2.TerminatingTeam);
                    }
                }
            }
        }

        public Player LastHurtingPlayer;
        public AdvancedTeam LastHurtedTeam;

        public void PlayerHurt(HurtingEventArgs ev)
        {
            if (ev.Attacker == ev.Target) return;
            if (ev.Attacker == null) return;
            if (null == ev.Target) return;
            if (!ev.Attacker.IsConnected && !ev.Target.IsConnected) return;
            LastHurtingPlayer = ev.Attacker;
            LastHurtedTeam = ev.Target.GetAdvancedTeam();
            if (ev.Attacker.GetAdvancedTeam().ConfirmFriendshipWithTeam(ev.Target.GetAdvancedTeam()))
            {
                ev.Attacker.ShowHint("<color=red>Don't damage a friendly team</color>");
                if (ev.Target.Health <= ev.Amount * 0.3f)
                {
                    if (plugin.Config.FriendlyFireReflection)
                        if (!TeamKillList.Contains(ev.Attacker))
                        {
                            ev.Attacker.ShowHint($"<color=red>Team damage will now be reflected for {plugin.Config.ReflectionDamageTime} seconds</color>");
                            TeamKillList.Add(ev.Attacker);
                            Timing.CallDelayed(plugin.Config.ReflectionDamageTime, () => { TeamKillList.Remove(ev.Attacker); ev.Attacker.ShowHint("<color=green>Team reflection damage is disabled</color>"); });
                        }
                }
                if (!plugin.Config.FriendlyFire)
                {
                    ev.IsAllowed = false;
                }
                else
                {
                    if (TeamKillList.Contains(ev.Attacker))
                    {
                        if (TeamKillList.Contains(ev.Attacker))
                            ev.Attacker.Hurt(ev.Amount);
                    }
                    else
                    {
                        ev.Target.Hurt(ev.Amount * 0.3f);
                    }
                    ev.IsAllowed = false;
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

        public void RaycastHelper(Player ply)
        {
            if (plugin.Config.TeamsListPromptsAtAnnouncement && ply.HasHint) return;
            if (!UnityEngine.Physics.Raycast(ply.CameraTransform.position, ply.CameraTransform.forward, out UnityEngine.RaycastHit raycastHit, 999, 13))
                return;
            if (!UnityEngine.Physics.Raycast(ply.CameraTransform.position, ply.CameraTransform.forward, out UnityEngine.RaycastHit rayNormal, 10, 13))
                return;
            //Makes sure it only gets players???
            Player target = Player.Get(raycastHit.collider.gameObject);
            Player targetEnemy = Player.Get(rayNormal.collider.gameObject);
            if (target == null)
                return;
            if (ply.GetAdvancedTeam().ConfirmFriendshipWithTeam(target.GetAdvancedTeam()))
            {
                ply.ShowHint(TeamPlugin.Singleton.Translation.TeamIsFriendlyHint.Replace("(user)", target.Nickname).Replace("(dist)", raycastHit.distance.ToString()).Replace("(role)", target.GetAdvancedTeam().Name), 0.3f);
            }
            else if (targetEnemy != null && ply.GetAdvancedTeam().ConfirmEnemyshipWithTeam(targetEnemy.GetAdvancedTeam()))
            {
                ply.ShowHint(TeamPlugin.Singleton.Translation.TeamIsHostileHint.Replace("(user)", targetEnemy.Nickname).Replace("(role)", target.GetAdvancedTeam().Name), 0.3f);
            }
            else if (targetEnemy != null && ply.GetAdvancedTeam().ConfirmNeutralshipWithTeam(targetEnemy.GetAdvancedTeam()))
            {
                ply.ShowHint(TeamPlugin.Singleton.Translation.TeamIsNeutralHint.Replace("(user)", targetEnemy.Nickname).Replace("(role)", target.GetAdvancedTeam().Name), 0.3f);
            }
            else if (ply.GetAdvancedTeam().ConfirmRequiredshipWithTeam(target.GetAdvancedTeam()))
            {
                ply.ShowHint(TeamPlugin.Singleton.Translation.TeamIsRequiredHint.Replace("(user)", target.Nickname).Replace("(dist)", raycastHit.distance.ToString()).Replace("(role)", target.GetAdvancedTeam().Name), 0.3f);
            }
        }

        public void ReferancingTeam(TeamEvents.ReferancingTeamEventArgs ev)
        {
            HiddenInterference = new Random().Next(0, 99);
            int UnitNumPre = new Random().Next(1, 19);
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
            if (plugin.Config.TeamSpawnsOnlyIfEnemiesExist)
            {
                bool CanSpawn = false;
                foreach (AdvancedTeam at in plugin.Config.Teams)
                {
                    if (at.DoesExist())
                    {
                        if (ReferancedTeam.ConfirmEnemyshipWithTeam(at))
                        {
                            CanSpawn = true;
                            Log.Debug("Allowing team spawn because it has a common enemy", plugin.Config.Debug);
                            break;
                        }
                    }
                }
                if (CanSpawn)
                {
                    Log.Debug($"Team Spawn Check has been allowed! Allowing the Spawning team {ReferancedTeam.Name}", plugin.Config.Debug);
                }
                else
                {
                    Log.Debug($"Team Spawn Check has been denied! {ReferancedTeam.Name} doesn't have a common enemy spawning normal team...", plugin.Config.Debug);
                    ReferancedTeam = null;
                    return;
                }
            }
            if (ReferancedTeam.PlayBeforeSpawning && !ReferancedTeam.VanillaTeam && !PlayedAlready)
            {
                if (HiddenInterference < ev.AdvancedTeam.ChanceForHiddenMtfNato)
                {
                    if (plugin.Config.TeamsListPromptsAtAnnouncement)
                        ShowAllPlayersTeams(10);
                    Cassie.MessageTranslated(ReferancedTeam.CassieAnnouncement.Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Unit}", $"NATO_{UnitNamePre[0]}").Replace("{UnitNum}", UnitNumPre.ToString()), ReferancedTeam.CassieAnnouncementSubtitles.Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Unit}", $"{UnitNamePre}").Replace("{UnitNum}", UnitNumPre.ToString()));
                }
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

        public void RoleChange(ChangingRoleEventArgs ev)
        {
            PlayerTimesAlive[ev.Player] = Round.ElapsedTime;
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

        public void RoundEnd(RoundEndedEventArgs ev)
        {
            Leaderboard.DestroyTeamLeaders();
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
            if (ev.Player.GetAdvancedTeam().ConfirmFriendshipWithTeam(plugin.Config.FindAT("SCP")))
            {
                if (plugin.Config.FriendlyFire) return;
                ev.IsAllowed = false;
            }
        }

        public void Scp106FemurBreakerPreventer(EnteringFemurBreakerEventArgs ev)
        {
            if (ev.Player.GetAdvancedTeam().SpawnRoom == RoomType.Hcz106 || ev.Player.GetAdvancedTeam().ConfirmFriendshipWithTeam(plugin.Config.FindAT("SCP")))
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
                ShowAllPlayersTeams(10);
            }
            if (!ev.Killer.GetAdvancedTeam().VanillaTeam)
            {
                ev.IsAllowed = false;
            }
        }

        public void ShowAllPlayersTeams(int secs)
        {
            foreach (Player p in Player.List)
            {
                if (!p.GetAdvancedTeam().Spectator)
                    p.ShowFriendlyTeamDisplay(secs);
            }
        }

        public void TeamSpawning(RespawningTeamEventArgs ev)
        {
            if (ReferancedTeam == null)
            {
                Log.Warn("Possible Force Team Spawn...");
                return;
            }
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
            int UnitNum = new Random().Next(1, 19);
            string UnitName = Letters[new Random().Next(0, 26)];
            if (!ReferancedTeam.PlayBeforeSpawning && !ReferancedTeam.VanillaTeam && !PlayedAlready && ev.NextKnownTeam != Respawning.SpawnableTeamType.NineTailedFox)
                if (HiddenInterference < ReferancedTeam.ChanceForHiddenMtfNato)
                {
                    if (plugin.Config.TeamsListPromptsAtAnnouncement)
                        ShowAllPlayersTeams(10);
                    Cassie.MessageTranslated(ReferancedTeam.CassieAnnouncement.Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Unit}", $"NATO_{UnitName[0]}").Replace("{UnitNum}", UnitNum.ToString()), ReferancedTeam.CassieAnnouncementSubtitles.Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Unit}", $"{UnitName}").Replace("{UnitNum}", UnitNum.ToString()));
                }

            Dictionary<string, int> Helper = new Dictionary<string, int>();
            foreach (string t in ReferancedTeam.SpawnOrder)
            {
                Helper[t.Split(':')[0]] = int.Parse(t.Split(':')[1]);
            }
            foreach (Player p in ev.Players)
            {
                if (Helper.Values.Count == 0)
                {
                    p.SetRole(RoleType.Spectator, Exiled.API.Enums.SpawnReason.Died);
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
                p.ChangeAdvancedRole(ReferancedTeam, TeamPlugin.Singleton.Config.FindAST(ReferancedTeam.Name, Helper.First().Key), Extentions.InventoryDestroyType.Destroy, true);
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

        #endregion Public Methods

        #region Private Fields

        private string[] Letters = new string[26]
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

        private string nickHelper;
        private bool PlayedAlready = false;
        private Plugin<TeamConfig> plugin;
        private int Respawns = 0;

        #endregion Private Fields

        #region Private Properties

        private int ScpsLeft => Player.List.Where(e => e.IsScp && e.Role.Type != RoleType.Scp0492).Count();

        #endregion Private Properties

        #region Private Methods

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
                                    ply.ShowFriendlyTeamDisplay();
                                else
                                    ply.ShowFriendlyTeamDisplay(1, PlayerTimesAlive[ply].TotalSeconds + plugin.Config.ShowEnemyTeamsForTime < Round.ElapsedTime.TotalSeconds);
                            }
                        }
                        if (plugin.Config.ShowFriendlyHint)
                            RaycastHelper(ply);
                    }
                }
            }
        }

        #endregion Private Methods
    }
}