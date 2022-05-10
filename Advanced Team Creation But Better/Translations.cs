using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using Exiled.API;
using System.IO;
using Exiled.Loader;
using Exiled.API.Features;
using ATCBB.TeamAPI;

namespace ATCBB
{
    public class Translations : ITranslation
    {
        public string NoPermissions { get; set; } = "You can't use that here";
    }
}