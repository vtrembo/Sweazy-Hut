namespace SweazyHut.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Addons.Hosting;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Victoria;

    /// <summary>
    /// Class responsible for handling all commands and various events.
    /// </summary>
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider provider;
        private readonly DiscordSocketClient client;
        private readonly CommandService service;
        private readonly IConfiguration configuration;
        private readonly LavaNode lavanode;

        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration configuration, LavaNode lavanode)
        {
            this.provider = provider;
            this.client = client;
            this.service = service;
            this.configuration = configuration;
            this.lavanode = lavanode;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            this.client.MessageReceived += this.OnMessageReceived;
            this.service.CommandExecuted += this.OnCommandExecutedAsync;
            this.client.Ready += this.OnReadyAsync;
            await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext commandContext, IResult result)
        {
            if (!command.IsSpecified || result.IsSuccess) { return; }
            string title = string.Empty;
            string description = string.Empty;

            switch (result.Error)
            {
                case CommandError.BadArgCount:
                    title = "Invalid use of command";
                    description = "Please provide the correct amount of parameters.";
                    break;
                case CommandError.MultipleMatches:
                    title = "Invalid argument";
                    description = "Please provide a valid argument.";
                    break;
                case CommandError.ObjectNotFound:
                    title = "Not found";
                    description = "The argument that was provided could not be found.";
                    break;
                case CommandError.ParseFailed:
                    title = "Invalid argument";
                    description = "The argument that you provided could not be parsed correctly.";
                    break;
                case CommandError.UnmetPrecondition:
                    title = "Access denied";
                    description = "You or the bot does not meet the required preconditions.";
                    break;
                default:
                    title = "An error occurred";
                    description = "An error occurred while trying to run this command.";
                    break;
            }

            /*            var error = new cweembedbuilder()
                            .withtitle(title)
                            .withdescription(description)
                            .withstyle(embedstyle.error)
                            .build();

                        await context.channel.sendmessageasync(embed: error);*/

        }

        private async Task OnMessageReceived(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message))
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            var argPos = 0;
            if (!message.HasStringPrefix(this.configuration["Prefix"], ref argPos) && !message.HasMentionPrefix(this.client.CurrentUser, ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(this.client, message);
            await this.service.ExecuteAsync(context, argPos, this.provider);
        }
        private async Task OnReadyAsync()
        {
            if (!lavanode.IsConnected)
            {
                await lavanode.ConnectAsync();
            }
        }
    }  
}
