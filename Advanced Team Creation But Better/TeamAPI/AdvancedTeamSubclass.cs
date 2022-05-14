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
    }
}