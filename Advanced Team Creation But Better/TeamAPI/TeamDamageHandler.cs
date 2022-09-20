﻿using Footprinting;
using PlayerStatsSystem;
using Subtitles;
using Exiled.API.Features;

namespace AdvancedTeamCreation.TeamAPI
{
    public class TeamDamageHandler : CustomReasonDamageHandler
    {
        public override float Damage { get; set; }

        public override CassieAnnouncement CassieDeathAnnouncement => _cassieAnnouncement;

        public override string ServerLogsText => "Killed with a custom team reason - " + _deathReason;
        public Footprint Attacker { get; set; }

        public TeamDamageHandler(Player _Attacker, string customReason, float damage, string customCassieAnnouncement = "") : base(customReason)
        {
            _deathReason = customReason;
            Attacker = _Attacker.Footprint;
            Damage = damage;
            _cassieAnnouncement.Announcement = customCassieAnnouncement;
            _cassieAnnouncement.SubtitleParts = new SubtitlePart[1]
            {
                new SubtitlePart(SubtitleType.Custom, new string[1] { customCassieAnnouncement })
            };
        }
    }
}