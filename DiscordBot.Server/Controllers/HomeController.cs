using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DiscordBot.Server.Models;
using DiscordBot.Server;
using DiscordBot.LibraryData;
using Microsoft.Extensions.DependencyInjection;
using Abstractions;
using DiscordBot.Server.ViewModels;

namespace DiscordBot.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGuildBot _bot;
        private readonly ILogger _logger;

        public HomeController(IGuildBot bot, ILogger logger)
        {
            _bot = bot;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
