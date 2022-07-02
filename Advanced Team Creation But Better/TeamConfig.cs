using ATCBB.TeamAPI;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.CustomRoles.API.Features;
using Exiled.Loader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ATCBB
{
    public class TeamConfig : IConfig
    {
        #region Public Fields

        public List<AdvancedTeamSubclass> SubTeams = new List<AdvancedTeamSubclass>();
        public List<AdvancedTeam> Teams = new List<AdvancedTeam>();
        public List<CustomRole> DynamicRoles = new List<CustomRole>();

        #endregion Public Fields

        #region Public Properties

        public ATCBB.TeamAPI.CustomConfig.AtcItems AtcItems { get; set; } = new TeamAPI.CustomConfig.AtcItems();

        [Description("Determines if Class D Personal are friends with other Class D and Chaos")]
        public bool ClassDFriendsWithChaos { get; set; } = true;

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
        public bool VanillaTeamsHavePriority { get; set; } = true;

        #endregion Public Properties

        #region Public Methods

        public AdvancedTeamSubclass FindAST(string Team, string name)
        {
            foreach (AdvancedTeam at in Teams)
            {
                if (at.Name == Team)
                {
                    foreach (AdvancedTeamSubclass sb in SubTeams)
                    {
                        if (sb.Name == name && sb.AdvancedTeam == at.Name)
                        {
                            return sb;
                        }
                    }
                }
            }
            return null;
        }

        public AdvancedTeam FindAT(string name)
        {
            foreach (AdvancedTeam at in Teams)
            {
                if (at.Name == name)
                {
                    return at;
                }
            }
            return null;
        }

        public void LoadItems()
        {
            Log.Info("Registering custom items");
            Exiled.CustomItems.API.Features.CustomItem.RegisterItems(overrideClass: AtcItems);
        }

        public void LoadTeamConfigs()
        {
            Teams.Clear();
            SubTeams.Clear();
            if (!Directory.Exists(ConfigsFolder))
            {
                Directory.CreateDirectory(ConfigsFolder);
            }
            foreach (Team t in Enum.GetValues(typeof(Team)).Cast<Team>())
            {
                var at = OriginalToCustomTeamHelper.SetUpAfterOriginalTeam(t);
                Teams.Add(at);
                Log.Debug($"Referancing Team from vanilla game called {t} creating {at.Name} with Displayer of {at.DisplayName}", Debug);
            }
            foreach (string at in Directory.EnumerateDirectories(ConfigsFolder))
            {
                var gh = Loader.Deserializer.Deserialize<AdvancedTeam>(File.ReadAllText(Path.Combine(at, "TeamConfig.yml")));
                Teams.Add(gh);
                Log.Debug($"Deserializing Team {at}", Debug);
                foreach (string ast in Directory.EnumerateFiles(at))
                {
                    if (!ast.Contains("TeamConfig.yml"))
                    {
                        var g = Loader.Deserializer.Deserialize<AdvancedTeamSubclass>(File.ReadAllText(ast));
                        g.AdvancedTeam = gh.Name;
                        SubTeams.Add(g);
                        Log.Debug($"Deserializing SubTeam {ast}", Debug);
                    }
                }
            }
        }

        public void UnloadItems()
        {
            Exiled.CustomItems.API.Features.CustomItem.UnregisterItems();
        }

        #endregion Public Methods
    }
}