using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace ThiefVladislavBot
{
    public class Game
    {
        public List<string> Locations { get; set; }
        public bool IsKey { get; set; }
        public bool IsOver { get; set; }

        public Game()
        {
            Locations = new List<string>() { "Zal", "Tualet", "Komnata" };
            IsKey = false;
            IsOver = false;
        }

        private KeyboardButton[][] KeyboardOptimizer(List<string> names)
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

            return keyboard;
        }

        public Tuple<string, ReplyKeyboardMarkup> StartGame()
        {
            string text = "Darova, idi svoiej dorogoi, Stalker";
            var keyboard = new ReplyKeyboardMarkup(KeyboardOptimizer(Locations))
            {
                ResizeKeyboard = true
            };
            return new Tuple<string, ReplyKeyboardMarkup>(text, keyboard);
        }


    }
}
