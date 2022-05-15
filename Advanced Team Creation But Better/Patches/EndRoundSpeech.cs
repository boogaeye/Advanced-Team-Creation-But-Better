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
        public static bool Prefix(Radio __instance, bool bruh)
        {
            Exiled.API.Features.Player ply = Exiled.API.Features.Player.Get(__instance._hub);
            if (string.IsNullOrEmpty(ply?.UserId)) return false;
            if (CustomRoundEnder.RoundEnded) return __instance._dissonanceSetup.SpectatorChat = bruh;
            if (ply.HasItem(ItemType.Radio)) return __instance._dissonanceSetup.RadioAsHuman = bruh;
            return false;
        }
    }
}
