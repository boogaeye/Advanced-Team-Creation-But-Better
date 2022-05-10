using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBB.TeamAPI
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

        public static bool TryGetCustomItemTypeFromString(this string Weapon, out CustomItems.API.CustomItem Custom)
        {
            if (CustomItems.API.API.TryGetItem(Weapon, out CustomItems.API.CustomItem item))
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
