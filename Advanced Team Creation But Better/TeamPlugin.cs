using AdvancedTeamCreation.TeamAPI.Events;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using HarmonyLib;
using System;
using System.Reflection;
using Exiled.CustomRoles.API.Features;
using Exiled.API.Enums;
using AdvancedTeamCreation.TeamAPI;
using AdvancedTeamCreation.TeamAPI.Helpers;

namespace AdvancedTeamCreation
{
    public class TeamPlugin : Plugin<TeamConfig, Translations>
    {
        public static Assembly assemblyTimer;
        public static Assembly assemblyAudioPlayer;
        public static Assembly assemblyTkInfo;
        public static Assembly assemblyRemoteKeycard;
        public static TeamPlugin Singleton;
        public Harmony Harmony;
        public EventHandler EventHandler;
        public TeamEventHandler TeamEventHandler;

        public override string Author => "SCP Fazbear"; //Adopting my current username
        public override string Name => "Advanced Team Creation";
        public override string Prefix => "ATC";
        public override Version RequiredExiledVersion => new Version(5, 3, 0, 0);
        public override Version Version => new Version(2, 0, 0, 1);
        public override PluginPriority Priority => PluginPriority.Last;

        public void CheckPlugins()
        {
            foreach (IPlugin<IConfig> plugin in Exiled.Loader.Loader.Plugins)
            {
                if (plugin.Name == "RespawnTimer" && plugin.Config.IsEnabled)
                {
                    assemblyTimer = plugin.Assembly;
                    Log.Debug("RespawnTimer assembly found", this.Config.Debug);
                }
                //else if (plugin.Name == "RemoteKeycard" && plugin.Config.IsEnabled)
                //{
                //    assemblyRemoteKeycard = plugin.Assembly;
                //    RemoteKeyEvents();
                //    Log.Debug("RemoteKeycard assembly found", this.Config.Debug);
                //}
                else if (plugin.Name == "AudioPlayer" && plugin.Config.IsEnabled)
                {
                    assemblyAudioPlayer = plugin.Assembly;
                    Log.Debug("AudioPlayer assembly found", this.Config.Debug);
                }
            }
        }

        public void RemoteKeyEvents()
        {
            RemoteKeycard.Handlers.Events.UsingKeycard += EventHandler.UsingKeycard;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Map.Generated -= EventHandler.MapGenerated;
            Exiled.Events.Handlers.Server.RespawningTeam -= EventHandler.TeamSpawning;
            Exiled.Events.Handlers.Server.EndingRound -= EventHandler.RoundEnding;
            Exiled.Events.Handlers.Server.RoundEnded -= EventHandler.RoundEnd;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= EventHandler.MtfRespawnCassie;
            Exiled.Events.Handlers.Player.Hurting -= EventHandler.PlayerHurt;
            Exiled.Events.Handlers.Player.ChangingRole -= EventHandler.RoleChange;
            Exiled.Events.Handlers.Player.Escaping -= EventHandler.EscapingEvent;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination -= EventHandler.ScpTermination;
            TeamEvents.ReferancingTeam -= TeamEventHandler.ReferancingTeam;
            Exiled.Events.Handlers.Player.Dying -= EventHandler.PlayerDead;
            Exiled.Events.Handlers.Player.EnteringFemurBreaker -= EventHandler.Scp106FemurBreakerPreventer;
            Exiled.Events.Handlers.Scp106.Containing -= EventHandler.Scp106DistressHelper;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= EventHandler.RagdollSpawn;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= EventHandler.Scp106NoTeamKill;
            //if (assemblyRemoteKeycard != null)
            //    RemoteKeycard.Handlers.Events.UsingKeycard -= EventHandler.UsingKeycard;
            Config.UnloadItems();
            foreach (AdvancedTeamSubclass ats in UnitHelper.Subteams)
            {
                if (ats.CustomKeycardConfig.RegisterKeycard)
                    ats.UnregisterCustomKeycard();
            }
            Harmony.UnpatchAll("BoogaEye.TeamStuff.Bruh");
            EventHandler = null;
            TeamEventHandler = null;
            Harmony = null;
            base.OnDisabled();
        }

        public override void OnEnabled()
        {
            Harmony = new Harmony("BoogaEye.TeamStuff.Bruh");
            Singleton = this;
            EventHandler = new EventHandler(this);
            TeamEventHandler = new TeamEventHandler(this);
            Exiled.Events.Handlers.Map.Generated += EventHandler.MapGenerated;
            Exiled.Events.Handlers.Server.RespawningTeam += EventHandler.TeamSpawning;
            Exiled.Events.Handlers.Server.RoundEnded += EventHandler.RoundEnd;
            Exiled.Events.Handlers.Server.EndingRound += EventHandler.RoundEnding;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += EventHandler.MtfRespawnCassie;
            Exiled.Events.Handlers.Player.ChangingRole += EventHandler.RoleChange;
            Exiled.Events.Handlers.Player.Escaping += EventHandler.EscapingEvent;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += EventHandler.Scp106NoTeamKill;
            Exiled.Events.Handlers.Player.Hurting += EventHandler.PlayerHurt;
            Exiled.Events.Handlers.Player.Dying += EventHandler.PlayerDead;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination += EventHandler.ScpTermination;
            TeamEvents.ReferancingTeam += TeamEventHandler.ReferancingTeam;
            Exiled.Events.Handlers.Player.EnteringFemurBreaker += EventHandler.Scp106FemurBreakerPreventer;
            Exiled.Events.Handlers.Scp106.Containing += EventHandler.Scp106DistressHelper;
            Exiled.Events.Handlers.Player.SpawningRagdoll += EventHandler.RagdollSpawn;
            if (Config.UseCustomItemsFromATC)
            {
                Config.LoadItems();
            }
            Harmony.PatchAll();
            CheckPlugins();
            base.OnEnabled();
        }

        public override void OnReloaded()
        {
            Config.LoadTeamConfigs();
            base.OnReloaded();
        }
    }
}