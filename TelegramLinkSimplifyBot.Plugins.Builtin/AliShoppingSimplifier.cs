using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLinkSimplifyBot.Plugin;

namespace TelegramLinkSimplifyBot.Plugins.Builtin;
public class AliShoppingSimplifier : ISimplifier
{
    public string Name => "AliShoppingSimplifier";

    public string Version => "6.0.0";

    public IDictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>> SimplifyMethods => throw new NotImplementedException();
}
