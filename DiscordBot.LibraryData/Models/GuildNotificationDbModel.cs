using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBot.LibraryData.Models
{
    public class GuildNotificationDbModel
    {
        [Key]
        public string Id { get; set; } = "notify_msg";
        public bool Notify { get; set; } = true;
    }
}
