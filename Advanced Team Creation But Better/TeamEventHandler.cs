using AdvancedTeamCreation.TeamAPI;
using AdvancedTeamCreation.TeamAPI.Events;
using Exiled.API.Features;
using AdvancedTeamCreation.TeamAPI.Helpers;

namespace AdvancedTeamCreation
{
    public class TeamEventHandler
    {
        public TeamEventHandler(Plugin<TeamConfig> Plugin)
        {
            plugin = Plugin;
        }

        public void ReferancingTeam(TeamEvents.ReferancingTeamEventArgs ev)
        {
            Log.Debug($"Got Referance {ev.AdvancedTeam.Name}");
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
                Log.Debug("Checking if team meets enemy conditions");
                bool CanSpawn = false;
                foreach (AdvancedTeam at in UnitHelper.Teams)
                {
                    if (at.DoesExist())
                    {
                        if (RespawnHelper.ReferancedTeam.ConfirmEnemyshipWithTeam(at))
                        {
                            CanSpawn = true;
                            Log.Debug("Allowing team spawn because it has a common enemy");
                            break;
                        }
                    }
                }
                if (CanSpawn)
                {
                    Log.Debug($"Team Spawn Check has been allowed! Allowing the Spawning team {RespawnHelper.ReferancedTeam.Name}");
                }
                else
                {
                    Log.Debug($"Team Spawn Check has been denied! {RespawnHelper.ReferancedTeam.Name} doesn't have a common enemy spawning normal team...");
                    RespawnHelper.SetReferance(null);
                    return;
                }
            }
            if (RespawnHelper.ReferancedTeam.PlayBeforeSpawning && !RespawnHelper.ReferancedTeam.VanillaTeam)
            {
                Log.Debug("Playing Team Announcement from Referance!");
                MessageHelper.PlayTeamAnnouncement(RespawnHelper.ReferancedTeam);
            }

            if (TeamPlugin.assemblyTimer != null && !ev.AdvancedTeam.VanillaTeam && ev.SupposedTeam != Respawning.SpawnableTeamType.None)
            {
                Log.Debug("Loading Timer Config(Respawn Timer Support)");
                //HudHelper.LoadTimerConfig(RespawnHelper.ReferancedTeam);
            }
        }

        private Plugin<TeamConfig> plugin;
    }
}