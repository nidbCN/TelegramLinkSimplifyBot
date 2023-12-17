using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramLinkSimplifyBot.Plugin;

namespace TelegramLinkSimplifyBot.Plugins.Builtin.Simplifiers;
internal class TwitterSimplifier : ISimplifier
{
    public string Name => "TwitterSimplifier";

    public string Version => "6.0.0";

    public IDictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>> SimplifyMethods
        => new Dictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>>()
        {
            {"twitter.com", SimplifyNormalUrl }
        };

    private async Task<(bool, string?, Uri?)> SimplifyNormalUrl(Uri origin)
    {
        var cleardUrl = await Task.Run(() => new Uri(origin, origin.AbsolutePath));

        return (true, null, cleardUrl);
    }
}