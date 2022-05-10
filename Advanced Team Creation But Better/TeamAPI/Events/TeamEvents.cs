using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATCBB.TeamAPI.Extentions;
using Random = UnityEngine.Random;

namespace ATCBB.TeamAPI.Events
{
    public delegate void TeamEvents<TEventArgs>(TEventArgs eventArgs) where TEventArgs : EventArgs;

    public static class TeamEvents
    {
        public static event TeamEvents<ReferancingTeamEventArgs> ReferancingTeam;

        public static event TeamEvents<SettingAdvancedTeamEventArgs> SettingAdvancedTeam;

        public class SettingAdvancedTeamEventArgs : EventArgs
        {
            public SettingAdvancedTeamEventArgs(AdvancedTeam advancedTeam)
            {
                AdvancedTeam = advancedTeam;
            }

            public bool IsAllowed { get; set; } = true;
            public AdvancedTeam AdvancedTeam { get; set; }

            public void Invoke()
            {
                var teamev = SettingAdvancedTeam;
                teamev.Invoke(this);
            }
        }

        public class ReferancingTeamEventArgs : EventArgs
        {
            public ReferancingTeamEventArgs(Respawning.SpawnableTeamType team)
            {
                SupposedTeam = team;
                foreach (AdvancedTeam at in TeamPlugin.Singleton.Config.Teams)
                {
                    if (Random.Range(0, 99) < at.Chance)
                    {
                        AdvancedTeam = at;
                        return;
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

            public bool IsAllowed { get; set; } = true;
            public Respawning.SpawnableTeamType SupposedTeam { get; }
            public AdvancedTeam AdvancedTeam { get; }

            public void CalculateTeams()
            {
                Invoke();
            }

            private void Invoke()
            {
                ReferancingTeam.Invoke(this);
            }
        }
    }
}