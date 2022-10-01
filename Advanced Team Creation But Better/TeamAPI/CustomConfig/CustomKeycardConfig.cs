using AdvancedTeamCreation.ATCCustomItems;
using System.ComponentModel;

namespace AdvancedTeamCreation.TeamAPI.CustomConfig
{
    public class CustomKeycardConfig
    {
        public bool RegisterKeycard { get; set; } = false;
        public CustomKeycard CustomKeycard { get; set; } = new CustomKeycard();
    }
}