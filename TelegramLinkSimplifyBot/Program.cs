using TelegramLinkSimplifyBot;
using TelegramLinkSimplifyBot.Configs;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddUserSecrets<Program>();

builder.Services
    .Configure<AppSecret>(builder.Configuration.GetSection(nameof(AppSecret)))
    .AddHostedService<Worker>();

await builder.Build().RunAsync();
