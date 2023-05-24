using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Extensions;
using PlayerRoles;

namespace AdvancedTeamCreation.TeamAPI.Helpers
{
    public class OriginalToCustomTeamHelper
    {
        public static bool GetRolesFromTeam(Team t, out RoleTypeId[] bruh)
        {
            List<RoleTypeId> T = new List<RoleTypeId>();
            foreach (RoleTypeId rt in Enum.GetValues(typeof(RoleTypeId)).Cast<RoleTypeId>())
            {
                if (RoleExtensions.GetTeam(rt) == t)
                {
                    T.Add(rt);
                }
            }
            bruh = T.ToArray();
            return true;
        }

        public static bool GetRolesFromTeam(string t, out RoleTypeId[] bruh)
        {
            Team Selection = Team.Dead;
            foreach (Team ts in Enum.GetValues(typeof(Team)).Cast<Team>())
            {
                if (ts.ToString() == t)
                {
                    Selection = ts;
                    break;
                }
            }
            if (Selection == Team.Dead)
            {
                bruh = null;
                return false;
            }
            List<RoleTypeId> T = new List<RoleTypeId>();
            foreach (RoleTypeId rt in Enum.GetValues(typeof(RoleTypeId)).Cast<RoleTypeId>())
            {
                if (RoleExtensions.GetTeam(rt) == Selection)
                {
                    T.Add(rt);
                }
            }
            bruh = T.ToArray();
            return true;
        }

        public static AdvancedTeam SetUpAfterOriginalTeam(Team t)
        {
            Log.Debug($"Converting Type {t}");
            AdvancedTeam at = new AdvancedTeam
            {
                Name = t.ToString(),
                VanillaTeam = true,
                Chance = 1,
                EscapableClasses = new RoleTypeId[] { },
            };
            if (TeamPlugin.Singleton.Translation.TeamDescriptions.ContainsKey(t))
            {
                at.LastIndexSubclass = new AdvancedTeamSubclass() { Name = t.ToString(), AdvancedTeam = "DEFAULT", Description = TeamPlugin.Singleton.Translation.TeamDescriptions[t] };
            }
            else
            {
                at.LastIndexSubclass = new AdvancedTeamSubclass() { Name = "DEFAULT", AdvancedTeam = "DEFAULT", Description = "Contact Server Owner for Desc...." };
            }
            if (TeamPlugin.Singleton.Translation.TeamDisplayers.ContainsKey(t))
            {
                at.DisplayName = TeamPlugin.Singleton.Translation.TeamDisplayers[t];
            }
            else
            {
                at.DisplayName = t.ToString();
            }
            if (TeamPlugin.Singleton.Translation.TeamCassieSlaughter.ContainsKey(t))
            {
                at.CassieSlaughtered = TeamPlugin.Singleton.Translation.TeamCassieSlaughter[t];
                at.CassieSlaughteredSubtitles = TeamPlugin.Singleton.Translation.TeamCassieSlaughterSubtitles[t];
            }
            else
            {
                at.CassieSlaughtered = String.Empty;
            }
            Log.Debug($"Made Vanilla Team {t} looking snazzy {at.Name}");
            switch (t)
            {
                case Team.ClassD:
                    at.Color = "orange";
                    at.SaidName = "Class D";
                    at.RoundEnderConfig.FriendlyTeams = new List<string>();
                    at.RoundEnderConfig.RequiredTeams = new List<string>()
                    {
                        "Scientists",
                        "SCPs",
                        "FoundationForces"
                    };
                    if (TeamPlugin.Singleton.Config.ClassDFriendsWithChaos)
                    {
                        at.RoundEnderConfig.FriendlyTeams.Add("ChaosInsurgency");
                        at.RoundEnderConfig.FriendlyTeams.Add("ClassD");
                    }
                    break;

                case Team.Scientists:
                    at.Color = "yellow";
                    at.SaidName = "Scientist";
                    at.RoundEnderConfig.FriendlyTeams = new List<string>()
                    {
                        "Scientists",
                        "FoundationForces"
                    };
                    at.RoundEnderConfig.RequiredTeams = new List<string>()
                    {
                        "ClassD",
                        "SCPs",
                        "ChaosInsurgency"
                    };
                    break;

                case Team.SCPs:
                    at.Color = "red";
                    at.SaidName = "SCPSubjects";
                    at.RoundEnderConfig.FriendlyTeams = new List<string>()
                    {
                        "SCPs"
                    };
                    at.RoundEnderConfig.RequiredTeams = new List<string>()
                    {
                        "ClassD",
                        "FoundationForces",
                        "Scientists"
                    };
                    if (!TeamPlugin.Singleton.Config.ScpNeutralWithChaos)
                        at.RoundEnderConfig.RequiredTeams.Add("ChaosInsurgency");
                    break;

                case Team.ChaosInsurgency:
                    at.Color = "green";
                    at.SaidName = "Chaos Insurgency";
                    at.RoundEnderConfig.FriendlyTeams = new List<string>()
                    {
                        "ChaosInsurgency"
                    };
                    if (TeamPlugin.Singleton.Config.ClassDFriendsWithChaos)
                    {
                        at.RoundEnderConfig.FriendlyTeams.Add("ClassD");
                    }
                    at.RoundEnderConfig.RequiredTeams = new List<string>()
                    {
                        "FoundationForces",
                        "Scientists"
                    };
                    if (TeamPlugin.Singleton.Config.ClassDFriendsWithChaos)
                    {
                        at.RoundEnderConfig.RequiredTeams.Add("ClassD");
                    }
                    if (!TeamPlugin.Singleton.Config.ScpNeutralWithChaos)
                        at.RoundEnderConfig.RequiredTeams.Add("SCPs");
                    break;

                case Team.FoundationForces:
                    at.Color = "blue";
                    at.SaidName = "MTFUnit";
                    at.RoundEnderConfig.FriendlyTeams = new List<string>()
                    {
                        "FoundationForces",
                        "Scientists"
                    };
                    at.RoundEnderConfig.RequiredTeams = new List<string>()
                    {
                        "ClassD",
                        "SCPs",
                        "ChaosInsurgency",
                        "Scientists"
                    };
                    break;

                default:
                    at.RoundEnderConfig.FriendlyTeams = new List<string>();
                    at.RoundEnderConfig.RequiredTeams = new List<string>();
                    at.Spectator = true;
                    at.SaidName = "Unspecified";
                    break;
            }
            Log.Debug($"Returning Vanilla Team {t}");
            return at;
        }
    }
}