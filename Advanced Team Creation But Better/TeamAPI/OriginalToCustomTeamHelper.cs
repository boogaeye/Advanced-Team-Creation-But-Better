using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled;
using Exiled.API.Extensions;

namespace ATCBB.TeamAPI
{
    public class OriginalToCustomTeamHelper
    {
        public static bool GetRolesFromTeam(Team t, out RoleType[] bruh)
        {
            List<RoleType> T = new List<RoleType>();
            foreach (RoleType rt in Enum.GetValues(typeof(RoleType)).Cast<RoleType>())
            {
                if (rt.GetTeam() == t)
                {
                    T.Add(rt);
                }
            }
            bruh = T.ToArray();
            return true;
        }

        public static bool GetRolesFromTeam(string t, out RoleType[] bruh)
        {
            Team Selection = Team.RIP;
            foreach (Team ts in Enum.GetValues(typeof(Team)).Cast<Team>())
            {
                if (ts.ToString() == t)
                {
                    Selection = ts;
                    break;
                }
            }
            if (Selection == Team.RIP)
            {
                bruh = null;
                return false;
            }
            List<RoleType> T = new List<RoleType>();
            foreach (RoleType rt in Enum.GetValues(typeof(RoleType)).Cast<RoleType>())
            {
                if (rt.GetTeam() == Selection)
                {
                    T.Add(rt);
                }
            }
            bruh = T.ToArray();
            return true;
        }

        public static AdvancedTeam SetUpAfterOriginalTeam(Team t)
        {
            bool d = TeamPlugin.Singleton.Config.Debug;
            Log.Debug($"Converting Type {t}", d);
            AdvancedTeam at = new AdvancedTeam
            {
                Name = t.ToString(),
                VanillaTeam = true,
                Chance = 1
            };
            if (TeamPlugin.Singleton.Translation.TeamCassieSlaughter.ContainsKey(t))
            {
                at.CassieSlaughtered = TeamPlugin.Singleton.Translation.TeamCassieSlaughter[t];
                at.CassieSlaughteredSubtitles = TeamPlugin.Singleton.Translation.TeamCassieSlaughterSubtitles[t];
            }
            else
            {
                at.CassieSlaughtered = String.Empty;
            }
            Log.Debug($"Made Vanilla Team {t} looking snazzy {at.Name}", d);
            switch (t)
            {
                case Team.CDP:
                    at.Color = "orange";
                    at.DisplayName = "<color=orange>Class D Personal Escapee's</color>";
                    at.SaidName = "Class D";
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
                    at.SaidName = "Scientist";
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
                    at.SaidName = "SCPSubjects";
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
                    at.SaidName = "Chaos Insurgency";
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
                    at.SaidName = "MTFUnit";
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
                    at.SaidName = "Unspecified";
                    break;
            }
            Log.Debug($"Returning Vanilla Team {t}", d);
            return at;
        }
    }
}