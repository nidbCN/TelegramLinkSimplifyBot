using System.Reflection;
using TelegramLinkSimplifyBot.Plugin;

namespace TelegramLinkSimplifyBot.Core;
public class Simplifier
{
    private readonly IList<ISimplifier> _simplifiers;

    public Simplifier()
    {
        string[] pluginPaths = new string[]
       {
           "Plugins"
           // Paths to plugins to load.
       };

        _simplifiers = pluginPaths.SelectMany(pluginPath =>
        {
            Assembly pluginAssembly = LoadPlugin(pluginPath);
            return CreateSimplifier(pluginAssembly);
        }).ToArray();
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
        throw new NotImplementedException();
    }

    static IEnumerable<ISimplifier> CreateSimplifier(Assembly assembly)
    {
        int count = 0;

        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(ISimplifier).IsAssignableFrom(type))
            {
                if (Activator.CreateInstance(type) is ISimplifier result)
                {
                    count++;
                    yield return result;
                }
            }
        }

        if (count == 0)
        {
            string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            throw new ApplicationException(
                $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                $"Available types: {availableTypes}");
        }
    }
}
