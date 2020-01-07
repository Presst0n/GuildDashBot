//using DiscordBot.LibraryData;
//using DiscordBot.LibraryData.Models;
//using System.Linq;
//using System.Threading.Tasks;

//namespace DiscordBot.LibraryServices
//{
//    public class TwitchChannelService : ITwitchChannel
//    {
//        private LibraryContext _context;

//        public TwitchChannelService(LibraryContext context)
//        {
//            _context = context;
//        }

//        public Task<ChannelDbModel> GetId()
//        {
//            return Task.FromResult(_context.TwitchChannel.FirstOrDefault());
//        }

//        public async Task SetId(ulong id)
//        {
//            if (_context.TwitchChannel.Where(x => x.Channel_Id != id).Count() >= 1)
//            {
//                ChannelDbModel ChannelFromDb = _context.TwitchChannel.FirstOrDefault();
//                _context.TwitchChannel.Remove(ChannelFromDb);
//                _context.TwitchChannel.Add(new ChannelDbModel
//                {
//                    Channel_Id = id,
//                });

//            }
//            else if (_context.TwitchChannel.Where(x => x.Channel_Id == id).Count() < 1)
//            {
//                _context.TwitchChannel.Add(new ChannelDbModel
//                {
//                    Channel_Id = id
//                });
//            }
            
//            await _context.SaveChangesAsync();
//        }
//    }
//}
