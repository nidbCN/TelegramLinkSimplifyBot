using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLinkSimplifyBot.Plugin;
internal class Bilibili
{
    public async string Test(Uri url)
    {
        using var client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false })
        {
            BaseAddress = url
        };

        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");

        var resp = await client.GetAsync(url);
        if (resp.StatusCode == System.Net.HttpStatusCode.Redirect)
        {
            var newUrl = resp.Headers.Location;
            return new Uri(newUrl, newUrl.AbsolutePath).ToString();
        }

        return string.Empty;
    }
}
