
namespace Abstractions.Db
{
    public interface IDataAccess : ITwitchStreamers, ITwitchChannel, IGuildNotifications, IGuildMessages
    {
        //Task AddAsync(GuildMessageDbModel message);
        //Task<GuildMessageDbModel> GetMessageById(string id);
        //Task<IEnumerable<GuildMessageDbModel>> GetMessages();
    }
}
