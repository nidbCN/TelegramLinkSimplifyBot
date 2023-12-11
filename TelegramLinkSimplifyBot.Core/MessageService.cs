using System.Reflection;
using System.Windows.Input;
using TelegramLinkSimplifyBot.Core.Context;
using TelegramLinkSimplifyBot.Plugin;

namespace TelegramLinkSimplifyBot.Core;
public class MessageService
{
    private readonly IList<ISimplifier> _simplifiers;

    public MessageService()
    {
        string[] pluginPaths = new string[]
       {
           @"Plugins/TelegramLinkSimplifyBot.Plugins.Bilibili.dll"
           // Paths to plugins to load.
       };

        _simplifiers = pluginPaths.SelectMany(pluginPath =>
        {
            Assembly pluginAssembly = LoadPlugin(pluginPath);
            return CreateSimplifier(pluginAssembly);
        }).ToArray();

        Console.WriteLine();
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

    static Assembly LoadPlugin(string relativePath)
    {
        // Navigate up to the solution root


        var root = Directory.GetCurrentDirectory();

        string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
        Console.WriteLine($"Loading commands from: {pluginLocation}");
        PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));

    }

    static IEnumerable<ISimplifier> CreateSimplifier(Assembly assembly)
    {
        int count = 0;

        var types = assembly.GetTypes()
            .Where(type => type.Name.Contains("Simplifier"))
            .FirstOrDefault();

        if(types is not null)
        {
            var isAssignable = typeof(ISimplifier).IsAssignableFrom(types);
            Console.WriteLine(isAssignable);
        }

        return null;

        //foreach (Type type in types)
        //{
        //    ISimplifier result = Activator.CreateInstance(type) as TelegramLinkSimplifyBot.Plugin.ISimplifier;
        //    if (result != null)
        //    {
        //        count++;
        //        yield return result;
        //    }

        //}

        if (count == 0)
        {
            string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            throw new ApplicationException(
                $"Can't find any type which implements {nameof(ISimplifier)} in {assembly} from {assembly.Location}.\n" +
                $"Available types: {availableTypes}");
        }
    }
}
