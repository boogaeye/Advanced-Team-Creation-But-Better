using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBB.TeamAPI.CustomConfig
{
    public class LeaderboardConfigHelper
    {
        [Description("Friendly Teams are teams that are deemed friendly to this team\nif a team is neutral then don't add that team to Friendly or Enemy\nCan Contain Custom Teams\nTo add a custom team, just added the team name")]
        public List<string> FriendlyTeams { get; set; } = new List<string>()
        {
            "MTF",
            "CHI",
            "SCP",
            "CDP",
            "RSC"
        };
        [Description("Required Teams are teams that need to be completely nonexistance in the game for this team to win\nCan Contain Custom Teams")]
        public List<string> RequiredTeams { get; set; } = new List<string>()
        {
            "MTF",
            "CHI",
            "SCP",
            "CDP",
            "RSC"
        };
    }
}
