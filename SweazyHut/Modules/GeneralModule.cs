namespace SweazyHut.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Discord.Commands;

    public class GeneralModule : SweazyHutModuleBase
    {

        [Command("ping")]
        public async Task Ping()
        {
            await this.ReplyAsync($"Pong! 🏓 `{this.Context.Client.Latency}ms`");
        }
        [Command("coinflip")]
        [Alias("cf")]
        public async Task coinFlip()
        {
            if (await GetRandomInt() == 1)
                await Context.Channel.SendFileAsync("Images/bitcoin1.png");
            else
                await Context.Channel.SendFileAsync("Images/bitcoin2.png");
        }
    }
}
