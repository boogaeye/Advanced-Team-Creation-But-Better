using Exiled.API.Enums;
using System.ComponentModel;

namespace AdvancedTeamCreation.TeamAPI.CustomConfig
{
    public class CustomStartRoundSpawnConfig
    {
        public int ChanceReplace { get; set; } = 0;

        [Description("What Roles to have a chance to replace")]
        public RoleType[] RoleType { get; set; } = new RoleType[] { global::RoleType.FacilityGuard, global::RoleType.ClassD };

        [Description("Rooms they will spawn in ONLY at the start of the round")]
        public RoomType[] SpawnRooms { get; set; } = new RoomType[] { RoomType.Hcz939 };
    }
}