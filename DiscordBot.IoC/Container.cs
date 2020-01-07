using Abstractions;
using DiscordBot.Core;
using Logger;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.IoC
{
    public static class Container
    {
        public static IServiceCollection AddGuildBotDependencies(this IServiceCollection collection)
            => collection
                .AddSingleton<IGuildBot, GuildBot>()
                .AddSingleton<ILogger, InMemoryLogger>();
    }
}
