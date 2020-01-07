//using DiscordBot.LibraryData;
//using DiscordBot.LibraryData.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace DiscordBot.LibraryServices
//{
//    public class TwitchStreamersService : ITwitchStreamers
//    {
//        private LibraryContext _context;

//        public TwitchStreamersService(LibraryContext context)
//        {
//            _context = context;
//        }

//        public async Task AddStreamerAsync(StreamerDbModel streamer)
//        {
//            if(_context.TwitchStreamers.Where(x => /*x.StreamerLogin == streamer.StreamerLogin || */x.UniqueID == streamer.UniqueID).Count() < 1)
//            {
//                _context.TwitchStreamers.Add(new StreamerDbModel
//                {
//                    StreamerLogin = streamer.StreamerLogin,
//                    StreamerId = streamer.StreamerId,
//                    UniqueID = streamer.UniqueID,
//                    IsStreaming = streamer.IsStreaming,
//                    SentMessage = streamer.SentMessage,
//                    ProfileImage = streamer.ProfileImage,
//                    StreamTitle = streamer.StreamTitle,
//                    PlayedGame = streamer.PlayedGame,
//                    TotalFollows = streamer.TotalFollows,
//                    UrlAddress = streamer.UrlAddress,
//                    Viewers = streamer.Viewers
//                });
//            }
//            else
//            {
//                var existingStreamer = _context.TwitchStreamers.Where(x => /*x.StreamerLogin == streamer.StreamerLogin ||*/ x.UniqueID == streamer.UniqueID).FirstOrDefault();
//                existingStreamer.StreamerLogin = streamer.StreamerLogin;
//                existingStreamer.StreamerId = streamer.StreamerId;
//                existingStreamer.UniqueID = streamer.UniqueID;
//                existingStreamer.IsStreaming = streamer.IsStreaming;
//                existingStreamer.SentMessage = streamer.SentMessage;
//                existingStreamer.ProfileImage = streamer.ProfileImage;
//                existingStreamer.PlayedGame = streamer.PlayedGame;
//                existingStreamer.StreamTitle = streamer.StreamTitle;
//                existingStreamer.TotalFollows = streamer.TotalFollows;
//                existingStreamer.UrlAddress = streamer.UrlAddress;
//                existingStreamer.Viewers = streamer.Viewers;

//                _context.TwitchStreamers.Update(existingStreamer);
//            }

//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteStreamerAsync(StreamerDbModel streamer)
//        {
//            if(_context.TwitchStreamers.Where(x => x.UniqueID == streamer.UniqueID).Count() > 0)
//            {
//                _context.TwitchStreamers.Remove(
//                    _context.TwitchStreamers
//                    .Where(s => s.StreamerLogin == streamer.StreamerLogin)
//                    .First());

//                await _context.SaveChangesAsync();
//            }
//        }

//        public async Task<StreamerDbModel> GetStreamerById(string id)
//        {
//            return await Task.FromResult(_context.TwitchStreamers.Where(x => x.UniqueID == id).FirstOrDefault());
//        }

//        public Task<IEnumerable<StreamerDbModel>> GetStreamers()
//        {
//            return Task.FromResult<IEnumerable<StreamerDbModel>>(_context.TwitchStreamers.ToList());
//        }
//    }
//}
