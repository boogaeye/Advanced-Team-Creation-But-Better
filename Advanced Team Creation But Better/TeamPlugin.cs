﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.API;
using Exiled.Events.Handlers;
using HarmonyLib;
using ATCBB.TeamAPI.Events;
using System.Reflection;

namespace ATCBB
{
    public class TeamPlugin : Plugin<TeamConfig, Translations>
    {
        public static TeamPlugin Singleton;
        public TeamEventHandler TeamEventHandler;
        public Harmony Harmony;

        public static Assembly assemblyTimer;
        public override void OnEnabled()
        {
            Harmony = new Harmony("BoogaEye.TeamStuff.Bruh");
            Singleton = this;
            TeamEventHandler = new TeamEventHandler(this);
            Exiled.Events.Handlers.Map.Generated += TeamEventHandler.MapGenerated;
            Exiled.Events.Handlers.Server.RespawningTeam += TeamEventHandler.TeamSpawning;
            Exiled.Events.Handlers.Server.RoundEnded += TeamEventHandler.RoundEnd;
            Exiled.Events.Handlers.Player.ChangingRole += TeamEventHandler.RoleChange;
            TeamEvents.ReferancingTeam += TeamEventHandler.ReferancingTeam;
            if (!Exiled.API.Features.Server.FriendlyFire)
            {
                Log.Warn("Friendly Fire Is heavily recommended to be enabled on server config as it can lead to problems with people not being able to finish around because a person is supposed to be their enemy");
                Exiled.API.Features.Server.FriendlyFire = true;
                Log.Warn("Friendly Fire is now enabled automatically (if you want friendly fire disabled please enable the corrisponding config)");
            }
            Harmony.PatchAll();
            CheckPlugins();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Map.Generated -= TeamEventHandler.MapGenerated;
            Exiled.Events.Handlers.Player.ChangingRole -= TeamEventHandler.RoleChange;
            Exiled.Events.Handlers.Server.RespawningTeam -= TeamEventHandler.TeamSpawning;
            TeamEvents.ReferancingTeam -= TeamEventHandler.ReferancingTeam;
            Exiled.Events.Handlers.Server.RoundEnded -= TeamEventHandler.RoundEnd;
            Harmony.UnpatchAll("BoogaEye.TeamStuff.Bruh");
            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            Config.LoadTeamConfigs();
            base.OnReloaded();
        }

        public void CheckPlugins()
        {
            foreach (IPlugin<IConfig> plugin in Exiled.Loader.Loader.Plugins)
            {
                if (plugin.Name == "RespawnTimer" && plugin.Config.IsEnabled)
                {
                    assemblyTimer = plugin.Assembly;
                    StartRT();
                    Log.Debug("RespawnTimer assembly found", this.Config.Debug);
                }
            }
        }

        public static string mtfTrans;
        public static string chaosTrans;

        public void StartRT()
        {
            var cfg = RespawnTimer.RespawnTimer.Singleton.Translation;
            Log.Debug("Got respawn timer configs", Config.Debug);
            mtfTrans = cfg.Ntf;
            chaosTrans = cfg.Ci;
        }
    }
}