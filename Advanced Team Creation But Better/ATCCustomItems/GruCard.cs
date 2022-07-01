using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomItems.API.EventArgs;
using Exiled.CustomItems.API;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs;
using CustomPlayerEffects;
using Exiled.API.Features.Attributes;

namespace ATCBB.ATCCustomItems
{
    [CustomItem(ItemType.KeycardResearchCoordinator)]
    public class GruCard : CustomItem
    {
        public override uint Id { get; set; } = 3;
        public override string Name { get; set; } = "GRU Card";
        public override string Description { get; set; } = "Opens gates and Heavy Containment Checkpoints only\nwith addition of intercom";
        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; }
        public override ItemType Type { get; set; } = ItemType.KeycardResearchCoordinator;

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
                        case Exiled.API.Enums.DoorType.Scp914Gate:
                            ev.IsAllowed = true;
                            break;

                        default:
                            if (ev.Door.RequiredPermissions.RequiredPermissions != Interactables.Interobjects.DoorUtils.KeycardPermissions.None)
                                ev.IsAllowed = false;
                            if (ev.Door.RequiredPermissions.RequiredPermissions == Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelOne)
                            {
                                ev.IsAllowed = true;
                            }
                            break;
                    }
                }
            }
        }
    }
}