namespace TelegramLinkSimplifyBot.Plugin;

public interface ISimplifier
{
    /// <summary>
    /// Plugin name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Plugin version.
    /// </summary>
    public string Verison { get; }

    /// <summary>
    /// 
    /// </summary>
    public IDictionary<string, Func<Uri, Task<(bool, string?, Uri?)>>> SimplifyMethods { get; }

}
