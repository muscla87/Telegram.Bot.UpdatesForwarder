using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using System.Net.Http;

namespace Telegram.Bot.UpdatesForwarder
{
    public class MessageForwarder
    {
        public string BotToken { get; set; }
        public string Url { get; set; }

        public MessageForwarder(string botToken, string url)
        {
            BotToken = botToken;
            Url = url;
        }

        public Task<bool> IsTokenValid()
        {
            var client = new TelegramBotClient(BotToken);
            return client.TestApiAsync();
        }

        public async void StartForwarding(CancellationToken cancellationToken)
        {
            var botClient = new TelegramBotClient(BotToken);

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };
            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cancellationToken);
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received {update.Type} #{update.Id}");
            HttpClient client = new HttpClient();
            var updateContent = JsonConvert.SerializeObject(update);
            var buffer = System.Text.Encoding.UTF8.GetBytes(updateContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            try
            {
                var stopwatch = Stopwatch.StartNew();
                await client.PostAsync(Url, byteContent);
                Console.WriteLine($"Forwarded {update.Type} #{update.Id} in {stopwatch.ElapsedMilliseconds}ms");

            }
            catch (Exception exception)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException => $"Telegram API Error: [{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    HttpRequestException httpRequestException => $"Could not forward received {update.Type} #{update.Id}. Error: {httpRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}