using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled;

namespace ATCBB.TeamAPI
{
    public class OriginalToCustomTeamHelper
    {
        public static AdvancedTeam SetUpAfterOriginalTeam(Team t)
        {
            Log.Info($"Converting Type {t}");
            AdvancedTeam at = new AdvancedTeam
            {
                Name = t.ToString(),
                VanillaTeam = true,
                Chance = 1
            };
            Log.Info($"Made Vanilla Team {t} looking snazzy {at.Name}");
            switch (t)
            {
                case Team.CDP:
                    at.Color = "orange";
                    at.DisplayName = "<color=orange>Class D Personal Escapee's</color>";
                    at.RoundEnderConfig.FriendlyTeams = new List<string>();
                    at.RoundEnderConfig.RequiredTeams = new List<string>()
                    {
                        "RSC",
                        "SCP",
                        "MTF"
                    };
                    if (TeamPlugin.Singleton.Config.ClassDFriendsWithChaos)
                    {
                        at.RoundEnderConfig.FriendlyTeams.Add("CHI");
                        at.RoundEnderConfig.FriendlyTeams.Add("CDP");
                    }
                    break;
                case Team.RSC:
                    at.DisplayName = "<color=yellow>Research Personal Escapee's</color>";
                    at.Color = "yellow";
                    at.RoundEnderConfig.FriendlyTeams = new List<string>()
                    {
                        "RSC",
                        "MTF"
                    };
                    at.RoundEnderConfig.RequiredTeams = new List<string>()
                    {
                        "CDP",
                        "SCP",
                        "MTF",
                        "CHI"
                    };
                    break;
                case Team.SCP:
                    at.DisplayName = "<color=red>SCP's</color>";
                    at.Color = "red";
                    at.RoundEnderConfig.FriendlyTeams = new List<string>()
                    {
                        "SCP"
                    };
                    at.RoundEnderConfig.RequiredTeams = new List<string>()
                    {
                        "CDP",
                        "MTF",
                        "RSC"
                    };
                    if (!TeamPlugin.Singleton.Config.ScpNeutralWithChaos)
                        at.RoundEnderConfig.RequiredTeams.Add("CHI");
                    break;
                case Team.CHI:
                    at.DisplayName = "<color=green>The Insurgency</color>";
                    at.Color = "green";
                    at.RoundEnderConfig.FriendlyTeams = new List<string>()
                    {
                        "CHI"
                    };
                    if (TeamPlugin.Singleton.Config.ClassDFriendsWithChaos)
                    {
                        at.RoundEnderConfig.FriendlyTeams.Add("CDP");
                    }
                    at.RoundEnderConfig.RequiredTeams = new List<string>()
                    {
                        "MTF",
                        "RSC"
                    };
                    if (TeamPlugin.Singleton.Config.ClassDFriendsWithChaos)
                    {
                        at.RoundEnderConfig.RequiredTeams.Add("CDP");
                    }
                    if (!TeamPlugin.Singleton.Config.ScpNeutralWithChaos)
                        at.RoundEnderConfig.RequiredTeams.Add("SCP");
                    break;
                case Team.MTF:
                    at.DisplayName = "<color=blue>Mobile Task Force</color> and <color=grey>Facility Forces</color>";
                    at.Color = "blue";
                    at.RoundEnderConfig.FriendlyTeams = new List<string>()
                    {
                        "MTF",
                        "RSC"
                    };
                    at.RoundEnderConfig.RequiredTeams = new List<string>()
                    {
                        "CDP",
                        "SCP",
                        "CHI",
                        "RSC"
                    };
                    break;
                default:
                    at.RoundEnderConfig.FriendlyTeams = new List<string>();
                    at.RoundEnderConfig.RequiredTeams = new List<string>();
                    at.Spectator = true;
                    break;
            }
            Log.Info($"Returning Vanilla Team {t}");
            return at;
        }
    }
}
