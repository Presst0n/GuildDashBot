using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class ServerDetail
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public IEnumerable<TextChannel> TextChannels { get; set; }
    }
}
