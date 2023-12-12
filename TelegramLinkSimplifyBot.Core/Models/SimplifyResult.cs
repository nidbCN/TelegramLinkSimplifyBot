namespace TelegramLinkSimplifyBot.Core.Models;

public class SimplifyResult
{
    public SimplifyResult(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public SimplifyResult(bool isSuccess, Uri url)
    {
        IsSuccess = isSuccess;
        Url = url;
    }

    public bool IsSuccess { get; set; }

    public Uri? Url { get; set; }

    public string? Plugin { get; set; }

    public string? Message { get; set; }
}
