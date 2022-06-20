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
    [CustomItem(ItemType.KeycardJanitor)]
    public class SerpentsHandCard : CustomItem
    {
        public override uint Id { get; set; } = 4;
        public override string Name { get; set; } = "Serpents Hand Card";
        public override string Description { get; set; } = "Opens gates and Heavy Containment Checkpoints only";
        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; }
        public override ItemType Type { get; set; } = ItemType.KeycardJanitor;

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
                        case Exiled.API.Enums.DoorType.GateA:
                        case Exiled.API.Enums.DoorType.GateB:
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
