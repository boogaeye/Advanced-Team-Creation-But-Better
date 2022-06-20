using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATCBB.ATCCustomItems;

namespace ATCBB.TeamAPI.CustomConfig
{
    public class AtcItems
    {
        public Gun106 Gun106 { get; set; } = new Gun106();
        public AlaCard AlaCard { get; set; } = new AlaCard();
        public AlaFirstCard AlaFirstCard { get; set; } = new AlaFirstCard();
        public AlaOfficerCard AlaOfficerCard { get; set; } = new AlaOfficerCard();
        public AlaCommanderCard AlaCommanderCard { get; set; } = new AlaCommanderCard();
        public SerpentsHandCard SerpentsCard { get; set; } = new SerpentsHandCard();
        public SerpentsHandCommanderCard SerpentsHandCommandCard { get; set; } = new SerpentsHandCommanderCard();
        public GruCard GruCard { get; set; } = new GruCard();
        public GruCommanderCard GruCommandCard { get; set; } = new GruCommanderCard();
        public TtaCard TtaCard { get; set; } = new TtaCard();
        public Grenade106 Grenade106 { get; set; } = new Grenade106();
    }
}
