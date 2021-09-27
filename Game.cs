using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ThiefVladislavBot
{
    public class Game
    {
        public List<string> LocationsToGo { get; set; }
        public string LocationRightNow { get; set; }

        public List<string> AllLocations { get; } = new List<string>()
        {
            "Осмотреться",
            "Зал",
            "Туалет",
            "Рискнуть и зайти в туалет",
            "Вернуться в прихожую",
            "Пойти в туалет",
            "Пойти в зал",
            "Застелить постель",
            "Отойти от постели",
            "Расстелить обратно",
            "Снова застелить постель",
            "Пойти на кухню",
            "Выпрыгнуть в окно",
            "Бегом в прихожую",
            "КОНЕЦ.",
            "Бросить в дядю ржавым ключом",
            "Бросить в дядю золотым ключом",
            "Убежать в зал",
            "Убежать в туалет",
            "Вернуться в зал",
            "Вспомнить лучшие моменты жизни",
            "Отойти от холодильника",
            "Попробовать открыть ржавым ключом"
        };

        public bool IsGoldenKey { get; set; }
        public bool DidAttackByGoldenKey { get; set; }
        public bool IsRustyKey { get; set; }
        public bool DidAttackByRustyKey { get; set; }
        public bool IsOver { get; set; }
        public bool IsWCVisited { get; set; }
        public bool IsMadeBed { get; set; }
        public bool IsUnmadeBed { get; set; }
        public bool IsRaidMode { get; set; }
        private string _lastWords = "";
        private string _lastSticker = "";

        public Game()
        {
            LocationsToGo = new List<string>() { "Осмотреться", "Зал", "Туалет" };
            LocationRightNow = "Прихожая";
            IsGoldenKey = false;
            IsRustyKey = false;
            IsOver = false;
            IsWCVisited = false;
            IsMadeBed = false;
            IsUnmadeBed = false;
            DidAttackByRustyKey = false;
            DidAttackByGoldenKey = false;
            IsRaidMode = false;
        }

        private ReplyKeyboardMarkup KeyboardOptimizer(List<string> names)
        {
            int x = 2;
            int y = (int)Math.Ceiling((double)(names.Count) / x);
            int counter = 0;

            KeyboardButton[][] keyboard = new KeyboardButton[y][];

            for (int i = 0; i < keyboard.Length; i++)
            {
                if (i + 1 == keyboard.Length)
                    keyboard[i] = new KeyboardButton[names.Count - counter];
                else
                    keyboard[i] = new KeyboardButton[x];

                for (int j = 0; j < keyboard[i].Length; j++)
                {
                    if (counter < names.Count)
                        keyboard[i][j] = new KeyboardButton();
                    counter++;
                }
            }

            for (int i = 0; i < names.Count; i++)
                keyboard[i / x][i % x] = names[i];

            return new ReplyKeyboardMarkup(keyboard) { ResizeKeyboard = true };
        }

        public Tuple<string, ReplyKeyboardMarkup, string> StartGame()
        {
            string text = "Вы вор Владислав. Вы забрались в квартиру своего дяди в шабанах и стоите в прихожей.";
            _lastWords = text;
            var keyboard = KeyboardOptimizer(LocationsToGo);
            string sticker = "https://tlgrm.ru/_/stickers/4dd/300/4dd300fd-0a89-3f3d-ac53-8ec93976495e/192/115.webp";
            _lastSticker = sticker;
            return new Tuple<string, ReplyKeyboardMarkup, string>(text, keyboard, sticker);
        }

        public Tuple<string, ReplyKeyboardMarkup, string> NextTurn(Message message)
        {
            LocationRightNow = message.Text;
            string textToSay = "";
            string sticker = "https://cdn.tlgrm.app/stickers/cbe/e09/cbee092b-2911-4290-b015-f8eb4f6c7ec4/192/10.webp";

            switch (LocationRightNow)
            {
                case "Осмотреться":
                    textToSay = "Перестань осматриваться, действуй.";
                    LocationsToGo = new List<string>() { "Пойти в туалет", "Пойти в зал" };
                    sticker = "https://tlgrm.ru/_/stickers/8a1/9aa/8a19aab4-98c0-37cb-a3d4-491cb94d7e12/192/59.webp";
                    break;

                case "Пойти в туалет":
                case "Туалет":
                    if (IsWCVisited)
                    {
                        textToSay = "В туалете больше нет ничего ценного. Шорох из трубы кажется уже не таким подозрительным.";
                        LocationsToGo = new List<string>() { "Вернуться в прихожую" };
                        sticker = "https://tlgrm.ru/_/stickers/8a1/9aa/8a19aab4-98c0-37cb-a3d4-491cb94d7e12/192/53.webp";
                        break;
                    }
                    else
                    {
                        textToSay = "Только вы приблизились к двери, как услышали за ней подозрительный шорох. Возможно это ваш дядя и если он вас увидит, то вам несдобровать.";
                        LocationsToGo = new List<string>() { "Рискнуть и зайти в туалет", "Вернуться в прихожую" };
                        sticker = "https://tlgrm.ru/_/stickers/8a1/9aa/8a19aab4-98c0-37cb-a3d4-491cb94d7e12/192/46.webp";
                        break;
                    }
                case "Рискнуть и зайти в туалет":
                    IsWCVisited = true;
                    IsGoldenKey = true;
                    textToSay = "Подозрительным шорохом оказалась течь в проржавевшей трубе. А еще вы нашли золотой ключ.";
                    LocationsToGo = new List<string>() { "Вернуться в прихожую" };
                    sticker = "https://tlgrm.ru/_/stickers/8a1/9aa/8a19aab4-98c0-37cb-a3d4-491cb94d7e12/192/51.webp";
                    break;

                case "Вернуться в прихожую":
                    textToSay = "Вы все еще вор Владислав и стоите в прихожей своего дяди. В шабанах.";
                    LocationsToGo = new List<string>() { "Осмотреться", "Зал", "Туалет" };
                    sticker = "https://tlgrm.ru/_/stickers/8a1/9aa/8a19aab4-98c0-37cb-a3d4-491cb94d7e12/192/52.webp";
                    break;

                case "Вернуться в зал":
                case "Пойти в зал":
                case "Зал":
                    if (IsUnmadeBed || !IsMadeBed)
                    {
                        textToSay = "Вы тихо зашли в зал. Окна завешаны. Постель не заправлена. Дяди нет.";
                        LocationsToGo = new List<string>() { "Пойти на кухню", "Застелить постель", "Вернуться в прихожую" };
                        sticker = "https://cdn.tlgrm.app/stickers/38b/6ab/38b6abb4-854c-39ff-806f-4e6f1f4cd9ff/192/3.webp";
                        break;
                    }
                    else
                    {
                        textToSay = "Вы тихо зашли в зал. Окна завешаны. Постель заправлена. Дяди нет.";
                        LocationsToGo = new List<string>() { "Пойти на кухню", "Расстелить постель", "Вернуться в прихожую" };
                        sticker = "https://tlgrm.ru/_/stickers/38b/6ab/38b6abb4-854c-39ff-806f-4e6f1f4cd9ff/192/29.webp";
                        break;
                    }
                case "Снова застелить постель":
                case "Застелить постель":
                    if (!IsMadeBed)
                    {
                        IsRustyKey = true;
                        IsMadeBed = true;
                        textToSay = "Молодец. Вы застелили дядину постель. А на полу нашли ржавый ключ.";
                        LocationsToGo = new List<string>() { "Отойти от постели", "Расстелить обратно" };
                        sticker = "https://cdn.tlgrm.app/stickers/38b/6ab/38b6abb4-854c-39ff-806f-4e6f1f4cd9ff/192/10.webp";
                        break;
                    }
                    else
                    {
                        textToSay = "Только вы подровнаяли уголки одеяла, как вдруг услышали шум из прихожей.";
                        IsRaidMode = true;
                        LocationsToGo = new List<string>() { "Выпрыгнуть в окно", "Бегом в прихожую" };
                        sticker = "https://tlgrm.ru/_/stickers/8a1/9aa/8a19aab4-98c0-37cb-a3d4-491cb94d7e12/192/44.webp";
                        break;
                    }
                case "Отойти от постели":
                    if (IsMadeBed)
                    {
                        textToSay = "Вы тихо зашли в зал. Окна завешаны. Постель заправлена. Дяди нет.";
                        LocationsToGo = new List<string>() { "Пойти на кухню", "Расстелить постель", "Вернуться в прихожую" };
                        sticker = "https://tlgrm.ru/_/stickers/38b/6ab/38b6abb4-854c-39ff-806f-4e6f1f4cd9ff/192/29.webp";
                    }
                    if (IsUnmadeBed)
                    {
                        textToSay = "Вы тихо зашли в зал. Окна завешаны. Постель не заправлена. Дяди нет.";
                        LocationsToGo = new List<string>() { "Пойти на кухню", "Застелить постель", "Вернуться в прихожую" };
                        sticker = "https://cdn.tlgrm.app/stickers/38b/6ab/38b6abb4-854c-39ff-806f-4e6f1f4cd9ff/192/3.webp";
                    }
                    break;

                case "Расстелить обратно":
                case "Расстелить постель":
                    textToSay = "Вы вернули постель в прежнее состояние.";
                    IsUnmadeBed = true;
                    LocationsToGo = new List<string>() { "Снова застелить постель", "Отойти от постели" };
                    sticker = "https://cdn.tlgrm.app/stickers/844/c8e/844c8ee0-e7d3-4c34-90cb-e48c466b2315/192/6.webp";
                    break;

                case "Выпрыгнуть в окно":
                    textToSay = "Вы не взяли во внимание, что дядя живет на 6ом этаже. Вы разбились и умерли.";
                    LocationsToGo = new List<string>() { "КОНЕЦ." };
                    sticker = "https://tlgrm.ru/_/stickers/4dd/300/4dd300fd-0a89-3f3d-ac53-8ec93976495e/192/66.webp";
                    break;

                case "Бегом в прихожую":
                    textToSay = "Оказавшись в прихожей вы увидели только что вернувшегося дядю.\nКажется он вас видит...";
                    if (IsRustyKey)
                        LocationsToGo = new List<string>() { "Убежать в зал", "Убежать в туалет", "Бросить в дядю ржавым ключом" };
                    if (IsGoldenKey)
                        LocationsToGo = new List<string>() { "Убежать в зал", "Убежать в туалет", "Бросить в дядю ржавым ключом", "Бросить в дядю золотым ключом" };
                    sticker = "https://cdn.tlgrm.app/stickers/e3a/27d/e3a27d19-e86a-4932-9a27-1bbcca58d75a/192/1.webp";
                    break;

                case "Убежать в зал":
                    textToSay = "У вас нет времени на раздумья, надо действовать.";
                    LocationsToGo = new List<string>() { "Выпрыгнуть в окно", "Бегом на кухню" };
                    sticker = "https://tlgrm.ru/_/stickers/8a1/9aa/8a19aab4-98c0-37cb-a3d4-491cb94d7e12/192/69.webp";
                    break;

                case "Убежать в туалет":
                case "Бросить в дядю ржавым ключом и убежать в туалет":
                case "Бросить в дядю золотым ключом и убежать в туалет":
                    textToSay = "Дядя медленно зашел в туалет. Медленно закрыл дверь. Медленно достал дробовик. И быстро распределил ваши внутренности по кафельной плитке.";
                    LocationsToGo = new List<string>() { "КОНЕЦ." };
                    sticker = "https://tlgrm.ru/_/stickers/e3a/27d/e3a27d19-e86a-4932-9a27-1bbcca58d75a/192/20.webp";
                    break;

                case "Бросить в дядю ржавым ключом":
                    textToSay = "Теперь он вас точно заметил. Браво!";
                    DidAttackByRustyKey = true;
                    LocationsToGo = new List<string>() { "Убежать в зал", "Убежать в туалет" };
                    if (IsGoldenKey)
                        LocationsToGo = new List<string>() { "Убежать в зал", "Убежать в туалет", "Бросить в дядю золотым ключом и убежать в туалет" };
                    sticker = "https://tlgrm.ru/_/stickers/8a1/9aa/8a19aab4-98c0-37cb-a3d4-491cb94d7e12/192/70.webp";
                    break;

                case "Бросить в дядю золотым ключом":
                    DidAttackByGoldenKey = true;
                    textToSay = "Теперь он вас точно заметил. Браво!";
                    LocationsToGo = new List<string>() { "Убежать в зал", "Убежать в туалет", "Бросить в дядю ржавым ключом и убежать в туалет" };
                    sticker = "https://tlgrm.ru/_/stickers/8a1/9aa/8a19aab4-98c0-37cb-a3d4-491cb94d7e12/192/66.webp";
                    break;

                case "Пойти на кухню":
                case "Отойти от холодильника":
                    textToSay = "Перед вами холодильник с замком.";
                    LocationsToGo = new List<string>() { "Попробовать открыть", "Вернуться в зал" };
                    if (IsGoldenKey)
                        LocationsToGo = new List<string>() { "Попробовать открыть золотым ключом", "Вернуться в зал" };
                    if (IsRustyKey)
                        LocationsToGo = new List<string>() { "Попробовать открыть ржавым ключом", "Вернуться в зал" };
                    if (IsRustyKey && IsGoldenKey)
                        LocationsToGo = new List<string>() { "Попробовать открыть золотым ключом", "Попробовать открыть ржавым ключом", "Вернуться в зал" };
                    sticker = "https://tlgrm.ru/_/stickers/9e5/bbe/9e5bbe60-32fe-34e3-a413-e9bb4538ae9e/192/13.webp";
                    break;

                case "Бегом на кухню":
                    textToSay = "Перед вами холодильник с замком.";
                    if (IsGoldenKey && !DidAttackByGoldenKey && !DidAttackByRustyKey)
                        LocationsToGo = new List<string>() { "Попробовать открыть ржавым ключом", "Попробовать открыть золотым ключом" };
                    if (!DidAttackByRustyKey && (DidAttackByGoldenKey || !IsGoldenKey))
                        LocationsToGo = new List<string>() { "Попробовать открыть ржавым ключом" };
                    if ((DidAttackByRustyKey || !IsRustyKey) && (!DidAttackByGoldenKey && IsGoldenKey))
                        LocationsToGo = new List<string>() { "Попробовать открыть золотым ключом" };
                    if (DidAttackByRustyKey && (DidAttackByGoldenKey || !IsGoldenKey))
                        LocationsToGo = new List<string>() { "Попробовать открыть" };
                    sticker = "https://tlgrm.ru/_/stickers/9e5/bbe/9e5bbe60-32fe-34e3-a413-e9bb4538ae9e/192/50.webp";
                    break;

                case "Попробовать открыть":
                case "Попробовать открыть золотым ключом":
                    if (IsRaidMode)
                    {
                        textToSay = "Заперто. Времени больше не осталось. Вы в тупике.";
                        LocationsToGo = new List<string>() { "Вспомнить лучшие моменты жизни" };
                        sticker = "https://tlgrm.ru/_/stickers/4a4/f28/4a4f2880-e005-3f8f-ab47-2bb189e7d263/192/19.webp";
                        break;
                    }
                    else
                    {
                        textToSay = "Заперто.";
                        LocationsToGo = new List<string>() { "Отойти от холодильника" };
                        sticker = "https://tlgrm.ru/_/stickers/9e5/bbe/9e5bbe60-32fe-34e3-a413-e9bb4538ae9e/192/51.webp";
                        break;
                    }
                case "КОНЕЦ.":
                    textToSay = "";
                    LocationsToGo = new List<string>() { "КОНЕЦ?" };
                    IsOver = true;
                    break;

                case "":
                    textToSay = "";
                    LocationsToGo = new List<string>() { "КОНЕЦ." };
                    break;

                case "Вспомнить лучшие моменты жизни":
                    textToSay = "Дядя медленно зашел на кухню. Медленно закрыл дверь. Медленно достал дробовик. И быстро распределил ваши внутренности по дверце холодильника.";
                    LocationsToGo = new List<string>() { "КОНЕЦ." };
                    sticker = "https://cdn.tlgrm.app/stickers/e3a/27d/e3a27d19-e86a-4932-9a27-1bbcca58d75a/192/2.webp";
                    break;

                case "Попробовать открыть ржавым ключом":
                    textToSay = "Пызы.";
                    LocationsToGo = new List<string>() { "Good Ending" };
                    IsOver = true;
                    sticker = "https://www.amigoss.eu/wp-content/uploads/2019/01/Amigos_mockup_pyzy_z_miesem_1.png";
                    break;

                default:
                    textToSay = _lastWords;
                    break;
            }
            _lastWords = textToSay;

            return new Tuple<string, ReplyKeyboardMarkup, string>(textToSay, KeyboardOptimizer(LocationsToGo), sticker);
        }
    }
}