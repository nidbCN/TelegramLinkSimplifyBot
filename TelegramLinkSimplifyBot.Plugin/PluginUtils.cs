using Microsoft.Extensions.Logging;
using System.Reflection;
using TelegramLinkSimplifyBot.Plugin.Context;
using TelegramLinkSimplifyBot.Plugin.Extensions;

namespace TelegramLinkSimplifyBot.Plugin;
public static class PluginUtils
{
    public static IList<ISimplifier> LoadAllDll(string path)
        => CreateSimplifier(LoadPlugin(path)).ToArray();

    public static IList<ISimplifier> LoadAddDll(params string[] path)
        => path.SelectMany(pluginPath =>
             CreateSimplifier(LoadPlugin(pluginPath))
        ).ToArray();

    static Assembly LoadPlugin(string relativePath, ILogger? logger = null)
    {
        var root = Directory.GetCurrentDirectory();

        var pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));

        logger?.LogInformation("Loading plugins from: {location}", pluginLocation);

        var loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(
            new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }

    static IEnumerable<ISimplifier> CreateSimplifier(Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(type => type.IsTypeOf<ISimplifier>());

        if (types is null || !types.Any())
        {
            string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            throw new ApplicationException(
                $"Can't find any type which implements {nameof(ISimplifier)} in {assembly} from {assembly.Location}.\n" +
                $"Available types: {availableTypes}");
        }

        var simplifiers = types
            .Select(type => Activator.CreateInstance(type) as ISimplifier)
            .Where(simp => simp is not null)
            .Select(simp => simp!);

        return simplifiers;
    }
}
