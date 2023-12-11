using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Filters;
using Telegram.Bot.Types;
using TelegramLinkSimplifyBot.Core;

namespace Telegram.Bot.Controllers;

public class BotController : ControllerBase
{
    private readonly MessageService _messageService;
    
    public BotController(MessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        CancellationToken cancellationToken)
    {
        
        return Ok();
    }
}
