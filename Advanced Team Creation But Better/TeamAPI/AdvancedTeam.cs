using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API;
using Exiled.API.Features;

namespace ATCBB.TeamAPI
{
    public class AdvancedTeam
    {
        public static readonly string DEFAULTAnnounce = "DEFAULT";
        public string Name { get; set; } = "No Team";
        public RoomType SpawnRoom { get; set; } = RoomType.Surface;
        public int Chance { get; set; } = 75;
        public string[] SpawnOrder { get; set; } = { "Commander:1", "Officer:3", "Cadet:5" };
        public string CassieAnnouncement { get; set; } = DEFAULTAnnounce;
        public string CassieAnnouncementSubtitles { get; set; } = DEFAULTAnnounce;
        public bool VanillaTeam;
    }
}