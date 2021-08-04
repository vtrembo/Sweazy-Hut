namespace SweazyHut
{
    using System.IO;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Addons.Hosting;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using SweazyHut.Data.Context;
    using SweazyHut.Services;
    using Victoria;

    /// <summary>
    /// Entry point of the Sweazy's Hut bot.
    /// </summary>
    internal class Program
    {
        private static async Task Main()
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .Build();

                    x.AddConfiguration(configuration);
                })
                .ConfigureLogging(x =>
                {
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel.Debug);
                })
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200,
                    };
                    config.Token = context.Configuration["Token"];
                })
                .UseCommandService()
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddHostedService<CommandHandler>()
                        .AddScoped<ISweazyHutDbContext, SweazyHutDbContext>()
                        .AddLavaNode(x =>
                        {
                            x.SelfDeaf = true;

                        });
                })
                .UseConsoleLifetime();
            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}