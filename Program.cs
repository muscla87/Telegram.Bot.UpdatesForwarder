using System;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace Telegram.Bot.UpdatesForwarder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<CliOptions>(args)
                               .WithParsedAsync<CliOptions>(StartBot);
        }

        private async static Task StartBot(CliOptions arg)
        {
            if (arg.BotToken == null)
            {
                System.Console.WriteLine("Bot token is not specified");
                return;
            }
            if (arg.Url == null)
            {
                System.Console.WriteLine("Destination URL is not specified");
                return;
            }

            using var cts = new CancellationTokenSource();
            var forwarder = new MessageForwarder(arg.BotToken, arg.Url);

            if (await forwarder.IsTokenValid())
            {
                forwarder.StartForwarding(cts.Token);
                Console.ReadLine();
                cts.Cancel();
            }
            else
            {
                System.Console.WriteLine("Bot token is invalid");
            }
        }
    }
}
