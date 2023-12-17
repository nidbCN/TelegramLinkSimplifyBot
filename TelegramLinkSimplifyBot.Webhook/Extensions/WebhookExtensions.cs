using Microsoft.Extensions.Options;
namespace TelegramLinkSimplifyBot.Webhook.Extensions;

public static class WebHookExtensions
{
    public static T GetConfiguration<T>(this IServiceProvider serviceProvider)
        where T : class
    {
        var option = serviceProvider.GetService<IOptions<T>>()
            ?? throw new ArgumentNullException(nameof(T));
        return option.Value;
    }

    public static ControllerActionEndpointConventionBuilder MapBotWebhookRoute<T>(
        this IEndpointRouteBuilder endpoints,
        string route)
    {
        var controllerName = typeof(T).Name.Replace("Controller", "", StringComparison.Ordinal);
        var actionName = typeof(T).GetMethods()[0].Name;

        return endpoints.MapControllerRoute(
            name: "bot_webhook",
            pattern: route,
            defaults: new { controller = controllerName, action = actionName });
    }
}
