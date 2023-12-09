namespace TelegramLinkSimplifyBot.Core;
public static class UpdateMethods
{
    public static async Task<string> OnReceivedText(string message)
    {
        if(message == "/start")
        {
            return await Task.Run(() => "");
        }


    }

}
