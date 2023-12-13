using Microsoft.Extensions.Logging;
using TelegramLinkSimplifyBot.Core.Models;
using TelegramLinkSimplifyBot.Plugin;

namespace TelegramLinkSimplifyBot.Core;

public class MessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IList<ISimplifier> _simplifiers;

    public IDictionary<string, IList<string>> HostInfo =>
        (IDictionary<string, IList<string>>)
            _simplifiers.ToDictionary(
            simp => $"{simp.Name} {simp.Version}",
            simp => simp.SimplifyMethods.Keys.ToList());

    public MessageService(ILogger<MessageService> logger)
    {
        _logger = logger;
        _simplifiers = PluginUtils.LoadAddDll(new[] { @"Plugins\TelegramLinkSimplifyBot.Plugins.Builtin.dll" });

        _logger.LogInformation("Success load {cnt} simplifiers.", _simplifiers.Count);
    }

    public async Task<SimplifyResult> SimplifyWithPlugins(Uri originUrl)
    {
        // 第一个可用的简化器
        var simplifier = _simplifiers
            .SelectMany(plugin => plugin.SimplifyMethods
                .Where(pair => pair.Key == originUrl!.Host)
                .Select(_ => plugin))
            .FirstOrDefault();

        if (simplifier is null)
        {
            return new SimplifyResult(false)
            {
                Url = originUrl,
                Message = "No plugin can hold this url.",
                Plugin = "null",
            };
        }

        _logger.LogInformation("Mathch simplifier {name} for {host}", simplifier.Name, originUrl.Host);

        (var success, var messsage, var result) = await
            simplifier.SimplifyMethods[originUrl!.Host](originUrl!);

        return new SimplifyResult(success)
        {
            Url = result,
            Message = messsage,
            Plugin = $"{simplifier.Name} {simplifier.Version}",
        };
    }
}
