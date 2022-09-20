using AdvancedTeamCreation.TeamAPI;
using AdvancedTeamCreation.TeamAPI.Events;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using AdvancedTeamCreation.TeamAPI.Helpers;

namespace AdvancedTeamCreation
{
    public class TeamEventHandler
    {
        public Dictionary<Player, TimeSpan> PlayerTimesAlive = new Dictionary<Player, TimeSpan>();

        public TeamEventHandler(Plugin<TeamConfig> Plugin)
        {
            plugin = Plugin;
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
            MessageHelper.RollForHiddenSpawn();
            RespawnHelper.SetReferance(ev.AdvancedTeam);
            if (plugin.Config.TeamSpawnsOnlyIfEnemiesExist)
            {
                Log.Debug("Checking if team meets enemy conditions", plugin.Config.Debug);
                bool CanSpawn = false;
                foreach (AdvancedTeam at in UnitHelper.Teams)
                {
                    if (at.DoesExist())
                    {
                        if (RespawnHelper.ReferancedTeam.ConfirmEnemyshipWithTeam(at))
                        {
                            CanSpawn = true;
                            Log.Debug("Allowing team spawn because it has a common enemy", plugin.Config.Debug);
                            break;
                        }
                    }
                }
                if (CanSpawn)
                {
                    Log.Debug($"Team Spawn Check has been allowed! Allowing the Spawning team {RespawnHelper.ReferancedTeam.Name}", plugin.Config.Debug);
                }
                else
                {
                    Log.Debug($"Team Spawn Check has been denied! {RespawnHelper.ReferancedTeam.Name} doesn't have a common enemy spawning normal team...", plugin.Config.Debug);
                    RespawnHelper.SetReferance(null);
                    return;
                }
            }
            if (RespawnHelper.ReferancedTeam.PlayBeforeSpawning && !RespawnHelper.ReferancedTeam.VanillaTeam)
            {
                MessageHelper.PlayTeamAnnouncement(RespawnHelper.ReferancedTeam);
            }

            if (TeamPlugin.assemblyTimer != null && !ev.AdvancedTeam.VanillaTeam && ev.SupposedTeam != Respawning.SpawnableTeamType.None)
            {
                HudHelper.LoadTimerConfig(RespawnHelper.ReferancedTeam);
            }
        }

        private Plugin<TeamConfig> plugin;
    }
}