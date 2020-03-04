using Abstractions;
using Logger;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.IoC
{
    public static class Container
    {
        public static IServiceCollection AddDependencies(this IServiceCollection collection)
            => collection
                .AddSingleton<ILogger, InMemoryLogger>();
    }
}
