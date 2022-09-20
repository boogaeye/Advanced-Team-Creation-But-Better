using System;
using HarmonyLib;

namespace AdvancedTeamCreation.Patches
{
    [HarmonyPatch(typeof(Respawning.RespawnTickets), nameof(Respawning.RespawnTickets.DrawRandomTeam))]
    public class RefNextTeam
    {
        public static void Postfix(ref Respawning.SpawnableTeamType __result)
        {
            try
            {
                if (__result == Respawning.SpawnableTeamType.NineTailedFox)
                    new TeamAPI.Events.TeamEvents.ReferancingTeamEventArgs(Respawning.SpawnableTeamType.NineTailedFox).CalculateTeams();
                else if (__result == Respawning.SpawnableTeamType.ChaosInsurgency)
                    new TeamAPI.Events.TeamEvents.ReferancingTeamEventArgs(Respawning.SpawnableTeamType.ChaosInsurgency).CalculateTeams();
            }
            catch (Exception)
            {
            }
        }
    }
}