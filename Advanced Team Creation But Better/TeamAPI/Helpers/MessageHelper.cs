using System;
using System.Linq;
using Exiled.API.Features;

namespace AdvancedTeamCreation.TeamAPI.Helpers
{
    public class MessageHelper
    {
        public static int ScpsLeft => Player.List.Where(e => e.IsScp && e.Role.Type != RoleType.Scp0492).Count();

        public static string[] Natos = new string[26]
        {
            "Alpha",
            "Bravo",
            "Charlie",
            "Delta",
            "Echo",
            "Foxtrot",
            "Golf",
            "Hotel",
            "India",
            "Juliett",
            "Kilo",
            "Lima",
            "Mike",
            "November",
            "Oscar",
            "Papa",
            "Quebec",
            "Romeo",
            "Sierra",
            "Tango",
            "Uniform",
            "Victor",
            "Whiskey",
            "X-ray",
            "Yankee",
            "Zulu"
        };

        public static int HiddenInterference { get; private set; }
        public static int Spawns { get; private set; }
        private static bool PlayedAlready = false;
        private static Random rand = new Random();

        public static void CustomTeamScpTermination(string scpName, AdvancedTeam at)
        {
            if (at.VanillaTeam) return;
            string text = scpName;
            text = "scp " + text + " CONTAINEDSUCCESSFULLY. containment unit " + at.SaidName;
            float num = ((AlphaWarheadController.Host.timeToDetonation <= 0f) ? 3.5f : 1f);
            Cassie.GlitchyMessage(text, UnityEngine.Random.Range(0.1f, 0.14f) * num, UnityEngine.Random.Range(0.07f, 0.08f) * num);
        }

        public static void SlaughteredTeam(AdvancedTeam at, AdvancedTeam TerminatingTeam)
        {
            Cassie.MessageTranslated(at.CassieSlaughtered.Replace("{Terminating}", TerminatingTeam.SaidName).Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Terminated}", at.SaidName), at.CassieSlaughteredSubtitles.Replace("{Terminating}", $"<color={TerminatingTeam.Color}>{TerminatingTeam.Name}</color>").Replace("{Terminated}", $"<color={at.Color}>{at.Name}</color>").Replace("{SCPLeft}", ScpsLeft.ToString()));
        }

        public static void PlayTeamAnnouncement()
        {
            if (RespawnHelper.ReferancedTeam.CassieAnnouncement != String.Empty)
            {
                Log.Debug("Announcement is empty...", TeamPlugin.Singleton.Config.Debug);
                return;
            }
            if (PlayedAlready)
            {
                Log.Debug("Announcement has not been reset. Stopping announcement...", TeamPlugin.Singleton.Config.Debug);
                return;
            }
            Log.Debug("Attempting to Play Team Announcement!", TeamPlugin.Singleton.Config.Debug);
            int UnitNumPre = new Random().Next(1, 19);
            string UnitNamePre = Natos[new Random().Next(0, 26)];
            if (HiddenInterference < RespawnHelper.ReferancedTeam.ChanceForHiddenMtfNato)
            {
                if (TeamPlugin.Singleton.Config.TeamsListPromptsAtAnnouncement)
                    HudHelper.ShowAllPlayersTeamDisplay(10);
                Cassie.MessageTranslated(RespawnHelper.ReferancedTeam.CassieAnnouncement.Replace("{SCPLeft}", MessageHelper.ScpsLeft.ToString()).Replace("{Unit}", $"NATO_{UnitNamePre[0]}").Replace("{UnitNum}", UnitNumPre.ToString()), RespawnHelper.ReferancedTeam.CassieAnnouncementSubtitles.Replace("{SCPLeft}", MessageHelper.ScpsLeft.ToString()).Replace("{Unit}", $"{UnitNamePre}").Replace("{UnitNum}", UnitNumPre.ToString()));
            }

            PlayedAlready = true;
        }

        public static void PlayTeamAnnouncement(AdvancedTeam at)
        {
            if (at.CassieAnnouncement == String.Empty)
            {
                Log.Debug("Announcement is empty...", TeamPlugin.Singleton.Config.Debug);
                return;
            }
            if (PlayedAlready)
            {
                Log.Debug("Announcement has not been reset. Stopping announcement...", TeamPlugin.Singleton.Config.Debug);
                return;
            }
            Log.Debug("Attempting to Play Team Announcement!", TeamPlugin.Singleton.Config.Debug);
            int UnitNumPre = new Random().Next(1, 19);
            string UnitNamePre = Natos[new Random().Next(0, 26)];
            if (HiddenInterference < at.ChanceForHiddenMtfNato)
            {
                if (TeamPlugin.Singleton.Config.TeamsListPromptsAtAnnouncement)
                    HudHelper.ShowAllPlayersTeamDisplay(10);
                Cassie.MessageTranslated(at.CassieAnnouncement.Replace("{SCPLeft}", MessageHelper.ScpsLeft.ToString()).Replace("{Unit}", $"NATO_{UnitNamePre[0]}").Replace("{UnitNum}", UnitNumPre.ToString()), at.CassieAnnouncementSubtitles.Replace("{SCPLeft}", MessageHelper.ScpsLeft.ToString()).Replace("{Unit}", $"{UnitNamePre}").Replace("{UnitNum}", UnitNumPre.ToString()));
            }

            PlayedAlready = true;
        }

        public static void PlayTeamAnnouncement(AdvancedTeam at, string Nato, int Number)
        {
            if (at.CassieAnnouncement == String.Empty)
            {
                Log.Debug("Announcement is empty...", TeamPlugin.Singleton.Config.Debug);
                return;
            }
            if (PlayedAlready)
            {
                Log.Debug("Announcement has not been reset. Stopping announcement...", TeamPlugin.Singleton.Config.Debug);
                return;
            }
            Log.Debug("Attempting to Play Team Announcement!", TeamPlugin.Singleton.Config.Debug);
            if (HiddenInterference < at.ChanceForHiddenMtfNato)
            {
                if (TeamPlugin.Singleton.Config.TeamsListPromptsAtAnnouncement)
                    HudHelper.ShowAllPlayersTeamDisplay(10);
                Cassie.MessageTranslated(at.CassieAnnouncement.Replace("{SCPLeft}", MessageHelper.ScpsLeft.ToString()).Replace("{Unit}", $"Nato_{Nato[0]}").Replace("{UnitNum}", Number.ToString()), at.CassieAnnouncementSubtitles.Replace("{SCPLeft}", MessageHelper.ScpsLeft.ToString()).Replace("{Unit}", Nato).Replace("{UnitNum}", Number.ToString()));
            }

            PlayedAlready = true;
        }

        public static void ResetTeamAnnouncement()
        {
            PlayedAlready = false;
            Log.Debug("Announcement Reset!", TeamPlugin.Singleton.Config.Debug);
        }

        public static void RollForHiddenSpawn()
        {
            HiddenInterference = rand.Next(0, 99);
            Log.Debug($"Rolled a {HiddenInterference} for Interferance", TeamPlugin.Singleton.Config.Debug);
        }

        public static void ResetSpawns()
        {
            Spawns = 0;
            Log.Debug("Reset Spawn Index!", TeamPlugin.Singleton.Config.Debug);
        }

        public static void IncrementSpawns()
        {
            Spawns++;
            Log.Debug($"Incremented Spawn Index to {Spawns}", TeamPlugin.Singleton.Config.Debug);
        }
    }
}