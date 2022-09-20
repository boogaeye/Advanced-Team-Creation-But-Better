using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomItems.API;
using Exiled.Events.EventArgs;
using Exiled.API.Features.Attributes;

namespace AdvancedTeamCreation.ATCCustomItems
{
    [CustomItem(ItemType.KeycardO5)]
    public class SerpentsHandCommanderCard : CustomItem
    {
        public override uint Id { get; set; } = 1;
        public override string Name { get; set; } = "Serpents Hand Commander Card";
        public override string Description { get; set; } = "Opens gates and Heavy Containment Checkpoints only\nwith addition of intercom";
        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties() { Limit = 1, DynamicSpawnPoints = new List<DynamicSpawnPoint>() { new DynamicSpawnPoint() { Chance = 5, Location = SpawnLocation.InsideServersBottom }, new DynamicSpawnPoint() { Chance = 15, Location = SpawnLocation.Inside012 }, new DynamicSpawnPoint() { Chance = 1, Location = SpawnLocation.Inside096 } } };
        public override ItemType Type { get; set; } = ItemType.KeycardO5;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.InteractingDoor += DoorOpening;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= DoorOpening;
            base.UnsubscribeEvents();
        }

        public void DoorOpening(InteractingDoorEventArgs ev)
        {
            if (TryGet(ev.Player, out CustomItem ci))
            {
                if (ci.Id == Id)
                {
                    switch (ev.Door.Type)
                    {
                        case Exiled.API.Enums.DoorType.CheckpointEntrance:
                        case Exiled.API.Enums.DoorType.GateA:
                        case Exiled.API.Enums.DoorType.GateB:
                        case Exiled.API.Enums.DoorType.Intercom:
                            ev.IsAllowed = true;
                            break;

                        default:
                            if (ev.Door.RequiredPermissions.RequiredPermissions != Interactables.Interobjects.DoorUtils.KeycardPermissions.None)
                                ev.IsAllowed = false;
                            break;
                    }
                }
            }
        }
    }
}