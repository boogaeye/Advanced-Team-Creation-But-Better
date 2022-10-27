using AdvancedTeamCreation.TeamAPI;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.CustomRoles.API.Features;
using Exiled.Loader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using AdvancedTeamCreation.TeamAPI.Helpers;
using AdvancedTeamCreation.TeamAPI.CustomConfig;

namespace AdvancedTeamCreation
{
    public class TeamConfig : IConfig
    {
        public AtcItems AtcItems { get; set; } = new TeamAPI.CustomConfig.AtcItems();

        [Description("Determines if Class D Personal are friends with other Class D and Chaos")]
        public bool ClassDFriendsWithChaos { get; set; } = true;

        [Description("Defines if SCP Friendly teams can hurt eachother even with Friendly Fire on")]
        public bool SCPTeamsCantHurtEachotherWithFF { get; set; } = true;

        public bool IgnoresRoundLock { get; set; } = false;
        public string ConfigsFolder { get; set; } = Path.Combine(Paths.Configs, "AdvancedTeamCreation");

        [Description("Recommended to keep this on this provides a custom handler to custom teams so they can win or you can switch it off to turn it back to the default round ender")]
        public bool CustomRoundEnder { get; set; } = true;

        public bool Debug { get; set; } = false;
        public bool IsEnabled { get; set; } = true;
        public bool ScpNeutralWithChaos { get; set; } = false;

        [Description("Shows only friendly teams after time is up set to -1 for it to not disappear")]
        public int ShowEnemyTeamsForTime { get; set; } = 10;

        public bool ShowFriendlyHint { get; set; } = true;
        public bool ShowHurtFriendlyHint { get; set; } = true;

        [Description("Shows who are Hostile, Required, Friendly, and Neutral")]
        public bool ShowTeamsList { get; set; } = true;

        [Description("Shows only alive friendlies and requirements instead of showing all friendlies    reduces list size")]
        public bool ShowTeamInListIfAliveOnly { get; set; } = true;

        [Description("Makes the team list only appear when announcements go off")]
        public bool TeamsListPromptsAtAnnouncement { get; set; } = false;

        [Description("Only spawn a team if they have an enemy in a facility")]
        public bool TeamSpawnsOnlyIfEnemiesExist { get; set; } = true;

        public int TeleportRetries { get; set; } = 15;
        public bool UseCustomItemsFromATC { get; set; } = true;

        [Description("If Vanilla teams get the first chance to spawn instead of having the last chances to spawn")]
        public bool VanillaTeamsHavePriority { get; set; } = false;

        public void LoadItems()
        {
            Log.Info("Registering custom items");
            Exiled.CustomItems.API.Features.CustomItem.RegisterItems(overrideClass: AtcItems);
        }

        public void LoadTeamConfigs()
        {
            UnitHelper.UnregisterTeams();
            if (!Directory.Exists(ConfigsFolder))
            {
                Directory.CreateDirectory(ConfigsFolder);
            }
            foreach (Team t in Enum.GetValues(typeof(Team)).Cast<Team>())
            {
                UnitHelper.RegisterTeam(t);
            }
            foreach (string at in Directory.EnumerateDirectories(ConfigsFolder))
            {
                var gh = Loader.Deserializer.Deserialize<AdvancedTeam>(File.ReadAllText(Path.Combine(at, "TeamConfig.yml")));
                gh.Name = Path.GetFileName(at);
                Log.Debug($"Deserializing Team {at} to Team {gh.Name}", Debug);
                List<AdvancedTeamSubclass> SubteamMan = new List<AdvancedTeamSubclass>();
                foreach (string ast in Directory.EnumerateFiles(at))
                {
                    if (!ast.Contains("TeamConfig.yml") && ast.Contains(".yml"))
                    {
                        var g = Loader.Deserializer.Deserialize<AdvancedTeamSubclass>(File.ReadAllText(ast));
                        g.AdvancedTeam = gh.Name;
                        g.Name = Path.GetFileNameWithoutExtension(ast);
                        if (gh.SpawnOrder.Last().Split(':')[0] == g.Name)
                        {
                            gh.LastIndexSubclass = g;
                            Log.Debug($"{g.Name} is last spawn order", Debug);
                        }
                        if (g.CustomKeycardConfig.RegisterKeycard)
                            g.RegisterCustomKeycard();
                        SubteamMan.Add(g);
                        Log.Debug($"Deserializing SubTeam {ast} to Subclass {g.Name}", Debug);
                    }
                }
                UnitHelper.RegisterTeam(gh, SubteamMan.ToArray());
            }
        }

        public void UnloadItems()
        {
            Exiled.CustomItems.API.Features.CustomItem.UnregisterItems();
        }
    }
}