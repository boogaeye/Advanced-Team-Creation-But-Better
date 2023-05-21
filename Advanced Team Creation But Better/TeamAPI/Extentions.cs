using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using AdvancedTeamCreation.TeamAPI.Events;
using AdvancedTeamCreation.TeamAPI.Helpers;

namespace AdvancedTeamCreation.TeamAPI.Extentions
{
    public static class Extentions
    {
        public enum InventoryDestroyType
        {
            None,
            Drop,
            Destroy
        }

        public static int GetMaxWave(this AdvancedTeam at)
        {
            int c = 0;
            foreach (var item in at.SpawnOrder)
            {
                string i = item.Split(':')[1];
                c += int.Parse(i);
            }
            return c;
        }

        public static Queue<AdvancedTeamSubclass> GetSubclassQueue(this AdvancedTeam at)
        {
            Queue<AdvancedTeamSubclass> queue = new Queue<AdvancedTeamSubclass>();
            foreach (var item in at.SpawnOrder)
            {
                string n = item.Split(':')[0];
                string i = item.Split(':')[1];
                int z = int.Parse(i);
                for (int y = 0; y < z; y++)
                {
                    queue.Enqueue(UnitHelper.FindAST(at.Name, n));
                }
            }
            return queue;
        }

        public static AdvancedTeam GetAdvancedTeam(this Player ply)
        {
            foreach (LeaderboardHelper.TeamLeaderboard tl in RespawnHelper.Leaderboard.TeamLeaderboards)
            {
                if (tl.PlayerPairs.Keys.Contains(ply))
                {
                    return tl.Team;
                }
            }
            return null;
        }

        public static AdvancedTeamSubclass GetAdvancedSubTeam(this Player ply)
        {
            foreach (LeaderboardHelper.TeamLeaderboard tl in RespawnHelper.Leaderboard.TeamLeaderboards)
            {
                if (tl.PlayerPairs.Keys.Contains(ply))
                {
                    return tl.PlayerPairs[ply];
                }
            }
            return null;
        }

        public static void ShowPlayerTeamDisplay(this Player ply, int sec = 3, bool ShowOnlyImportantTeams = false)
        {
            if (!TeamPlugin.Singleton.Config.ShowTeamsList) return;
            Translations t = TeamPlugin.Singleton.Translation;
            string[] desc = ply.GetAdvancedSubTeam().Description.Split(':');
            string sb = "";
            foreach (string s in desc)
            {
                sb += $"<align=right><size=45%>{s}</align></size>\n";
            }
            sb += "\n";

            if (ply.GetAdvancedTeam().GetAllFriendlyTeams().Any())
            {
                sb += t.FriendlyTeamHeader;
                //Get Friendly Teams
                if (TeamPlugin.Singleton.Config.ShowTeamInListIfAliveOnly)
                {
                    //Made to reduce list size though reduces secrecy of team spawning as well
                    foreach (AdvancedTeam at in ply.GetAdvancedTeam().GetAllFriendlyTeams().Where(e => e.DoesExist()))
                    {
                        sb += $"\n{t.FriendlyTeamListed.Replace("(TEAM)", $"<color={at.Color}>{at.Name}</color>").Replace("(TEAMNoColor)", $"{at.Name}")}";
                    }
                }
                else
                {
                    foreach (AdvancedTeam at in ply.GetAdvancedTeam().GetAllFriendlyTeams())
                    {
                        sb += $"\n{t.FriendlyTeamListed.Replace("(TEAM)", $"<color={at.Color}>{at.Name}</color>").Replace("(TEAMNoColor)", $"{at.Name}")}";
                    }
                }
            }

            if (ply.GetAdvancedTeam().GetAllHostileTeams().Any() && !ShowOnlyImportantTeams)
            {
                //Get Enemy teams that are hostile
                sb += $"\n\n{t.HostileTeamHeader}";
                foreach (AdvancedTeam at in ply.GetAdvancedTeam().GetAllHostileTeams())
                {
                    sb += $"\n{t.HostileTeamListed.Replace("(TEAM)", $"<color={at.Color}>{at.Name}</color>").Replace("(TEAMNoColor)", $"{at.Name}")}";
                }
            }
            if (ply.GetAdvancedTeam().GetAllNeutralTeams().Any() && !ShowOnlyImportantTeams)
            {
                //Get Neutral teams not always hostile
                sb += $"\n\n{t.NeutralTeamHeader}";
                foreach (AdvancedTeam at in ply.GetAdvancedTeam().GetAllNeutralTeams())
                {
                    sb += $"\n{t.NeutralTeamListed.Replace("(TEAM)", $"<color={at.Color}>{at.Name}</color>").Replace("(TEAMNoColor)", $"{at.Name}")}";
                }
            }

            if (ply.GetAdvancedTeam().GetAllRequiredTeams().Any())
            {
                //Get Required teams friendlies that you need to help (escape maybe?)
                sb += $"\n\n{t.EscapeTeamHeader}";
                if (TeamPlugin.Singleton.Config.ShowTeamInListIfAliveOnly)
                {
                    //Made to reduce list size though reduces secrecy of team spawning as well
                    foreach (AdvancedTeam at in ply.GetAdvancedTeam().GetAllRequiredTeams().Where(e => e.DoesExist()))
                    {
                        sb += $"\n{t.EscapeTeamListed.Replace("(TEAM)", $"<color={at.Color}>{at.Name}</color>").Replace("(TEAMNoColor)", $"{at.Name}")}";
                    }
                }
                else
                {
                    foreach (AdvancedTeam at in ply.GetAdvancedTeam().GetAllRequiredTeams())
                    {
                        sb += $"\n{t.EscapeTeamListed.Replace("(TEAM)", $"<color={at.Color}>{at.Name}</color>").Replace("(TEAMNoColor)", $"{at.Name}")}";
                    }
                }
            }

            ply.ShowHint(sb, sec);
        }

