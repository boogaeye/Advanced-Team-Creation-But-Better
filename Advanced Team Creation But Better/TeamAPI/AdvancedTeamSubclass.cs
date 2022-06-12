using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBB.TeamAPI
{
    public class AdvancedTeamSubclass
    {
        public string AdvancedTeam;
        public string Name { get; set; }
        public string RoleDisplay { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public RoleType Model { get; set; }
        public string[] Inventory { get; set; } = { "None", "None", "None" };
        public string[] AmmoInventory { get; set; } = { "Nato9:90", "Nato556:90", "Nato762:90", "Ammo12Gauge:90", "Ammo44Cal:90" };
        public string Hint { get; set; } = "You are {Team}, Good Luck {Role}";
        public string Color { get; set; } = "yellow";
        
    }
}