using System;
using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomItems.API;
using Exiled.Events.EventArgs;
using CustomPlayerEffects;
using Exiled.API.Features.Attributes;

namespace AdvancedTeamCreation.ATCCustomItems
{
    [CustomItem(ItemType.GunRevolver)]
    public class Gun106 : CustomWeapon
    {
        public override uint Id { get; set; } = 0;
        public override string Name { get; set; } = "106 Gun";
        public override string Description { get; set; } = "Send players to the pocket dimension with a chance";
        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties() { Limit = 1, DynamicSpawnPoints = new List<DynamicSpawnPoint>() { new DynamicSpawnPoint() { Chance = 25, Location = SpawnLocation.InsideServersBottom }, new DynamicSpawnPoint() { Chance = 25, Location = SpawnLocation.Inside012 }, new DynamicSpawnPoint() { Chance = 25, Location = SpawnLocation.Inside096 } } };
        public override float Damage { get; set; } = 40;
        public override byte ClipSize { get; set; } = 10;
        public int RandScpPocket { get; set; } = 5;
        public int RandHumanPocket { get; set; } = 30;
        public override bool ShouldMessageOnGban => true;
        public override ItemType Type { get; set; } = ItemType.GunRevolver;

        protected override void OnHurting(HurtingEventArgs ev)
        {
            int rand = new Random().Next(0, 99);
            if (ev.Target.IsScp)
            {
                if (rand < RandScpPocket)
                {
                    ev.Target.EnableEffect<Corroding>();
                }
            }
            else
            {
                if (rand < RandHumanPocket)
                {
                    ev.Target.EnableEffect<Corroding>();
                }
            }
            base.OnHurting(ev);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.Player.ShowHint("This weapon is unreloadable");
            base.OnReloading(ev);
        }
    }
}