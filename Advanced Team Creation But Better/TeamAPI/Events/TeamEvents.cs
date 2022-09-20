﻿using AdvancedTeamCreation.TeamAPI.Helpers;
using Exiled.API.Features;
using System;
using Random = UnityEngine.Random;

namespace AdvancedTeamCreation.TeamAPI.Events
{
    public delegate void TeamEvents<TEventArgs>(TEventArgs eventArgs) where TEventArgs : EventArgs;

    public static class TeamEvents
    {
        /// <summary>
        /// When a team gets referanced and is about to spawn
        /// </summary>
        public static event TeamEvents<ReferancingTeamEventArgs> ReferancingTeam;

        /// <summary>
        /// When a player gets set to a team
        /// </summary>
        public static event TeamEvents<SettingAdvancedTeamEventArgs> SettingAdvancedTeam;

        /// <summary>
        /// On a full team killed
        /// </summary>
        public static event TeamEvents<AdvancedTeamSlaughteredEventArgs> AdvancedTeamSlaughtered;

        public static event TeamEvents<AdvancedTeamDeadPlayerEventArgs> AdvancedTeamDeadPlayer;

        public class ReferancingTeamEventArgs : EventArgs
        {
            public ReferancingTeamEventArgs(Respawning.SpawnableTeamType team)
            {
                if (RespawnHelper.ReferancedTeam != null)
                {
                    AdvancedTeam = RespawnHelper.ReferancedTeam;
                    return;
                }
                SupposedTeam = team;
                if (TeamPlugin.Singleton.Config.VanillaTeamsHavePriority)
                {
                    foreach (AdvancedTeam at in UnitHelper.Teams)
                    {
                        if (DetectChance(at))
                        {
                            AdvancedTeam = at;
                            return;
                        }
                    }
                }
                else
                {
                    foreach (AdvancedTeam at in UnitHelper.AdvancedTeamsOnly)
                    {
                        if (DetectChance(at))
                        {
                            AdvancedTeam = at;
                            return;
                        }
                    }
                }
                switch (team)
                {
                    case Respawning.SpawnableTeamType.ChaosInsurgency:
                        AdvancedTeam = UnitHelper.FindAT(Team.CHI.ToString());
                        break;

                    case Respawning.SpawnableTeamType.NineTailedFox:
                        AdvancedTeam = UnitHelper.FindAT(Team.MTF.ToString());
                        break;
                }
            }

            public AdvancedTeam AdvancedTeam { get; }
            public bool IsAllowed { get; set; } = true;
            public Respawning.SpawnableTeamType SupposedTeam { get; }

            public void CalculateTeams()
            {
                Invoke();
            }

            private bool DetectChance(AdvancedTeam at)
            {
                switch (at.BindedTeam)
                {
                    case Respawning.SpawnableTeamType.NineTailedFox:
                        if (SupposedTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
                            return false;
                        break;

                    case Respawning.SpawnableTeamType.ChaosInsurgency:
                        if (SupposedTeam == Respawning.SpawnableTeamType.NineTailedFox)
                            return false;
                        break;
                }
                return Random.Range(0, 99) < at.Chance;
            }

            private void Invoke()
            {
                ReferancingTeam.Invoke(this);
            }
        }

        public class AdvancedTeamSlaughteredEventArgs : EventArgs
        {
            public AdvancedTeamSlaughteredEventArgs(AdvancedTeam at, AdvancedTeam terminatingTeam)
            {
                AdvancedTeam = at;
                TerminatingTeam = terminatingTeam;
            }

            public AdvancedTeam AdvancedTeam { get; }
            public AdvancedTeam TerminatingTeam { get; }
            public bool IsAllowed { get; set; } = true;

            public void Slaughter()
            {
                Invoke();
            }

            private void Invoke()
            {
                AdvancedTeamSlaughtered?.Invoke(this);
            }
        }

        public class AdvancedTeamDeadPlayerEventArgs : EventArgs
        {
            public AdvancedTeamDeadPlayerEventArgs(AdvancedTeam at, Player target, Player killer)
            {
                AdvancedTeam = at;
                Target = target;
                Killer = killer;
            }

            public AdvancedTeam AdvancedTeam { get; }
            public Player Target { get; }
            public Player Killer { get; }
            public bool IsAllowed { get; set; } = true;

            public void Kill()
            {
                Invoke();
            }

            private void Invoke()
            {
                AdvancedTeamDeadPlayer?.Invoke(this);
            }
        }

        public class SettingAdvancedTeamEventArgs : EventArgs
        {
            public SettingAdvancedTeamEventArgs(Player ply, AdvancedTeam advancedTeam, AdvancedTeamSubclass advancedSubTeam)
            {
                AdvancedTeam = advancedTeam;
                AdvancedSubTeam = advancedSubTeam;
                Player = ply;
            }

            public AdvancedTeamSubclass AdvancedSubTeam { get; set; }
            public AdvancedTeam AdvancedTeam { get; set; }
            public bool IsAllowed { get; set; } = true;
            public Player Player { get; }

            public void Invoke()
            {
                var teamev = SettingAdvancedTeam;
                teamev?.Invoke(this);
            }
        }
    }
}