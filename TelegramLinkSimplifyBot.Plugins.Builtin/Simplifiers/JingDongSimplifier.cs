using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLinkSimplifyBot.Plugin;

namespace TelegramLinkSimplifyBot.Plugins.Builtin.Simplifiers;
public class JingDongSimplifier : ISimplifier
{
    public string Name => "JDSimplifier";

    public string Version => "6.0.1";

    public IDictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>> SimplifyMethods
        => new Dictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>>()
        {
            {"item.jd.com", SimplifyNormalUrl },
            {"item.m.jd.com", SimplifyNormalUrl},
        };

    private async Task<(bool, string?, Uri?)> SimplifyNormalUrl(Uri origin)
    {
        var cleardUrl = await Task.Run(() => new Uri(origin, origin.AbsolutePath));

        return (true, null, cleardUrl);
    }
}
