using System;
using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomItems.API;
using Exiled.Events.EventArgs;
using CustomPlayerEffects;
using Exiled.API.Features.Attributes;
using Exiled.Events.EventArgs.Map;
using Exiled.API.Enums;

namespace AdvancedTeamCreation.ATCCustomItems
{
    [CustomItem(ItemType.GrenadeHE)]
    public class Grenade106 : CustomGrenade
    {
        public override uint Id { get; set; } = 1;
        public override string Name { get; set; } = "106 Grenade";
        public override string Description { get; set; } = "Send players to the pocket dimension with a chance";
        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties() { Limit = 1, DynamicSpawnPoints = new List<DynamicSpawnPoint>() { new DynamicSpawnPoint() { Chance = 25, Location = SpawnLocationType.InsideServersBottom }, new DynamicSpawnPoint() { Chance = 25, Location = SpawnLocationType.Inside049Armory }, new DynamicSpawnPoint() { Chance = 25, Location = SpawnLocationType.Inside096 } } };
        public int RandScpPocket { get; set; } = 5;
        public int RandHumanPocket { get; set; } = 30;
        public override bool ShouldMessageOnGban => true;
        public override ItemType Type { get; set; } = ItemType.GrenadeHE;
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 5;

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            int rand = new Random().Next(0, 99);
            ev.TargetsToAffect.ForEach(e =>
            {
                if (e.IsScp && rand < RandScpPocket)
                {
                    e.EnableEffect<Corroding>(10);
                }
                else if (rand < RandHumanPocket)
                {
                    e.EnableEffect<Corroding>(10);
                    e.Heal(50);
                }
            });
            base.OnExploding(ev);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }
    }
}