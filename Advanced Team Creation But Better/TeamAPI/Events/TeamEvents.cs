using Exiled.API.Features;
using System;
using Random = UnityEngine.Random;

namespace ATCBB.TeamAPI.Events
{
    public delegate void TeamEvents<TEventArgs>(TEventArgs eventArgs) where TEventArgs : EventArgs;

    public static class TeamEvents
    {
        #region Public Events

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

        #endregion Public Events

        #region Public Classes

        public class ReferancingTeamEventArgs : EventArgs
        {
            #region Public Constructors

            public ReferancingTeamEventArgs(Respawning.SpawnableTeamType team)
            {
                SupposedTeam = team;
                if (TeamPlugin.Singleton.Config.VanillaTeamsHavePriority)
                {
                    foreach (AdvancedTeam at in TeamPlugin.Singleton.Config.Teams)
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
                    foreach (AdvancedTeam at in Extentions.Extentions.AdvancedTeamsOnly)
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
                        AdvancedTeam = TeamPlugin.Singleton.Config.FindAT(Team.CHI.ToString());
                        break;

                    case Respawning.SpawnableTeamType.NineTailedFox:
                        AdvancedTeam = TeamPlugin.Singleton.Config.FindAT(Team.MTF.ToString());
                        break;
                }
            }

            #endregion Public Constructors

            #region Public Properties

            public AdvancedTeam AdvancedTeam { get; }
            public bool IsAllowed { get; set; } = true;
            public Respawning.SpawnableTeamType SupposedTeam { get; }

            #endregion Public Properties

            #region Public Methods

            public void CalculateTeams()
            {
                Invoke();
            }

            #endregion Public Methods

            #region Private Methods

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

            #endregion Private Methods
        }

        public class AdvancedTeamSlaughteredEventArgs : EventArgs
        {
            #region Public Constructors

            public AdvancedTeamSlaughteredEventArgs(AdvancedTeam at, AdvancedTeam terminatingTeam)
            {
                AdvancedTeam = at;
                TerminatingTeam = terminatingTeam;
            }

            #endregion Public Constructors

            #region Public Properties

            public AdvancedTeam AdvancedTeam { get; }
            public AdvancedTeam TerminatingTeam { get; }
            public bool IsAllowed { get; set; } = true;

            #endregion Public Properties

            #region Public Methods

            public void Slaughter()
            {
                Invoke();
            }

            #endregion Public Methods

            #region Private Methods

            private void Invoke()
            {
                AdvancedTeamSlaughtered?.Invoke(this);
            }

            #endregion Private Methods
        }

        public class AdvancedTeamDeadPlayerEventArgs : EventArgs
        {
            #region Public Constructors

            public AdvancedTeamDeadPlayerEventArgs(AdvancedTeam at, Player target, Player killer)
            {
                AdvancedTeam = at;
                Target = target;
                Killer = killer;
            }

            #endregion Public Constructors

            #region Public Properties

            public AdvancedTeam AdvancedTeam { get; }
            public Player Target { get; }
            public Player Killer { get; }
            public bool IsAllowed { get; set; } = true;

            #endregion Public Properties

            #region Public Methods

            public void Kill()
            {
                Invoke();
            }

            #endregion Public Methods

            #region Private Methods

            private void Invoke()
            {
                AdvancedTeamDeadPlayer?.Invoke(this);
            }

            #endregion Private Methods
        }

        public class SettingAdvancedTeamEventArgs : EventArgs
        {
            #region Public Constructors

            public SettingAdvancedTeamEventArgs(Player ply, AdvancedTeam advancedTeam, AdvancedTeamSubclass advancedSubTeam)
            {
                AdvancedTeam = advancedTeam;
                AdvancedSubTeam = advancedSubTeam;
                Player = ply;
            }

            #endregion Public Constructors

            #region Public Properties

            public AdvancedTeamSubclass AdvancedSubTeam { get; set; }
            public AdvancedTeam AdvancedTeam { get; set; }
            public bool IsAllowed { get; set; } = true;
            public Player Player { get; }

            #endregion Public Properties

            #region Public Methods

            public void Invoke()
            {
                var teamev = SettingAdvancedTeam;
                teamev?.Invoke(this);
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}