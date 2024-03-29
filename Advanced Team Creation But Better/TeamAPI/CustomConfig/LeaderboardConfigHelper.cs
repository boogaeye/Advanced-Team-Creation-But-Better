﻿using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace AdvancedTeamCreation.TeamAPI.CustomConfig
{
    [Serializable]
    public class LeaderboardConfigHelper
    {
        [Description("Friendly Teams are teams that are deemed friendly to this team\n# if a team is neutral then don't add that team to Friendly or Enemy\n# Can Contain Custom Teams\n# To add a custom team, just add the team name")]
        public List<string> FriendlyTeams { get; set; } = new List<string>()
        {
            "FoundationForces",
            "ChaosInsurgency",
            "SCPs",
            "ClassD",
            "Scientist"
        };

        [Description("Required Teams are teams that need to be completely nonexistance in the game for this team to win\n# Can Contain Custom Teams")]
        public List<string> RequiredTeams { get; set; } = new List<string>()
        {
            "FoundationForces",
            "ChaosInsurgency",
            "SCPs",
            "ClassD",
            "Scientist"
        };
    }
}