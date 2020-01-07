using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBot.LibraryData.Models
{
    public class GuildMessageDbModel
    {
        [Key]
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }  // I might want to use this property later to display to the user info about last made change.
    }
}
