using DiscordBot.LibraryData.Models;
using DiscordBot.Server.Models;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Server.Controllers.Helpers
{
    public static class BotHelper
    {
        public static List<GuildMessageViewModel> PopulateModel()
        {
            List<GuildMessageViewModel> guildMessages = new List<GuildMessageViewModel>()
            {
                new GuildMessageViewModel { MessageId = "raidinfo_msg", Message = "Type a message about your guild raids." },
                new GuildMessageViewModel { MessageId = "raider_msg", Message = "Type a message for people getting Raider rank." },
                new GuildMessageViewModel { MessageId = "welcome_msg", Message = "Type a welcome message for newcomers." },
                new GuildMessageViewModel { MessageId = "rules_msg", Message = "Type some guild rules here." }
            };

            return guildMessages;
        }

        public static List<GuildMessageViewModel> ReplaceDefaultMessages(IEnumerable<GuildMessageDbModel> messages)
        {
            var model = new List<GuildMessageViewModel>();

            messages.ToList()
                .ForEach(m => model
                .Add(new GuildMessageViewModel() { MessageId = m.Id, Message = m.Message }));

            var selectedIDs = new HashSet<string>(messages
                .Select(x => x.Id));

            PopulateModel()
                .Where(s => !selectedIDs.Contains(s.MessageId)).ToList()
                .ForEach(x => model.Add(x));

            return model;
        }
    }
}
