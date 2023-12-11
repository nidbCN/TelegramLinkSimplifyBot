namespace TelegramLinkSimplifyBot.Plugin.Extensions;

public static class TypeExtension
{
    public static bool IsTypeOf<T>(this Type type)
        => typeof(T).IsAssignableFrom(type);
}
