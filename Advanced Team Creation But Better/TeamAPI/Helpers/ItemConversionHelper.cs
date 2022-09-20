using Exiled.API.Enums;
using System;
using System.Linq;

namespace AdvancedTeamCreation.TeamAPI.Helpers
{
    public static class ItemConversionHelper
    {
        public static bool TryGetItemTypeFromString(this string Weapon, out ItemType itemType)
        {
            itemType = ItemType.None;
            foreach (ItemType item in Enum.GetValues(typeof(ItemType)).Cast<ItemType>())
            {
                if (Weapon == item.ToString())
                {
                    itemType = item;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetAmmoTypeFromString(this string Weapon, out AmmoType AmmoType)
        {
            AmmoType = AmmoType.None;
            foreach (AmmoType item in Enum.GetValues(typeof(AmmoType)).Cast<AmmoType>())
            {
                if (Weapon == item.ToString())
                {
                    AmmoType = item;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetCustomItemTypeFromString(this string Weapon, out Exiled.CustomItems.API.Features.CustomItem Custom)
        {
            if (Exiled.CustomItems.API.Features.CustomItem.TryGet(Weapon, out Exiled.CustomItems.API.Features.CustomItem item))
            {
                Custom = item;
                return true;
            }
            else
            {
                Custom = null;
                return false;
            }
        }

        public static bool TryGetCustomItemTypeFromInt(this int Weapon, out Exiled.CustomItems.API.Features.CustomItem Custom)
        {
            if (Exiled.CustomItems.API.Features.CustomItem.TryGet(Weapon, out Exiled.CustomItems.API.Features.CustomItem item))
            {
                Custom = item;
                return true;
            }
            else
            {
                Custom = null;
                return false;
            }
        }
    }
}