        public static void ChangeAdvancedRole(this Player ply, AdvancedTeam at, AdvancedTeamSubclass ast, InventoryDestroyType ChangeInventory = InventoryDestroyType.None, bool ChangePosition = false)
        {
            if (!LeaderboardHelper.TeamsInstantiable) return;
            var av = new TeamEvents.SettingAdvancedTeamEventArgs(ply, at, ast);
            var t = av.AdvancedTeam;
            var ts = av.AdvancedSubTeam;
            if (!av.IsAllowed) return;
            at = av.AdvancedTeam;
            ast = av.AdvancedSubTeam;
            RespawnHelper.Leaderboard.ClearPlayerFromLeaderBoards(ply);
            RespawnHelper.Leaderboard.GetTeamLeaderboard(at.Name).AddPlayer(ply, ast);
            Timing.CallDelayed(0.5f, () =>
            {
                if (ChangePosition && at.SpawnRoom != RoomType.Surface && !Warhead.IsInProgress && !Warhead.IsDetonated)
                    foreach (Room r in Room.List)
                    {
                        if (r.Type == at.SpawnRoom)
                        {
                            Vector3 vec = r.Doors.First().Position + Vector3.forward * 1.5f + Vector3.up * 1.5f;
                            r.Doors.First().IsOpen = true;
                            for (var i = 0; i < TeamPlugin.Singleton.Config.TeleportRetries && ply.Position != vec; i++)
                            {
                                ply.Teleport(vec);
                            }
                        }
                    }
                if (ChangeInventory == InventoryDestroyType.Destroy)
                {
                    ply.ClearInventory();
                    ply.Ammo.Clear();
                }
                else if (ChangeInventory == InventoryDestroyType.Drop)
                {
                    ply.DropItems();
                    ply.ClearInventory();
                    ply.Ammo.Clear();
                }
                ply.RoleManager.ServerSetRole(ast.Model, PlayerRoles.RoleChangeReason.Respawn, PlayerRoles.RoleSpawnFlags.None);
                ply.CustomInfo = $"{ply.Nickname}\n{ast.RoleDisplay}";
                ply.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
                ply.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
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
                        else if (ItemConversionHelper.TryGetCustomItemTypeFromString(item, out Exiled.CustomItems.API.Features.CustomItem ci))
                        {
                            ci.Give(ply, false);
                        }
                        else if (uint.TryParse(item, out uint _i) && ItemConversionHelper.TryGetCustomItemTypeFromInt(_i, out Exiled.CustomItems.API.Features.CustomItem _ci))
                        {
                            ci.Give(ply, false);
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
                foreach (string item in ast.AmmoInventory)
                {
                    string[] breaker = item.Split(':');
                    try
                    {
                        if (ItemConversionHelper.TryGetAmmoTypeFromString(breaker[0], out AmmoType i))
                        {
                            ply.SetAmmo(i, ushort.Parse(breaker[1]));
                        }
                        else
                        {
                            Log.Warn($"{item} probably doesn't exist as an ammoType...");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                        Log.Error(e.StackTrace);
                    }
                }
                ply.IsGodModeEnabled = false;
            });
            Log.Debug($"{ply.Nickname} switched to {at.Name} : {ast.Name}");
        }

        public static void ChangeAdvancedRole(this Player ply, AdvancedTeamSubclass ast, InventoryDestroyType ChangeInventory = InventoryDestroyType.None, bool ChangePosition = false)
        {
            if (!LeaderboardHelper.TeamsInstantiable) return;
            var av = new TeamEvents.SettingAdvancedTeamEventArgs(ply, UnitHelper.FindAT(ast.AdvancedTeam), ast);
            var t = av.AdvancedTeam;
            var ts = av.AdvancedSubTeam;
            if (!av.IsAllowed) return;
            ast = av.AdvancedSubTeam;
            RespawnHelper.Leaderboard.ClearPlayerFromLeaderBoards(ply);
            RespawnHelper.Leaderboard.GetTeamLeaderboard(UnitHelper.FindAT(ast.AdvancedTeam).Name).AddPlayer(ply, ast);
            Timing.CallDelayed(0.5f, () =>
            {
                if (ChangePosition && UnitHelper.FindAT(ast.AdvancedTeam).SpawnRoom != RoomType.Surface && !Warhead.IsInProgress && !Warhead.IsDetonated)
                    foreach (Room r in Room.List)
                    {
                        if (r.Type == UnitHelper.FindAT(ast.AdvancedTeam).SpawnRoom)
                        {
                            Vector3 vec = r.Doors.First().Position + Vector3.forward * 1.5f + Vector3.up * 1.5f;
                            r.Doors.First().IsOpen = true;
                            for (var i = 0; i < TeamPlugin.Singleton.Config.TeleportRetries && ply.Position != vec; i++)
                            {
                                ply.Teleport(vec);
                            }
                        }
                    }
                if (ChangeInventory == InventoryDestroyType.Destroy)
                {
                    ply.ClearInventory();
                    ply.Ammo.Clear();
                }
                else if (ChangeInventory == InventoryDestroyType.Drop)
                {
                    ply.DropItems();
                    ply.ClearInventory();
                    ply.Ammo.Clear();
                }
                ply.RoleManager.ServerSetRole(ast.Model, PlayerRoles.RoleChangeReason.Respawn, PlayerRoles.RoleSpawnFlags.None);
                ply.CustomInfo = $"{ply.Nickname}\n{ast.RoleDisplay}";
                ply.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
                ply.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
                ply.MaxHealth = ast.MaxHP;
                ply.Health = ast.HP;
                ply.ShowHint(ast.Hint.Replace("{Team}", $"<color={UnitHelper.FindAT(ast.AdvancedTeam).Color}>{UnitHelper.FindAT(ast.AdvancedTeam).Name}</color>").Replace("{Role}", $"<color={ast.Color}>{ast.RoleDisplay}</color>"), 10);
                foreach (string item in ast.Inventory)
                {
                    try
                    {
                        if (ItemConversionHelper.TryGetItemTypeFromString(item, out ItemType i))
                        {
                            ply.AddItem(i);
                        }
                        else if (ItemConversionHelper.TryGetCustomItemTypeFromString(item, out Exiled.CustomItems.API.Features.CustomItem ci))
                        {
                            ci.Give(ply, false);
                        }
                        else if (uint.TryParse(item, out uint _i) && ItemConversionHelper.TryGetCustomItemTypeFromInt(_i, out Exiled.CustomItems.API.Features.CustomItem _ci))
                        {
                            ci.Give(ply, false);
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
                foreach (string item in ast.AmmoInventory)
                {
                    string[] breaker = item.Split(':');
                    try
                    {
                        if (ItemConversionHelper.TryGetAmmoTypeFromString(breaker[0], out AmmoType i))
                        {
                            ply.SetAmmo(i, ushort.Parse(breaker[1]));
                        }
                        else
                        {
                            Log.Warn($"{item} probably doesn't exist as an ammoType...");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                        Log.Error(e.StackTrace);
                    }
                }
                ply.IsGodModeEnabled = false;
            });
            Log.Debug($"{ply.Nickname} switched to {UnitHelper.FindAT(ast.AdvancedTeam).Name} : {ast.Name}");
        }

        public static void ChangeAdvancedTeam(this Player ply, AdvancedTeam at)
        {
            RespawnHelper.Leaderboard.ClearPlayerFromLeaderBoards(ply);
            RespawnHelper.Leaderboard.GetTeamLeaderboard(at.Name).AddPlayer(ply);
            Log.Debug($"{ply.Nickname} switched to {at.Name} : {at.LastIndexSubclass.Name}");
        }
    }
}