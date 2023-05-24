using Exiled.API.Features;
using AdvancedTeamCreation.TeamAPI.Extentions;

//using RespawnTimer.API.Features;
//using RespawnTimer;

namespace AdvancedTeamCreation.TeamAPI.Helpers
{
    public class HudHelper
    {
        //public static TimerView NormalTime;

        public static void RaycastHelper(Player ply)
        {
            if (TeamPlugin.Singleton.Config.TeamsListPromptsAtAnnouncement && ply.HasHint) return;
            if (!UnityEngine.Physics.Raycast(ply.CameraTransform.position, ply.CameraTransform.forward, out UnityEngine.RaycastHit raycastHit, 999, 13))
                return;
            if (!UnityEngine.Physics.Raycast(ply.CameraTransform.position, ply.CameraTransform.forward, out UnityEngine.RaycastHit rayNormal, 10, 13))
                return;
            //Makes sure it only gets players???
            Player target = Player.Get(raycastHit.collider.gameObject);
            Player targetEnemy = Player.Get(rayNormal.collider.gameObject);
            if (target == null)
                return;
            if (ply.GetAdvancedTeam().ConfirmFriendshipWithTeam(target.GetAdvancedTeam()))
            {
                ply.ShowHint(TeamPlugin.Singleton.Translation.TeamIsFriendlyHint.Replace("(user)", target.Nickname).Replace("(dist)", raycastHit.distance.ToString()).Replace("(role)", target.GetAdvancedTeam().Name), 0.3f);
            }
            else if (targetEnemy != null && ply.GetAdvancedTeam().ConfirmEnemyshipWithTeam(targetEnemy.GetAdvancedTeam()))
            {
                ply.ShowHint(TeamPlugin.Singleton.Translation.TeamIsHostileHint.Replace("(user)", targetEnemy.Nickname).Replace("(role)", target.GetAdvancedTeam().Name), 0.3f);
            }
            else if (targetEnemy != null && ply.GetAdvancedTeam().ConfirmNeutralshipWithTeam(targetEnemy.GetAdvancedTeam()))
            {
                ply.ShowHint(TeamPlugin.Singleton.Translation.TeamIsNeutralHint.Replace("(user)", targetEnemy.Nickname).Replace("(role)", target.GetAdvancedTeam().Name), 0.3f);
            }
            else if (ply.GetAdvancedTeam().ConfirmRequiredshipWithTeam(target.GetAdvancedTeam()))
            {
                ply.ShowHint(TeamPlugin.Singleton.Translation.TeamIsRequiredHint.Replace("(user)", target.Nickname).Replace("(dist)", raycastHit.distance.ToString()).Replace("(role)", target.GetAdvancedTeam().Name), 0.3f);
            }
        }

        public static void ShowAllPlayersTeamDisplay(int secs)
        {
            foreach (Player p in Player.List)
            {
                if (!p.GetAdvancedTeam().Spectator)
                    p.ShowPlayerTeamDisplay(secs);
            }
        }

        //public static void LoadTimerConfig(AdvancedTeam team)
        //{
        //    if (TeamPlugin.assemblyTimer == null) return;
        //    if (team != null)
        //        RespawnTimer.API.API.TimerView = new TimerView(NormalTime.BeforeRespawnString, NormalTime.DuringRespawnString.Replace("{team}", $"<color={team.Color}>{team.Name}</color>"), NormalTime.Properties);
        //    else
        //        RespawnTimer.API.API.TimerView = NormalTime;
        //}
    }
}