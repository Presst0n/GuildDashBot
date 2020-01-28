using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Server.Models;
using DiscordBot.Server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DiscordBot.Server.Resources.Constants;
using DiscordBot.Server.Twitch;
using Microsoft.EntityFrameworkCore;
using Abstractions;
using DiscordBot.LibraryData.Models;
using DiscordBot.Server.Controllers.Helpers;
using Abstractions.Db;

namespace DiscordBot.Server.Controllers
{
    [Authorize(Policy = "AdminRolePolicy")]
    public class BotController : Controller
    {
        private readonly IGuildMessages _guildMessageManager;
        private readonly IGuildNotifications _guildNotificationsManager;
        private readonly ITwitchStreamers _streamersManager;
        private readonly IGuildBot _bot;
        private readonly ILogger _logger;
        private readonly BotEvents _botEvents;

        public BotController(IGuildMessages guildMessageManager, IGuildNotifications guildNotificationsManager, ITwitchStreamers streamersManager,
            IGuildBot bot, ILogger logger, BotEvents botEvents)
        {
            _guildMessageManager = guildMessageManager;
            _guildNotificationsManager = guildNotificationsManager;
            _streamersManager = streamersManager;
            _bot = bot;
            _logger = logger;
            _botEvents = botEvents;
        }

        [HttpGet]
        public IActionResult Commands()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditStreamer(StreamerViewModel model)
        {
            var streamer = await _streamersManager.GetStreamerById(model.UniqueID);

            try
            {
                if (streamer != null)
                {
                    var s = await TwitchHelper.GetStreamer(streamer.StreamerLogin);
                    streamer.StreamerLogin = model.StreamerLogin;
                    streamer.UrlAddress = model.Url;

                    if (s.Users.Length != 0)
                    {
                        streamer.StreamerId = s.Users[0].Id;
                    }

                    await _streamersManager.AddStreamerAsync(streamer);
                }
            }
            catch (DbUpdateException ex)
            {
                ViewBag.ErrorTitle = $"Database error!";
                ViewBag.ErrorMessage = $"Something went wrong. The attempt to save the streamer has failed";
                _logger.Log($"Database Error: {ex.Message} -- {ex.StackTrace}");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditStreamer(string id)
        {
            var streamer = await _streamersManager.GetStreamerById(id);

            if (streamer != null)
            {
                var model = new StreamerViewModel
                {
                    UniqueID = streamer.UniqueID,
                    StreamerLogin = streamer.StreamerLogin,
                    Url = streamer.UrlAddress
                };

                return View(model);
            }
            else
            {
                ViewBag.ErrorMessage = $"Something went wrong. It seems this streamer doesn't exist. ";
                return View("NotFound");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStreamer(string id)
        {
            var streamer = await _streamersManager.GetStreamerById(id);

            if (streamer is null)
            {
                ViewBag.ErrorMessage = $"No streamers found";
                return View("NotFound");
            }
            else
            {
                await _streamersManager.DeleteStreamerAsync(streamer);
            }

            return RedirectToAction("ManageStreamers");
        }

        [HttpPost]
        public async Task<IActionResult> AddStreamer(StreamerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {

                    var dbModel = new StreamerDbModel
                    {
                        StreamerLogin = model.StreamerLogin,
                        UrlAddress = model.Url,
                        UniqueID = Guid.NewGuid().ToString()
                    };

                    var result = await TwitchHelper.GetAndMapStreamerDataAsync(dbModel);

                    if (result is null)
                    {
                        ViewBag.ErrorMessage("", "Cannot find given streamer.");
                        return View("NotFound");
                    }
                    else
                    {
                        ViewBag.Alert = true;
                        ViewBag.Success = true;

                        try
                        {
                            await _streamersManager.AddStreamerAsync(result);
                        }
                        catch (DbUpdateException)
                        {
                            ViewBag.Success = false;
                        }
                    }

                }
                else
                {
                    ViewBag.ErrorMessage("Something went wrong. :(");
                    return View("NotFound");
                }

            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AddStreamer()
        {
            ViewBag.Alert = false;

            return View();
        }

        public async Task<IActionResult> StartBot()
        {
            try
            {
                if (_bot.IsRunning())
                {
                    return BadRequest("Bot is already running.");
                }

                await _bot.Connect();
            }
            catch (Exception ex)
            {

                _logger.Log($"Coś się zesrało :/ {ex.Message} {ex.Data} - {ex.StackTrace} | {ex.InnerException}");

                //ViewBag.ErrorMessage = $"StackTrace: {ex.StackTrace}" +
                //    $"Error Message: {ex.Message}";
                //return View("NotFound");
            }
            await Task.Delay(50);
            return RedirectToAction("ManageBot");
        }

        public async Task<IActionResult> StopBot()
        {
            if (!_bot.IsRunning())
            {
                return BadRequest("Bot is not running.");
            }

            await _bot.Stop();

            await Task.Delay(50);
            return RedirectToAction("ManageBot");
        }


        [HttpGet]
        public async Task<IActionResult> ManageStreamers()
        {
            var streamers = await _streamersManager.GetStreamers();
            var model = new List<StreamerViewModel>();

            if (streamers != null)
            {
                foreach (var streamer in streamers)
                {
                    if (streamer.ProfileImage == null || streamer.ProfileImage.Length == 0)
                    {
                        streamer.ProfileImage = Images.TwitchDefaultPicture;
                    }

                    model.Add(new StreamerViewModel
                    {
                        IsStreaming = streamer.IsStreaming,
                        PlayedGame = streamer.PlayedGame,
                        ProfileImage = streamer.ProfileImage,
                        StreamerId = streamer.StreamerId,
                        StreamerLogin = streamer.StreamerLogin,
                        StreamTitle = streamer.StreamTitle,
                        TotalFollows = streamer.TotalFollows,
                        Viewers = streamer.Viewers,
                        Url = streamer.UrlAddress,
                        LiveIndicator = Images.OnlineStatus,
                        UniqueID = streamer.UniqueID
                    });
                }
            }
            ModelState.Clear();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageGuildNotifications()
        {
            var notification = await _guildNotificationsManager.GetNotificationStatus();
            var model = new GuildNotificationViewModel();

            if (notification is null)
            {
                var dBModel = new GuildNotificationDbModel();
                model.Notify = dBModel.Notify;

                await _guildNotificationsManager.SetNotificationStatus(dBModel);
            }
            else
            {
                model.Notify = notification.Notify;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageGuildNotifications(GuildNotificationViewModel model)
        {
            if (model != null)
            {
                await _guildNotificationsManager.SetNotificationStatus(new GuildNotificationDbModel { Notify = model.Notify });
            }
            else
            {
                ModelState.AddModelError("", "Cannot edit discord notifications :/");
                return View();
            }

            return View(model);
        }

        //[HttpGet]
        //public IActionResult ManageBot()
        //{ 
        //    ViewBag.IsRunning = _bot.IsRunning();
        //    return View();
        //}

        [HttpGet]
        public IActionResult ManageBot(bool value)
        {

            ViewBag.IsRunning = _bot.IsRunning();

            ViewBag.Alert = value;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageGuildMessages()
        {
            var messages = await _guildMessageManager.GetMessages();

            var model = new List<GuildMessageViewModel>();

            if (messages.Any())
            {
                model = BotHelper.ReplaceDefaultMessages(messages);
            }
            else
            {
                model = BotHelper.PopulateModel();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditGuildMessage(GuildMessageViewModel model, string id)
        {
            if (model is null)
            {
                ViewBag.ErrorMessage = $"Message with Id={model.MessageId} cannot be found";

                return View("NotFound");
            }
            else
            {
                var guildMessageDbModel = new GuildMessageDbModel()
                {
                    Id = id,
                    Message = model.Message
                };

                await _guildMessageManager.AddAsync(guildMessageDbModel);

                return RedirectToAction("ManageGuildMessages");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditGuildMessage(string id)
        {
            var msg = await _guildMessageManager.GetMessageById(id);

            var model = new ManageBotViewModel();
            if (msg != null)
            {
                var guildMessage = new GuildMessageViewModel() { MessageId = msg.Id, Message = msg.Message };

                return View(guildMessage);
            }
            else
            {
                var m = BotHelper.PopulateModel();

                var guildMessage = new GuildMessageViewModel()
                {
                    MessageId = id,
                    Message = m.Find(x => x.MessageId == id).Message
                };

                return View(guildMessage);
            }
        }

        [HttpGet]
        public IActionResult Log()
        {
            var model = _logger.GetAll().Reverse();
            return View(model);
        }
    }
}