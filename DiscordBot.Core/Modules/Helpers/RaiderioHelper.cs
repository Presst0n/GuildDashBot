using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core.Modules.Helpers
{
    public static class RaiderioHelper
    {
        public static string[] ConvertToArray(string name, string realm, string region)
        {
            var uRegion = region.ToUpper();
            string[] output = new string[] { name, realm, uRegion };
            return output;
        }
    }
}
