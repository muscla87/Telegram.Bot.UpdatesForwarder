# Telegram Bot Updates Forwarder
.NET Core CLI Tool to forward Telegram bot updates to a local server for development purposes. Run the tool with your bot token and the url of the service that shall receive bot updates.

## Get started

[Install .NET 3.1 or newer](https://get.dot.net) and run this command:

```
dotnet tool install --global Telegram.Bot.UpdatesForwarder
```

Run the server passing the two required parameters

```
telegram-forwarder -t "<TELEGRAM_BOT_TOKEN>" -u "<UPDATES_DESTINATION_URL>"
```
