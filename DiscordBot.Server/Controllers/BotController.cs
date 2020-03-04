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
using DiscordBot.Server.Bot;
using DiscordBot.Server.Bot.Services;

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
        private readonly TwitchHelper _twitchHelper;
        private readonly ICommandHandler _commandHandler;
        private readonly ITwitchLiveMonitor _liveMonitor;
        private readonly IGuildRolesManager _guildRolesManager;

        public BotController(IGuildMessages guildMessageManager, IGuildNotifications guildNotificationsManager, ITwitchStreamers streamersManager,
            IGuildBot bot, ILogger logger, BotEvents botEvents, TwitchHelper twitchHelper, ICommandHandler commandHandler, 
            ITwitchLiveMonitor liveMonitor, IGuildRolesManager guildRolesManager)
        {
            _guildMessageManager = guildMessageManager;
            _guildNotificationsManager = guildNotificationsManager;
            _streamersManager = streamersManager;
            _bot = bot;
            _logger = logger;
            _botEvents = botEvents;
            _twitchHelper = twitchHelper;
            _commandHandler = commandHandler;
            _liveMonitor = liveMonitor;
            _guildRolesManager = guildRolesManager;
        }

        [HttpGet]
        public IActionResult Commands()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditStreamer(StreamerViewModel model)
        {
            var streamer = await _streamersManager.GetStreamerByUniqueId(model.UniqueID);

            try
            {
                if (streamer != null)
                {
                    var s = await _twitchHelper.GetStreamer(streamer.StreamerLogin);
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
            var streamer = await _streamersManager.GetStreamerByUniqueId(id);

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
            var streamer = await _streamersManager.GetStreamerByUniqueId(id);

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

                    var result = await _twitchHelper.GetAndMapStreamerDataAsync(dbModel);

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
                        if (_liveMonitor.IsEnabled())
                        {
                            await _liveMonitor.CheckNewChannelsToMonitor();
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

                await _commandHandler.InitializeAsync();
                await _guildRolesManager.InitializeAsync();
                await _bot.Connect();
                await _liveMonitor.StartLiveMonitor();
                await Task.Delay(50);
                return RedirectToAction("ManageBot");
            }
            catch (Exception ex)
            {
                _logger.Log($"Starting bot has failed: {ex.Message} {ex.Data} - {ex.StackTrace}");

                ViewBag.ErrorTitle = $"Houston we have a problem :/";
                ViewBag.ErrorMessage = $"Error Message: {ex.Message}" +
                    $"Stack trace: {ex.StackTrace}";
                return View("Error");
            }
        }

        public async Task<IActionResult> StopBot()
        {
            if (!_bot.IsRunning())
            {
                return BadRequest("Bot is not running.");
            }


            try
            {
                await _commandHandler.Clear();
                await _guildRolesManager.Clear();
                await _bot.Stop();
            }
            catch (Exception ex)
            {
                _logger.Log($"{ex.Message} - {ex.StackTrace}");
            }

            await Task.Delay(50);
            return RedirectToAction("ManageBot");
        }


        [HttpGet]
        public async Task<IActionResult> ManageStreamers()
        {
            var streamers = await _streamersManager.GetStreamers();
            var model = new List<StreamerViewModel>();
            try
            {
                if (streamers != null)
                {
                    List<Task<StreamerDbModel>> tasks = new List<Task<StreamerDbModel>>();
                    foreach (var streamer in streamers)
                    {
                        tasks.Add(Task.Run(() => _twitchHelper.GetMappedStreamerAsync(streamer)));
                    }

                    var result = await Task.WhenAll(tasks);
                    tasks.Clear();
                    foreach (var s in result)
                    {
                        model.Add(new StreamerViewModel
                        {
                            IsStreaming = s.IsStreaming,
                            //PlayedGame = s.PlayedGame,
                            ProfileImage = s.ProfileImage,
                            StreamerId = s.StreamerId,
                            StreamerLogin = s.StreamerLogin,
                            StreamTitle = s.StreamTitle,
                            //TotalFollows = s.TotalFollows,
                            //Viewers = s.Viewers,
                            //Url = s.UrlAddress,
                            LiveIndicator = Images.OnlineStatus,
                            UniqueID = s.UniqueID
                        });
                    }

                    //foreach (var streamer in streamers)
                    //{
                    //    var mappitoStrimero = await _twitchHelper.GetAndMapStreamerDataAsync(streamer);

                    //    if (mappitoStrimero.ProfileImage == null || mappitoStrimero.ProfileImage.Length == 0)
                    //    {
                    //        mappitoStrimero.ProfileImage = Images.TwitchDefaultPicture;
                    //    }

                    //    model.Add(new StreamerViewModel
                    //    {
                    //        IsStreaming = mappitoStrimero.IsStreaming,
                    //        PlayedGame = mappitoStrimero.PlayedGame,
                    //        ProfileImage = mappitoStrimero.ProfileImage,
                    //        StreamerId = mappitoStrimero.StreamerId,
                    //        StreamerLogin = mappitoStrimero.StreamerLogin,
                    //        StreamTitle = mappitoStrimero.StreamTitle,
                    //        TotalFollows = mappitoStrimero.TotalFollows,
                    //        Viewers = mappitoStrimero.Viewers,
                    //        Url = mappitoStrimero.UrlAddress,
                    //        LiveIndicator = Images.OnlineStatus,
                    //        UniqueID = mappitoStrimero.UniqueID
                    //    });
                    //}
                }

                ModelState.Clear();
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorTitle = $"Houston we have a problem :/";
                ViewBag.ErrorMessage = $"Error message:{ex.Message} Stack trace: {ex.StackTrace}";
                return View("Error");
            }
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