﻿using System.Web;

namespace TelegramLinkSimplifyBot.Plugin.Bilibili;
public class BilibiliSimplifier : ISimplifier
{
    public string Name => "Bilibili Simplifier";

    public string Verison => "6.0.0.2";

    public IDictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>> SimplifyMethods
        => new Dictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>>()
        {
            {"b23.tv", SimplifyShortUrl },
            {"www.bilibili.com", SimplifyNormalUrl },
            {"live.bilibili.com", SimplifyLiveUrl },
        };

    private readonly HttpClient _client;

    public BilibiliSimplifier()
    {
        _client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false });
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
    }

    private async Task<(bool, string?, Uri?)> SimplifyShortUrl(Uri origin)
    {
        var resp = await _client.GetAsync(origin);
        if (resp.StatusCode == System.Net.HttpStatusCode.Redirect)
        {
            var redirectUrl = resp.Headers.Location;
            if (redirectUrl?.Host == "www.bilibili.com")
                return await SimplifyNormalUrl(redirectUrl);
            else
                return (false, $"Unexcept redirect host {redirectUrl?.Host}.", null);
        }

        return (false, $"Failed to fetch b23.tv, HTTP {resp.StatusCode}.", null);
    }

    private async Task<(bool, string?, Uri?)> SimplifyNormalUrl(Uri origin)
    {
        var cleardUrl = await Task.Run(() => new Uri(origin, origin.AbsolutePath));

        var querys = await Task.Run(() => HttpUtility.ParseQueryString(origin.Query));

        if (querys["t"] != null)
        {
            return (true, null, new Uri(cleardUrl, querys["t"]));
        }

        return (true, null, cleardUrl);
    }

    private async Task<(bool, string?, Uri?)> SimplifyLiveUrl(Uri origin)
    {
        if (origin is null)
        {
            throw new ArgumentNullException(nameof(origin));
        }

        var url = await Task.Run(() => new Uri(origin, origin.AbsolutePath));

        return (true, null, url);
    }
}
