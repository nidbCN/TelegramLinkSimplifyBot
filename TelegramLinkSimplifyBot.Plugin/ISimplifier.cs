namespace TelegramLinkSimplifyBot.Plugin;

internal interface ISimplifier
{
    public string Name { get; set; }
    public string Verison { get; set; }
    public IDictionary<string, Func<Task<(bool, string, Uri)>, Uri>> SimplifyMethods { get; set; }
}
