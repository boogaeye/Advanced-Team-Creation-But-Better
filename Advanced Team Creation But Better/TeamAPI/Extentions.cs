using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using CustomItems.API;
using MEC;
using UnityEngine;

namespace ATCBB.TeamAPI.Extentions
{
    public static class Extentions
    {
        public enum InventoryDestroyType
        {
            None,
            Drop,
            Destroy
        }
        public static void ChangeAdvancedRole(this Player ply, AdvancedTeam at, AdvancedTeamSubclass ast, InventoryDestroyType ChangeInventory = InventoryDestroyType.None, bool ChangePosition = false)
        {
            TeamPlugin.Singleton.TeamEventHandler.Leaderboard.ClearPlayerFromLeaderBoards(ply);
            TeamPlugin.Singleton.TeamEventHandler.Leaderboard.GetTeamLeaderboard(at.Name).AddPlayer(ply, ast);
            Timing.CallDelayed(0.5f, () =>
            {
                if (ChangePosition && at.SpawnRoom != RoomType.Surface && !Warhead.IsInProgress && !Warhead.IsDetonated)
                    foreach (Room r in Room.List)
                    {
                        if (r.Type == at.SpawnRoom)
                        {
                            if (PlayerMovementSync.FindSafePosition((r.Doors.First().Position + (Vector3.forward * 1.5f)) + Vector3.up * 1.5f, out Vector3 vec))
                            {
                                r.Doors.First().IsOpen = true;
                                for (var i = 0; i < TeamPlugin.Singleton.Config.TeleportRetries && ply.Position != vec; i++)
                                {
                                    ply.Teleport(vec);
                                }
                                
                            }
                            else
                            {
                                Log.Warn("Could not find safe position sending to surface");
                            }
                        }
                    }
                if (ChangeInventory == InventoryDestroyType.Destroy)
                    ply.ClearInventory();
                else if (ChangeInventory == InventoryDestroyType.Drop)
                {
                    ply.DropItems();
                }
                ply.SetRole(ast.Model, SpawnReason.Respawn, true);
                ply.InfoArea &= ~PlayerInfoArea.Role;
                ply.CustomInfo = $"<color={ast.Color}>{ast.RoleDisplay}</color>";
                ply.MaxHealth = ast.MaxHP;
                ply.Health = ast.HP;
                ply.ShowHint(ast.Hint.Replace("{Team}", $"<color={at.Color}>{at.Name}</color>").Replace("{Role}", $"<color={ast.Color}>{ast.RoleDisplay}</color>"), 10);
                foreach (string item in ast.Inventory)
                {
                    try
                    {
                        if (ItemConversionHelper.TryGetItemTypeFromString(item, out ItemType i))
                        {
                            ply.AddItem(i);
                        }
                        else if (ItemConversionHelper.TryGetCustomItemTypeFromString(item, out CustomItems.API.CustomItem ci))
                        {
                            ply.GiveItem(ci, false);
                        }
                        else
                        {
                            Log.Warn($"{item} probably doesn't exist as an item...");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                        Log.Error(e.StackTrace);
                    }
                }
            });
        }
        public static void ChangeAdvancedTeam(this Player ply, AdvancedTeam at)
        {
            TeamPlugin.Singleton.TeamEventHandler.Leaderboard.ClearPlayerFromLeaderBoards(ply);
            TeamPlugin.Singleton.TeamEventHandler.Leaderboard.GetTeamLeaderboard(at.Name).AddPlayer(ply);
        }
    }
}