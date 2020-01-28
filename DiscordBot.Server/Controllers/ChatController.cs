using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abstractions;
using AutoMapper;
using DiscordBot.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DiscordBot.Server.Controllers
{
    public class ChatController : Controller
    {
        private readonly IGuildBot _bot;

        public ChatController(IGuildBot bot)
        {
            _bot = bot;
        }

        public IActionResult Global()
        {
            if (!_bot.IsRunning()) 
            { 
                return RedirectToAction("ManageBot", "Bot", new { value = true }); 
            }

            var model = new ChatViewModel
            {
                AvailableServers = _bot.GetAvailableServers().Select(Mapper.Map<ServerDetailViewModel>),
                MessageBuffer = new List<ChatMessageViewModel>()
            };

            return View(model);
        }

        [HttpGet("[controller]/[action]/{serverId}/{channelId}")]
        public async Task<IActionResult> Channel(ulong serverId, ulong channelId)
        {
            if (!_bot.IsRunning()) { return RedirectToAction("ManageBot", "Bot", true); }

            var model = new ChatViewModel
            {
                ActiveServer = Mapper.Map<ServerDetailViewModel>(_bot.GetServerDetailFromId(serverId)),
                ActiveChannel = Mapper.Map<TextChannelViewModel>(_bot.GetTextChannelDetailFromId(serverId, channelId)),
                AvailableServers = _bot.GetAvailableServers().Select(Mapper.Map<ServerDetailViewModel>),
                MessageBuffer = (await _bot.GetMessageBufferFor(serverId, channelId)).Select(Mapper.Map<ChatMessageViewModel>)
            };

            return View(model);
        }
    }
}