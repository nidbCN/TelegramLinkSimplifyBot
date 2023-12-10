using System.Reflection;
using System.Runtime.Loader;

namespace TelegramLinkSimplifyBot.Core.Context;

class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
        => _resolver.ResolveAssemblyToPath(assemblyName) is string path
            ? LoadFromAssemblyPath(path)
            : null;

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        => _resolver.ResolveUnmanagedDllToPath(unmanagedDllName) is string path
            ? LoadUnmanagedDllFromPath(path)
            : IntPtr.Zero;
}
