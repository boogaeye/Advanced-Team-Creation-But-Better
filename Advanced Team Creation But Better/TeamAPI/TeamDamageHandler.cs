using Exiled.API.Features;
using Footprinting;
using Mirror;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTeamCreation.TeamAPI
{
    public class TeamDamageHandler : AttackerDamageHandler
    {
        public override Footprint Attacker { get; set; }

        public override bool AllowSelfDamage => true;

        public override float Damage { get; set; }
        public override bool ForceFullFriendlyFire { get; set; } = true;
        public override bool IgnoreFriendlyFireDetector => true;
        public override string ServerLogsText => "Custom Event for Advanced Team Creation damage";
        public Player Target;
        public string _deathReason;

        public TeamDamageHandler(Player target, Player attacker, string deathReason, float dmg)
        {
            Attacker = attacker.Footprint;
            Target = target;
            _deathReason = deathReason;
            Damage = dmg;
        }

        public override void WriteAdditionalData(NetworkWriter writer)
        {
            base.WriteAdditionalData(writer);
            writer.WriteString(_deathReason);
        }

        public override void ReadAdditionalData(NetworkReader reader)
        {
            base.ReadAdditionalData(reader);
            _deathReason = reader.ReadString();
        }
    }
}