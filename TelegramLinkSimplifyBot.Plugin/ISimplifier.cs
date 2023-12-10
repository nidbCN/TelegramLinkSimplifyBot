namespace TelegramLinkSimplifyBot.Plugin;

public interface ISimplifier
{
    public string Name { get; }
    public string Verison { get; }
    public IDictionary<string, Func<Uri, Task<(bool, string, Uri)>>> SimplifyMethods { get; }
    public IList<string> HandleHosts { get; }

    /// <summary>
    /// Simplify input url.
    /// </summary>
    /// <param name="url">Uri to simplify.</param>
    /// <param name="message">If success, return url string, or error message.</param>
    /// <returns>Simplify result.</returns>
    public Task<bool> HandleSimplify(Uri url, out string message);
}
