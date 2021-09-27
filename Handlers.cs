using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ThiefVladislavBot
{
    public class Handlers
    {
        private static Dictionary<long, Game> sessions = new Dictionary<long, Game>();

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Task<Message> action;
            long idSender = message.From.Id;
            if (!sessions.ContainsKey(idSender))
            {
                sessions.Add(idSender, new Game());
                Console.WriteLine($"{DateTime.Now:G} {message.From.FirstName} {message.From.LastName} @{message.From.Username} started new game");
                action = SendReplyKeyboard(botClient, message, sessions[idSender].StartGame());
            }
            else if (!sessions[idSender].IsOver)
            {
                action = SendReplyKeyboard(botClient, message, sessions[idSender].NextTurn(message));
            }
            else
            {
                sessions[idSender] = new Game();
                action = SendReplyKeyboard(botClient, message, sessions[idSender].StartGame());
            }

            if (message.Type != MessageType.Text)
            {
                Console.WriteLine($"{DateTime.Now:G} Receive message type: {message.Type}");
                Console.WriteLine($"{DateTime.Now:G} The message was sent to: @{message.Chat.Username}");
                return;
            }
            var sentMessage = await action;
            if (sentMessage.Text != null)
                Console.WriteLine($"{DateTime.Now:G} Sent to: @{message.Chat.Username} - '{sentMessage.Text.Substring(0, sentMessage.Text.Length > 20 ? 20 : sentMessage.Text.Length)}'");
            else if(sentMessage.Sticker != null)
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

            static async Task<Message> SendFile(ITelegramBotClient botClient, Message message)
            {
                await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string filePath = @"Files/tux.png";
                using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                return await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                                                      photo: new InputOnlineFile(fileStream, fileName),
                                                      caption: "Nice Picture");
            }
        }

        // Process Inline Keyboard callback data
        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}");

            await botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}");
        }

        private static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
        {
            Console.WriteLine($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };

            await botClient.AnswerInlineQueryAsync(
                inlineQueryId: inlineQuery.Id,
                results: results,
                isPersonal: true,
                cacheTime: 0);
        }

        private static Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}