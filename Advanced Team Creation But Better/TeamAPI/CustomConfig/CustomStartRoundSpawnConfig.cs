using Exiled.API.Enums;
using PlayerRoles;
using System.ComponentModel;

namespace AdvancedTeamCreation.TeamAPI.CustomConfig
{
    public class CustomStartRoundSpawnConfig
    {
        public int ChanceReplace { get; set; } = 0;

        [Description("What Roles to have a chance to replace")]
        public RoleTypeId[] RoleType { get; set; } = new RoleTypeId[] { RoleTypeId.FacilityGuard, RoleTypeId.ClassD };

        [Description("Rooms they will spawn in ONLY at the start of the round")]
        public RoomType[] SpawnRooms { get; set; } = new RoomType[] { RoomType.Hcz939 };
    }
}