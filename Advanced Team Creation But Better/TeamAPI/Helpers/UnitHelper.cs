using Respawning.NamingRules;
using Respawning;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using System.Xml.Linq;
using PlayerRoles;

namespace AdvancedTeamCreation.TeamAPI.Helpers
{
    public class UnitHelper
    {
        private static List<AdvancedTeamSubclass> RegisteredSubTeams = new List<AdvancedTeamSubclass>();
        private static List<AdvancedTeam> RegisteredTeams = new List<AdvancedTeam>();

        public static AdvancedTeam[] Teams => RegisteredTeams.ToArray();
        public static AdvancedTeamSubclass[] Subteams => RegisteredSubTeams.ToArray();

        //public static List<AdvancedTeam> AdvancedTeamsChi => RegisteredTeams.Where(e => !e.VanillaTeam && e.BindedTeam == SpawnableTeamType.ChaosInsurgency || e.BindedTeam == SpawnableTeamType.None).ToList();
        //public static List<AdvancedTeam> AdvancedTeamsMtf => RegisteredTeams.Where(e => !e.VanillaTeam && e.BindedTeam == SpawnableTeamType.NineTailedFox || e.BindedTeam == SpawnableTeamType.None).ToList();
        public static List<AdvancedTeam> AdvancedTeamsOnly => RegisteredTeams.Where(e => !e.VanillaTeam).ToList();

        //TODO Unit Naming Convension
        public static void AddUnitNameOnAdvancedTeam(AdvancedTeam Name, string UnitName)
        {
            //UnitNamingRules.AllNamingRules[SpawnableTeamType.NineTailedFox].AddCombination($"<color={Name.Color}>{Name.Name}-{UnitName}</color>", SpawnableTeamType.NineTailedFox);
        }

        public static void ChangeUnitNameOnAdvancedTeam(int index, AdvancedTeam Name, string UnitName)
        {
            //RespawnManager.Singleton.NamingManager.AllUnitNames.Remove(RespawnManager.Singleton.NamingManager.AllUnitNames[index]);
            //UnitNamingRules.AllNamingRules[SpawnableTeamType.NineTailedFox].AddCombination($"<color={Name.Color}>{Name.Name}-{UnitName}</color>", SpawnableTeamType.NineTailedFox);
        }

        public static void ChangeUnitNameOnAdvancedTeam(int index, string UnitName)
        {
            //RespawnManager.Singleton.NamingManager.AllUnitNames.Remove(RespawnManager.Singleton.NamingManager.AllUnitNames[index]);
            //UnitNamingRules.AllNamingRules[SpawnableTeamType.NineTailedFox].AddCombination(UnitName, SpawnableTeamType.NineTailedFox);
        }

        public static AdvancedTeamSubclass FindAST(string Team, string name)
        {
            foreach (AdvancedTeam at in RegisteredTeams)
            {
                if (at.Name == Team)
                {
                    foreach (AdvancedTeamSubclass sb in RegisteredSubTeams)
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

        public static AdvancedTeam FindAT(string name)
        {
            foreach (AdvancedTeam at in RegisteredTeams)
            {
                if (at.Name == name)
                {
                    return at;
                }
            }
            return null;
        }

        public static void RegisterTeam(AdvancedTeam at, params AdvancedTeamSubclass[] advancedTeamSubclasses)
        {
            RegisteredTeams.Add(at);
            RegisteredSubTeams.AddRange(advancedTeamSubclasses);
            Log.Info($"Registered {at.Name} and {advancedTeamSubclasses.Length} Subteams Default Subclass: {at.LastIndexSubclass.Name}");
        }

        public static void RegisterTeam(Team t)
        {
            var at = OriginalToCustomTeamHelper.SetUpAfterOriginalTeam(t);
            RegisteredTeams.Add(at);
            Log.Info($"Registered {at.Name} Subteams Default Subclass: {at.LastIndexSubclass.Name}");
        }

        public static void UnregisterTeams()
        {
            Log.Info($"Unregistered all teams");
            RegisteredTeams.Clear();
            RegisteredSubTeams.Clear();
        }
    }
}