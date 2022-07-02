using ATCBB.TeamAPI.Events;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using HarmonyLib;
using System;
using System.Reflection;
using Exiled.CustomRoles.API.Features;

namespace ATCBB
{
    public class TeamPlugin : Plugin<TeamConfig, Translations>
    {
        #region Public Fields

        public static Assembly assemblyTimer;
        public static string chaosTrans;
        public static string mtfTrans;
        public static TeamPlugin Singleton;
        public Harmony Harmony;
        public TeamEventHandler TeamEventHandler;

        #endregion Public Fields

        #region Public Properties

        public override string Author => "BoogaEye";
        public override string Name => "Advanced Team Creation";
        public override Version RequiredExiledVersion => new Version(5, 1, 3, 0);
        public override Version Version => new Version(1, 5, 1, 0);

        #endregion Public Properties

        #region Public Methods

        public void CheckPlugins()
        {
            foreach (IPlugin<IConfig> plugin in Exiled.Loader.Loader.Plugins)
            {
                if (plugin.Name == "RespawnTimer" && plugin.Config.IsEnabled)
                {
                    assemblyTimer = plugin.Assembly;
                    Log.Debug("RespawnTimer assembly found", this.Config.Debug);
                    StartRT();
                }
            }
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Map.Generated -= TeamEventHandler.MapGenerated;
            Exiled.Events.Handlers.Server.RespawningTeam -= TeamEventHandler.TeamSpawning;
            Exiled.Events.Handlers.Server.EndingRound -= TeamEventHandler.RoundEnding;
            Exiled.Events.Handlers.Server.RoundEnded -= TeamEventHandler.RoundEnd;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= TeamEventHandler.MtfRespawnCassie;
            Exiled.Events.Handlers.Player.Hurting -= TeamEventHandler.PlayerHurt;
            Exiled.Events.Handlers.Player.ChangingRole -= TeamEventHandler.RoleChange;
            Exiled.Events.Handlers.Player.Escaping -= TeamEventHandler.EscapingEvent;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination -= TeamEventHandler.ScpTermination;
            TeamEvents.ReferancingTeam -= TeamEventHandler.ReferancingTeam;
            Exiled.Events.Handlers.Player.Dying -= TeamEventHandler.PlayerDead;
            Exiled.Events.Handlers.Player.EnteringFemurBreaker -= TeamEventHandler.Scp106FemurBreakerPreventer;
            Exiled.Events.Handlers.Scp106.Containing -= TeamEventHandler.Scp106DistressHelper;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= TeamEventHandler.RagdollSpawn;
            Config.UnloadItems();
            CustomRole.UnregisterRoles();
            Harmony.UnpatchAll("BoogaEye.TeamStuff.Bruh");
            TeamEventHandler = null;
            Harmony = null;
            base.OnDisabled();
        }

        public override void OnEnabled()
        {
            Harmony = new Harmony("BoogaEye.TeamStuff.Bruh");
            Singleton = this;
            TeamEventHandler = new TeamEventHandler(this);
            Exiled.Events.Handlers.Map.Generated += TeamEventHandler.MapGenerated;
            Exiled.Events.Handlers.Server.RespawningTeam += TeamEventHandler.TeamSpawning;
            Exiled.Events.Handlers.Server.RoundEnded += TeamEventHandler.RoundEnd;
            Exiled.Events.Handlers.Server.EndingRound += TeamEventHandler.RoundEnding;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += TeamEventHandler.MtfRespawnCassie;
            Exiled.Events.Handlers.Player.ChangingRole += TeamEventHandler.RoleChange;
            Exiled.Events.Handlers.Player.Escaping += TeamEventHandler.EscapingEvent;
            Exiled.Events.Handlers.Player.Hurting += TeamEventHandler.PlayerHurt;
            Exiled.Events.Handlers.Player.Dying += TeamEventHandler.PlayerDead;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination += TeamEventHandler.ScpTermination;
            TeamEvents.ReferancingTeam += TeamEventHandler.ReferancingTeam;
            Exiled.Events.Handlers.Player.EnteringFemurBreaker += TeamEventHandler.Scp106FemurBreakerPreventer;
            Exiled.Events.Handlers.Scp106.Containing += TeamEventHandler.Scp106DistressHelper;
            Exiled.Events.Handlers.Player.SpawningRagdoll += TeamEventHandler.RagdollSpawn;
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

        public void StartRT()
        {
            MEC.Timing.CallDelayed(1, () =>
            {
                try
                {
                    Log.Debug($"Does translation Exist? => {RespawnTimer.RespawnTimer.Singleton.Translation}", Config.Debug);
                    mtfTrans = RespawnTimer.RespawnTimer.Singleton.Translation.Ntf;
                    chaosTrans = RespawnTimer.RespawnTimer.Singleton.Translation.Ci;
                    Log.Debug("Got respawn timer configs", Config.Debug);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    Log.Error(e.StackTrace);
                }
            });
        }

        #endregion Public Methods
    }
}