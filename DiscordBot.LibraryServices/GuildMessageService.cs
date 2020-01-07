//using DiscordBot.LibraryData;
//using DiscordBot.LibraryData.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace DiscordBot.LibraryServices
//{
//    public class GuildMessageService : IGuildMessages
//    {
//        private LibraryContext _context;

//        public GuildMessageService(LibraryContext context)
//        {
//            _context = context;
//        }

//        public async Task AddAsync(GuildMessageDbModel model)
//        {
//            var output = _context.GuildMessages.Find(model.Id);
//            if (output is null)
//            {
//                _context.GuildMessages.Add(model);
//            }
//            else
//            {
//                output.Message = model.Message;
//                _context.GuildMessages.Update(output);

//            }

//            await _context.SaveChangesAsync();
//        }

//        public Task<GuildMessageDbModel> GetMessageById(string id)
//        {
//            return Task.FromResult(_context.GuildMessages
//                .Where(x => x.Id == id)
//                .FirstOrDefault());
//        }

//        public Task<IEnumerable<GuildMessageDbModel>> GetMessages()
//        {
//            return Task.FromResult<IEnumerable<GuildMessageDbModel>>(_context.GuildMessages);
//        }
//    }
//}

