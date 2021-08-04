using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweazyHut.Modules
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Fields used via inheritance.")]
    public abstract class SweazyHutModuleBase : ModuleBase<SocketCommandContext>
    {
        public async Task<int> GetRandomInt()
        {
            Random ran = new Random();
            int num = 0;
            await Task.Run(() => {
                num = ran.Next(1, 3);
            }).ConfigureAwait(false);
            return num;
        }
    }
}
