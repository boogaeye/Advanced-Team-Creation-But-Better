using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using Exiled.API;
using System.IO;
using Exiled.Loader;
using Exiled.API.Features;
using Exiled.API.Extensions;
using ATCBB.TeamAPI;

namespace ATCBB
{
    public class TeamConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public bool FriendlyFire { get; set; } = true;
        [Description("Determines if Class D Personal are friends with other Class D and Chaos")]
        public bool ClassDFriendsWithChaos { get; set; } = true;
        public bool ScpNeutralWithChaos { get; set; } = false;
        public int TeleportRetries { get; set; } = 15;
        [Description("Recommended to keep this on this provides a custom handler to custom teams so they can win or you can switch it off to turn it back to the default round ender")]
        public bool CustomRoundEnder { get; set; } = true;
        public string ConfigsFolder { get; set; } = Path.Combine(Paths.Configs, "AdvancedTeamCreation");
        public List<AdvancedTeam> Teams = new List<AdvancedTeam>();
        public List<AdvancedTeamSubclass> SubTeams = new List<AdvancedTeamSubclass>();

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
                Teams.Add(OriginalToCustomTeamHelper.SetUpAfterOriginalTeam(t));
                Log.Debug($"Referancing Team from vanilla game called {t}", Debug);
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
    }
}