using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.Events.EventArgs;
using Exiled.API.Enums;
using Interactables.Interobjects.DoorUtils;
using KeycardPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions;
using RemoteKeycard.API.EventArgs;
using RemoteKeycard.Handlers;
using System.Collections;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace AdvancedTeamCreation.ATCCustomItems
{
    public class CustomKeycard : CustomItem
    {
        public override uint Id { get; set; } = 0;
        public override string Name { get; set; } = "Custom Keycard";
        public override string Description { get; set; } = "Opens gates and Heavy Containment Checkpoints only\nwith addition of intercom";
        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; }
        public override ItemType Type { get; set; } = ItemType.KeycardGuard;
        public DoorType[] DoorAccess { get; set; } = new DoorType[] { DoorType.GateA, DoorType.GateB };
        public KeycardPermissions[] KeycardPermissions { get; set; } = new KeycardPermissions[] { Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelOne, Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelTwo, Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelThree };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.InteractingDoor += DoorOpening;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += ActivatingWarhead;
            Exiled.Events.Handlers.Player.InteractingLocker += InteractingLocker;
            Exiled.Events.Handlers.Player.UnlockingGenerator += UnlockingGenerator;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= DoorOpening;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= ActivatingWarhead;
            Exiled.Events.Handlers.Player.InteractingLocker -= InteractingLocker;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= UnlockingGenerator;
            base.UnsubscribeEvents();
        }

        public static bool CheckPerms(KeycardPermissions[] kp, params KeycardPermissions[] kp2)
        {
            foreach (KeycardPermissions k in kp2)
            {
                Log.Info($"Checking Perm {k}");
                if (kp.Contains(k))
                {
                    Log.Info($"Has Perm {k}");
                    return true;
                }
            }
            return false;
        }

        private void UnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (TryGet(ev.Player, out CustomItem ci))
            {
                if (ci.Id == Id)
                {
                    ev.IsAllowed = CheckPerms(KeycardPermissions, ev.Generator.Base._requiredPermission);
                }
            }
        }

        private void InteractingLocker(InteractingLockerEventArgs ev)
        {
            if (TryGet(ev.Player, out CustomItem ci))
            {
                if (ci.Id == Id)
                {
                    ev.IsAllowed = CheckPerms(KeycardPermissions, ev.Chamber.RequiredPermissions);
                }
            }
        }

        private void ActivatingWarhead(ActivatingWarheadPanelEventArgs ev)
        {
            if (TryGet(ev.Player, out CustomItem ci))
            {
                if (ci.Id == Id)
                {
                    ev.IsAllowed = CheckPerms(KeycardPermissions, Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead);
                }
            }
        }

        public void DoorOpening(InteractingDoorEventArgs ev)
        {
            if (TryGet(ev.Player, out CustomItem ci))
            {
                if (ci.Id == Id)
                {
                    if (ev.Door.RequiredPermissions.RequiredPermissions == Interactables.Interobjects.DoorUtils.KeycardPermissions.None)
                    {
                        ev.IsAllowed = true;
                        return;
                    }

                    foreach (DoorType dt in DoorAccess)
                    {
                        if (ev.Door.Type == dt)
                        {
                            ev.IsAllowed = true;
                            return;
                        }
                    }
                    ev.IsAllowed = CheckPerms(KeycardPermissions, ev.Door.RequiredPermissions.RequiredPermissions);
                }
            }
        }
    }
}