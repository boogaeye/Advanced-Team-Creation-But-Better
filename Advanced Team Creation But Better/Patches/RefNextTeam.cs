using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace ATCBB.Patches
{
    [HarmonyPatch(typeof(Respawning.RespawnTickets), nameof(Respawning.RespawnTickets.DrawRandomTeam))]
    public class RefNextTeam
    {
        public static void Postfix(ref Respawning.SpawnableTeamType __result)
        {
            new TeamAPI.Events.TeamEvents.ReferancingTeamEventArgs(__result).CalculateTeams();
        }
    }
}