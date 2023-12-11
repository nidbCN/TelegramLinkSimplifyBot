using Microsoft.Extensions.Logging;
using System.Reflection;
using TelegramLinkSimplifyBot.Plugin;

namespace TelegramLinkSimplifyBot.Core;
public class MessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IList<ISimplifier> _simplifiers;

    public MessageService(ILogger<MessageService> logger)
    {
        _logger = logger;
        _simplifiers = PluginUtils.LoadAddDll(new[] { @"Plugins" });

        _logger.LogInformation("Success load {cnt} simplifiers.", _simplifiers.Count);
    }

    public async Task<string> HandleUpdate(string text)
    {
        var isUri = Uri.TryCreate(text, UriKind.Absolute, out var uri);

        if (isUri)
        {
            var simplifier = _simplifiers
                .SelectMany(plugin => plugin.SimplifyMethods
                    .Where(pair => pair.Key == uri!.Host)
                    .Select(_ => plugin))
                .FirstOrDefault();

            if (simplifier is null)
            {
                return "No plugin can hold this url";
            }

            (var success, var messsage, var result) = await simplifier.SimplifyMethods[uri!.Host](uri!);

            if (success)
            {
                return $"`{result}`\nBy plugin {simplifier.Name} {simplifier.Verison}.";
            }

            return $"";
        }

        // commands
        return text switch
        {
            "/start" => "Welcome! Please send a url to me.",
            _ => "UnKnow command."
        };
    }
}
