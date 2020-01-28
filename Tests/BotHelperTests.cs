using DiscordBot.LibraryData.Models;
using DiscordBot.Server.Controllers.Helpers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests
{
    public class BotHelperTests
    {
        [Theory]
        [InlineData("Piece of cake", "raidinfo_msg", "raider_msg", "welcome_msg", "rules_msg")]
        public void ReplaceDefaultMessages_Should_Override_MessageProperty_Of_Existing_Instance_With_Given_Id_From_Collection
            (string msg, string id1, string id2, string id3, string id4)
        {
            // Arrange
            List<GuildMessageDbModel> fakeMessagesLoadedFromDatabase = new List<GuildMessageDbModel> 
            { 
                new GuildMessageDbModel { Id = id1, Message = msg },
                new GuildMessageDbModel { Id = id2, Message = msg },
                new GuildMessageDbModel { Id = id3, Message = msg },
                new GuildMessageDbModel { Id = id4, Message = msg }
            };

            // Act
            var result = BotHelper.ReplaceDefaultMessages(fakeMessagesLoadedFromDatabase);

            // Assert
            foreach (var item in fakeMessagesLoadedFromDatabase)
            {
                Assert.Contains(msg, result.Where(x => x.MessageId == item.Id).Select(c => c.Message).First());
            }
        }
    }
}
