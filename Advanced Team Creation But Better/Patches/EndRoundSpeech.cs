using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATCBB.TeamAPI.CustomEventHelpers;
using HarmonyLib;

namespace ATCBB.Patches
{
    [HarmonyPatch(typeof(Radio), nameof(Radio.UserCode_CmdSyncTransmissionStatus))]
    public class EndRoundSpeech
    {
        [HarmonyPrefix]
        public static bool Prefix(Radio __instance, bool b)
        {
            Exiled.API.Features.Player ply = Exiled.API.Features.Player.Get(__instance._hub);
            if (string.IsNullOrEmpty(ply?.UserId)) return false;
            if (CustomRoundEnder.RoundEnded) return __instance._dissonanceSetup.SpectatorChat = b;
            if (ply.HasItem(ItemType.Radio)) return __instance._dissonanceSetup.RadioAsHuman = b;
            if (ply.Role.Type == RoleType.Scp93953 || ply.Role.Type == RoleType.Scp93989 || ply.Role.Type == RoleType.Scp049) return __instance._dissonanceSetup.MimicAs939 = b;
            return false;
        }
    }
}
