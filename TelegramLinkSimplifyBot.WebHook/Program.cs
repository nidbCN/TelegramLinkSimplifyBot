using Telegram.Bot;
using Telegram.Bot.Controllers;
using Telegram.Bot.Services;
using TelegramLinkSimplifyBot.Webhook.Extensions;
using TelegramLinkSimplifyBot.Webhook.Configs;

var builder = WebApplication.CreateBuilder(args);

// Setup Bot configuration
var botConfig = builder.Configuration.GetSection(nameof(BotConfig));
var tokenConfig = builder.Configuration.GetSection(nameof(TokenConfig));
builder.Services.Configure<BotConfig>(botConfig);
builder.Services.Configure<TokenConfig>(tokenConfig);

builder.Services
    .AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
    {
        var tokenConfig = serviceProvider.GetConfiguration<TokenConfig>();
        var options = new TelegramBotClientOptions(tokenConfig.BotToken);

        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddScoped<UpdateHandlers>();
builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

app.MapBotWebhookRoute<BotController>(route: botConfig[nameof(BotConfig.Route)]);
app.MapControllers();
app.Run();
