namespace AdvancedTeamCreation.TeamAPI.Helpers
{
    public class RespawnHelper
    {
        public static LeaderboardHelper Leaderboard { get; } = new LeaderboardHelper();
        public static AdvancedTeam ReferancedTeam { get; private set; }
        public static AdvancedTeam CassieHelper { get; private set; }
        public static AdvancedTeam LastTeamSpawned { get; private set; }

        public static AdvancedTeam SetReferance(AdvancedTeam advancedTeam)
        {
            ReferancedTeam = advancedTeam;

            if (advancedTeam != null)
            {
                CassieHelper = advancedTeam;
                LastTeamSpawned = advancedTeam;
            }
            return advancedTeam;
        }

        public static void ResetCassie()
        {
            CassieHelper = null;
        }
    }
}