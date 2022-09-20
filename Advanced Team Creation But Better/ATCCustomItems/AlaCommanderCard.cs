using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs;

namespace AdvancedTeamCreation.ATCCustomItems
{
    [Exiled.API.Features.Attributes.CustomItem(ItemType.KeycardFacilityManager)]
    public class AlaCommanderCard : CustomItem
    {
        public override uint Id { get; set; } = 8;
        public override string Name { get; set; } = "ALA Commander Card";
        public override string Description { get; set; } = "Opens gates and Heavy Containment Checkpoints only\nwith addition of intercom";
        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; }
        public override ItemType Type { get; set; } = ItemType.KeycardFacilityManager;

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
            if (ev.Door.IsLocked) return;
            if (TryGet(ev.Player, out CustomItem ci))
            {
                if (ci.Id == Id)
                {
                    switch (ev.Door.Type)
                    {
                        case Exiled.API.Enums.DoorType.CheckpointEntrance:
                        case Exiled.API.Enums.DoorType.CheckpointLczA:
                        case Exiled.API.Enums.DoorType.CheckpointLczB:
                        case Exiled.API.Enums.DoorType.GateA:
                        case Exiled.API.Enums.DoorType.GateB:
                        case Exiled.API.Enums.DoorType.HID:
                        case Exiled.API.Enums.DoorType.Scp106Primary:
                        case Exiled.API.Enums.DoorType.Scp106Secondary:
                        case Exiled.API.Enums.DoorType.Scp106Bottom:
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
                            else if (ev.Door.RequiredPermissions.RequiredPermissions == Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelTwo)
                            {
                                ev.IsAllowed = true;
                            }
                            else if (ev.Door.RequiredPermissions.RequiredPermissions == Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelThree)
                            {
                                ev.IsAllowed = true;
                            }
                            else if (ev.Door.RequiredPermissions.RequiredPermissions == Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelOne)
                            {
                                ev.IsAllowed = true;
                            }
                            else if (ev.Door.RequiredPermissions.RequiredPermissions == Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelTwo)
                            {
                                ev.IsAllowed = true;
                            }
                            else if (ev.Door.RequiredPermissions.RequiredPermissions == Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelThree)
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