using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBot.LibraryData.Models
{
    public class ChannelDbModel
    {
        [Key]
        public ulong Channel_Id { get; set; }
    }
}
