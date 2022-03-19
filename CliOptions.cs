using CommandLine;

namespace Telegram.Bot.UpdatesForwarder
{
    public class CliOptions
    {
        [Option('t', "token", Required = true, HelpText = "Telegram bot token")]
        public string? BotToken { get; set; }

        [Option('u', "url", Required = true, HelpText = "Destination URL to forward messages to")]
        public string? Url { get; set; }
    }
}