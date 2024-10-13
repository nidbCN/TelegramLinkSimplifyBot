using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TelegramLinkSimplifyBot.Plugin;

namespace TelegramLinkSimplifyBot.Plugins.Builtin.Simplifiers;
public class JingDongSimplifier : ISimplifier
{
    public string Name => "JDSimplifier";

    public string Version => "6.0.1";

    private readonly HttpClient _client;

    public IDictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>> SimplifyMethods
        => new Dictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>>
        {
            {"3.cn", SimplifyShortUrl},
            {"item.jd.com", SimplifyNormalUrl },
            {"item.m.jd.com", SimplifyNormalUrl},
        };

    public JingDongSimplifier()
    {
        _client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
    }

    private async Task<(bool, string?, Uri?)> SimplifyNormalUrl(Uri origin)
    {
        var clearedUrl = await Task.Run(() => new Uri(origin, origin.AbsolutePath));

        return (true, null, clearedUrl);
    }

    private async Task<(bool, string?, Uri?)> SimplifyShortUrl(Uri origin)
    {
        var resp = await _client.GetAsync(origin);

        if (resp.StatusCode != System.Net.HttpStatusCode.Redirect)
            return (false, $"Failed to fetch 3.cn, HTTP {resp.StatusCode}.", null);

        var redirectUrl = resp.Headers.Location;

        if (redirectUrl is not null
            && SimplifyMethods.ContainsKey(redirectUrl.Host))
            return await SimplifyNormalUrl(redirectUrl);

        return (false, $"UnExcept redirect host {redirectUrl?.Host}.", null);
    }
}
