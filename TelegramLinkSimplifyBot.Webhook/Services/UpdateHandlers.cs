using System.Reflection.Metadata.Ecma335;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramLinkSimplifyBot.Core;

namespace Telegram.Bot.Services;

public class UpdateHandlers
{
    private readonly ITelegramBotClient _botClient;
    private readonly MessageService _messageService;
    private readonly ILogger<UpdateHandlers> _logger;

    public UpdateHandlers(ITelegramBotClient botClient, MessageService messageService, ILogger<UpdateHandlers> logger)
    {
        _botClient = botClient;
        _logger = logger;
        _messageService = messageService;
    }

    public Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
            { EditedMessage: { } message } => BotOnMessageReceived(message, cancellationToken),
            _ => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);
        if (message.Text is not { })
            return;

        if (message.Text == "/start")
        {
            await SendStart(_botClient, message, cancellationToken);
            return;
        }
        else if (message.Text == "/info")
        {
            await SendInfo(_botClient, message, cancellationToken);
            return;
        }

        var SPLIT_CHAR = new[] { ' ', ';', ',' };

        var originUrl = message.Text;

        // has keyword http
        var startIndex = message.Text.IndexOf("http");
        if (startIndex != -1)
        {
            // foreach the text and find split char.
            var skipCount = originUrl
                .Skip(startIndex)
                .TakeWhile(ch => !SPLIT_CHAR.Contains(ch))
                .Count();

            originUrl = originUrl.Substring(startIndex, skipCount);
        }

        _logger.LogInformation("Receive url {url} from {id}.", originUrl, message.Chat.Id);

        if (!Uri.TryCreate(originUrl, UriKind.Absolute, out var url))
        {
            const string usage = "Unknow command or link, please send me a url";

            await _botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: usage.Replace(".", @"\."),
               replyMarkup: new ReplyKeyboardRemove(),
               cancellationToken: cancellationToken);

            return;
        }
        else
        {
            await SendLink(_botClient, message, cancellationToken);
            return;
        }

        async Task SendStart(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            const string usage =
                "*Welcome to use simplify bot*, this bot can remove tracking info in your link\n"
                + "\t`/start`: Show this welcome message\n"
                + "\t.`/info`: Show current plugins";

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: usage.Replace(".", @"\."),
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        async Task SendInfo(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var avaliableHosts = _messageService.HostInfo;
            var avaliableHostText = string.Join("\n",
                avaliableHosts.Select((info) => $"{info.Key}\t\n"
                    + string.Join("\t\n", info.Value.Select(host => $"`{host}`"
                    ))));

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: avaliableHostText.Replace(".", @"\."),
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        async Task SendLink(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var result = await _messageService.SimplifyWithPlugins(url!);

            if (result.IsSuccess)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"`{result.Url}`\nBy {result.Plugin}".Replace(".", @"\."),
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithUrl(
                            text: "Open link",
                            url: result.Url!.AbsoluteUri)),
                    cancellationToken: cancellationToken);
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"`Failed! Message {result.Message}`\nBy {result.Plugin}".Replace(".", @"\."),
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
            }
        }
    }

    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
