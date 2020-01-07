using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Server.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
