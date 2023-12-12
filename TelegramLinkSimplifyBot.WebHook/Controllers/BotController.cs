using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Filters;
using Telegram.Bot.Polling;
using Telegram.Bot.Services;
using Telegram.Bot.Types;
using TelegramLinkSimplifyBot.Core;

namespace Telegram.Bot.Controllers;

public class BotController : ControllerBase
{
    private readonly UpdateHandlers _updateHandler;
    
    public BotController(UpdateHandlers updateHandler)
    {
        _updateHandler = updateHandler;
    }

    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        CancellationToken cancellationToken)
    {
        await _updateHandler.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }
}
