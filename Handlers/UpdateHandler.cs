using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ThiefVladislavBot.Controllers;
using ThiefVladislavBot.Services;

namespace ThiefVladislavBot.Handlers
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly UserService _userService;
        private readonly MessageService _msgService;
        private readonly GameService _gameService;

        public UpdateHandler(UserService userService,
                             MessageService msgService,
                             GameService gameService)
        {
            _userService = userService;
            _msgService = msgService;
            _gameService = gameService;
        }

        public UpdateType[] AllowedUpdates => new[] {
            UpdateType.Message,
            //UpdateType.CallbackQuery
        };

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                var warningMessages = new[] { "bot was blocked by the user", "bot was kicked from the supergroup", "have no rights to send a message" };

                if (warningMessages.Any(x => apiRequestException.Message.Contains(x)))
                {
                    Log.Warning(apiRequestException.Message);
                }
                else
                {
                    Log.Error(apiRequestException, apiRequestException.Message);
                }

                return Task.CompletedTask;
            }

            Log.Error(exception, exception.Message);
            return Task.CompletedTask;
        }

        public Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            Task task = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(bot, update.Message),
                _ => Task.CompletedTask
            };
            return task;

            async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
            {
                Task<Message> action;
                long idSender = message.From.Id;
                long idChat = message.Chat.Id;
                if (_userService.Get(idSender, idChat) == null)
                {
                    _userService.Create(message.From, message.Chat.Id);
                    var newGame = _gameService.Create(message.From.Id, message.Chat.Id);
                    _msgService.Create(message.From.Id, message.Chat.Id, message.Text);

                    Console.WriteLine($"{DateTime.Now:G} {message.From.FirstName} {message.From.LastName} @{message.From.Username} has joined to us!");
                    var gameController = new GameController(newGame, idSender, idChat, _gameService.Get(idSender, idChat).Id);
                    action = SendReplyKeyboard(botClient, message, gameController.StartGame());
                    _gameService.Update(idSender, idChat, gameController.ToGameModel());
                }
                else if (_gameService.Get(idSender, idChat).IsOver)
                {
                    var newGame = _gameService.Create(message.From.Id, message.Chat.Id);
                    _msgService.Create(message.From.Id, message.Chat.Id, message.Text);

                    Console.WriteLine($"{DateTime.Now:G} {message.From.FirstName} {message.From.LastName} @{message.From.Username} has started new game");
                    var gameController = new GameController(newGame, idSender, idChat, _gameService.Get(idSender, idChat).Id);
                    action = SendReplyKeyboard(botClient, message, gameController.StartGame());
                    _gameService.Update(idSender, idChat, gameController.ToGameModel());
                }
                else
                {
                    var gameContinuation = new GameController(_gameService.Get(idSender, idChat), idSender, idChat, _gameService.Get(idSender, idChat).Id);
                    action = SendReplyKeyboard(botClient, message, gameContinuation.NextTurn(message));
                    _msgService.Create(message.From.Id, message.Chat.Id, message.Text);
                    _gameService.Update(idSender, idChat, gameContinuation.ToGameModel());
                }

                if (message.Type != MessageType.Text)
                {
                    Console.WriteLine($"{DateTime.Now:G} Receive message type: {message.Type}");
                    Console.WriteLine($"{DateTime.Now:G} The message was sent by: @{message.Chat.Username}");
                    return;
                }
                var sentMessage = await action;
                if (sentMessage.Text != null)
                    Console.WriteLine($"{DateTime.Now:G} Sent to: @{message.Chat.Username} - '{sentMessage.Text.Substring(0, sentMessage.Text.Length > 20 ? 20 : sentMessage.Text.Length)}'");
                else if (sentMessage.Sticker != null)
                    Console.WriteLine($"{DateTime.Now:G} Sent to: @{message.Chat.Username} - 'Sticker: {sentMessage.Sticker.FileId.Substring(0, sentMessage.Sticker.FileId.Length > 20 ? 20 : sentMessage.Sticker.FileId.Length)}'");

                static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message, Tuple<string, ReplyKeyboardMarkup, string> toSend)
                {
                    if (toSend.Item1 == "")
                    {
                        Console.WriteLine($"{DateTime.Now:G} Bad ending by: @{message.Chat.Username}");
                        return await botClient.SendStickerAsync(chatId: message.Chat.Id, sticker: "https://cdn.tlgrm.app/stickers/d06/e20/d06e2057-5c13-324d-b94f-9b5a0e64f2da/192/2.webp", replyMarkup: toSend.Item2);
                    }
                    if (toSend.Item3 != "https://www.amigoss.eu/wp-content/uploads/2019/01/Amigos_mockup_pyzy_z_miesem_1.png")
                        await botClient.SendStickerAsync(chatId: message.Chat.Id, sticker: toSend.Item3);
                    else
                    {
                        Console.WriteLine($"{DateTime.Now:G} Good ending by:@ {message.Chat.Username}");
                        await botClient.SendPhotoAsync(chatId: message.Chat.Id, photo: "https://www.amigoss.eu/wp-content/uploads/2019/01/Amigos_mockup_pyzy_z_miesem_1.png");
                    }

                    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: toSend.Item1,
                                                                replyMarkup: toSend.Item2);
                }

                static async Task<Message> RemoveKeyboard(ITelegramBotClient botClient, Message message)
                {
                    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: "Removing keyboard",
                                                                replyMarkup: new ReplyKeyboardRemove());
                }

            }

        }
    }
}
