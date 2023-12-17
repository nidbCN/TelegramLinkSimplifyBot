using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using TelegramLinkSimplifyBot.Plugin;

namespace TelegramLinkSimplifyBot.Plugins.Builtin.Simplifiers;
public class AliShoppingSimplifier : ISimplifier
{
    public string Name => "AliShoppingSimplifier";

    public string Version => "6.0.1";

    public IDictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>> SimplifyMethods
        => new Dictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>>()
        {
            {"item.taobao.com", SimplifyNormalTaobaoLink }
        };

    private async Task<(bool, string?, Uri?)> SimplifyNormalTaobaoLink(Uri origin)
    {
        const string QUERY_KEY = "id";

        var cleardUrl = await Task.Run(() => new Uri(origin, origin.AbsolutePath));

        var querys = await Task.Run(() => HttpUtility.ParseQueryString(origin.Query));

        if (querys[QUERY_KEY] is { } queryValue)
        {
            return (true, null, new Uri(cleardUrl, $"?{QUERY_KEY}={queryValue}"));
        }

        return (true, null, cleardUrl);
    }
}
