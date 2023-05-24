using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedTeamCreation.TeamAPI;
using AdvancedTeamCreation.TeamAPI.Extentions;
using Exiled.API.Features;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles;
using PluginAPI.Enums;
using PluginAPI.Events;
using Respawning;
using Respawning.NamingRules;
using static AdvancedTeamCreation.TeamAPI.Events.TeamEvents;

namespace AdvancedTeamCreation.Patches
{
    //TODO Find Spawn Tickets?
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
    public class RefNextTeam
    {
        public static bool Prefix(RespawnManager __instance)
        {
            ReferancingTeamEventArgs ev = new ReferancingTeamEventArgs(__instance.NextKnownTeam);
            if (__instance.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox)
                ev = new TeamAPI.Events.TeamEvents.ReferancingTeamEventArgs(Respawning.SpawnableTeamType.NineTailedFox);
            else if (__instance.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
                ev = new TeamAPI.Events.TeamEvents.ReferancingTeamEventArgs(Respawning.SpawnableTeamType.ChaosInsurgency);
            ev.CalculateTeams();

            if (ev.IsAllowed)
            {
                if (ev.AdvancedTeam.VanillaTeam)
                {
                    return true;
                }
                else
                {
                    if (!RespawnManager.SpawnableTeams.TryGetValue(__instance.NextKnownTeam, out var value) || __instance.NextKnownTeam == SpawnableTeamType.None)
                    {
                        ServerConsole.AddLog(string.Concat("Fatal error. Team '", __instance.NextKnownTeam, "' is undefined."), ConsoleColor.Red);
                    }
                    else
                    {
                        if (!EventManager.ExecuteEvent(ServerEventType.TeamRespawn, __instance.NextKnownTeam))
                        {
                            RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, __instance.NextKnownTeam);
                            __instance.NextKnownTeam = SpawnableTeamType.None;
                        }
                        else
                        {
                            List<ReferenceHub> list = ReferenceHub.AllHubs.Where(__instance.CheckSpawnable).ToList();
                            if (__instance._prioritySpawn)
                            {
                                list = list.OrderByDescending((ReferenceHub item) => item.roleManager.CurrentRole.ActiveTime).ToList();
                            }
                            else
                            {
                                list.ShuffleList();
                            }
                            int maxWaveSize = ev.AdvancedTeam.GetMaxWave();
                            int num = list.Count;
                            if (num > maxWaveSize)
                            {
                                list.RemoveRange(maxWaveSize, num - maxWaveSize);
                                num = maxWaveSize;
                            }
                            if (num > 0 && UnitNamingRule.TryGetNamingRule(__instance.NextKnownTeam, out var rule))
                            {
                                UnitNameMessageHandler.SendNew(__instance.NextKnownTeam, rule);
                            }
                            list.ShuffleList();
                            List<ReferenceHub> list2 = ListPool<ReferenceHub>.Shared.Rent();
                            Queue<AdvancedTeamSubclass> queue = ev.AdvancedTeam.GetSubclassQueue();
                            foreach (ReferenceHub item in list)
                            {
                                try
                                {
                                    AdvancedTeamSubclass newRole = queue.Dequeue();
                                    (new Player(item)).ChangeAdvancedRole(newRole, Extentions.InventoryDestroyType.Destroy, true);
                                    list2.Add(item);
                                    ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + item.LoggedNameFromRefHub() + " respawned as " + newRole.Name + ".", ServerLogs.ServerLogType.GameEvent);
                                }
                                catch (Exception ex)
                                {
                                    if (item != null)
                                    {
                                        ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + item.LoggedNameFromRefHub() + " couldn't be spawned. Err msg: " + ex.Message, ServerLogs.ServerLogType.GameEvent);
                                    }
                                    else
                                    {
                                        ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Couldn't spawn a player - target's ReferenceHub is null.", ServerLogs.ServerLogType.GameEvent);
                                    }
                                }
                            }
                            if (list2.Count > 0)
                            {
                                ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "RespawnManager has successfully spawned " + list2.Count + " players as " + ev.AdvancedTeam.Name + "!", ServerLogs.ServerLogType.GameEvent);
                                RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, __instance.NextKnownTeam);
                            }
                            ListPool<ReferenceHub>.Shared.Return(list2);
                            __instance.NextKnownTeam = SpawnableTeamType.None;
                        }
                    }
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}