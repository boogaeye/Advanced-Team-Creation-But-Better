﻿using System;
using System.IO;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;

namespace AdvancedTeamCreation.TeamAPI.Helpers
{
    public class MessageHelper
    {
        public static int ScpsLeft => Player.List.Where(e => e.IsScp && e.Role.Type != RoleTypeId.Scp0492).Count();

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
            float num = ((AlphaWarheadController.TimeUntilDetonation <= 0f) ? 3.5f : 1f);
            Cassie.GlitchyMessage(text, UnityEngine.Random.Range(0.1f, 0.14f) * num, UnityEngine.Random.Range(0.07f, 0.08f) * num);
        }

        public static void SlaughteredTeam(AdvancedTeam at, AdvancedTeam TerminatingTeam)
        {
            if (at.CassieSlaughtered == null) return;
            if (TerminatingTeam.CassieSlaughtered == null) return;
            Cassie.MessageTranslated(at.CassieSlaughtered.Replace("{Terminating}", TerminatingTeam.SaidName).Replace("{SCPLeft}", ScpsLeft.ToString()).Replace("{Terminated}", at.SaidName), at.CassieSlaughteredSubtitles.Replace("{Terminating}", $"<color={TerminatingTeam.Color}>{TerminatingTeam.Name}</color>").Replace("{Terminated}", $"<color={at.Color}>{at.Name}</color>").Replace("{SCPLeft}", ScpsLeft.ToString()));
        }

        public static void PlayTeamAnnouncement(AdvancedTeam at)
        {
            Log.Debug("Attempting to Play Team Random Nato Announcement!");
            int UnitNumPre = new Random().Next(1, 19);
            string UnitNamePre = Natos[new Random().Next(0, 26)];
            PlayTeamAnnouncement(at, UnitNamePre, UnitNumPre);
        }

        public static void PlayTeamAnnouncement(AdvancedTeam at, string Nato, int Number)
        {
            if (at.CassieAnnouncement == String.Empty)
            {
                Log.Debug("Announcement is empty...");
                return;
            }
            if (PlayedAlready)
            {
                Log.Debug("Announcement has not been reset. Stopping announcement...");
                return;
            }
            Log.Debug("Attempting to Play Team Custom Nato Announcement!");
            if (HiddenInterference < at.ChanceForHiddenMtfNato)
            {
                if (TeamPlugin.Singleton.Config.TeamsListPromptsAtAnnouncement)
                    HudHelper.ShowAllPlayersTeamDisplay(10);
                Cassie.MessageTranslated(at.CassieAnnouncement.Replace("{SCPLeft}", MessageHelper.ScpsLeft.ToString()).Replace("{Unit}", $"Nato_{Nato[0]}").Replace("{UnitNum}", Number.ToString()), at.CassieAnnouncementSubtitles.Replace("{SCPLeft}", MessageHelper.ScpsLeft.ToString()).Replace("{Unit}", Nato).Replace("{UnitNum}", Number.ToString()));
                //TODO IS NOT SUPPORTED
                //if (TeamPlugin.assemblyAudioPlayer != null)
                //{
                //    PlayAnnounceAudio(at);
                //}
            }
            PlayedAlready = true;
        }

        //public static void PlayAnnounceAudio(AdvancedTeam at)
        //{
        //    var path = Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, at.Name, "theme.mp3");
        //    if (File.Exists(path))
        //        AudioPlayer.API.AudioController.PlayFromFile(Path.Combine(TeamPlugin.Singleton.Config.ConfigsFolder, at.Name, "theme.mp3"), 50);
        //}

        public static void ResetTeamAnnouncement()
        {
            PlayedAlready = false;
            Log.Debug("Announcement Reset!");
        }

        public static void RollForHiddenSpawn()
        {
            HiddenInterference = rand.Next(0, 99);
            Log.Debug($"Rolled a {HiddenInterference} for Interferance");
        }

        public static void ResetSpawns()
        {
            Spawns = 0;
            Log.Debug("Reset Spawn Index!");
        }

        public static void IncrementSpawns()
        {
            Spawns++;
            Log.Debug($"Incremented Spawn Index to {Spawns}");
        }
    }
}