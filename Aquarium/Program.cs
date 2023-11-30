using System;
using System.Collections.Generic;
using System.Linq;

namespace Aquarium
{
    internal class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            Aquarium aquarium = new Aquarium();
            ActionBuilder actionBuilder = new ActionBuilder(aquarium);
            Menu menu = new Menu(actionBuilder.GiveActions(), actionBuilder.GiveDrawAquariumAction());

            menu.Work();
        }
    }

    class Fish
    {
        private int _age = 0;
        private int _maxAge = 10;

        private string _deadMessage = "Я мертвая рыбка.";

        public bool IsAlive => _age < _maxAge;
        private string AliveMessage => $"Я рыбка, мой возраст - {_age}.";

        public void Grow()
        {
            if (IsAlive == false)
                return;

            _age++;
        }

        public string TellStatus() =>
            IsAlive ? AliveMessage : _deadMessage;
    }

    class Aquarium
    {
        private List<Fish> _fish = new List<Fish>();
        private int _capacity = 5;

        private bool IsFull =>
            _capacity <= _fish.Count;

        public string[] TellFishStatus()
        {
            string[] fishStatus = new string[_capacity];

            for (int i = 0; i < _fish.Count; i++)
                fishStatus[i] = _fish[i].TellStatus();

            return fishStatus;
        }

        public void AddFish(Fish fish)
        {
            if (IsFull == false)
                _fish.Add(fish);
        }

        public void RemoveDeadFish() =>
            _fish.RemoveAll(fish => fish.IsAlive == false);

        public void Live() =>
            _fish.ForEach(fish => fish.Grow());
    }

    class Menu
    {
        private const ConsoleKey MoveSelectionUp = ConsoleKey.UpArrow;
        private const ConsoleKey MoveSelectionDown = ConsoleKey.DownArrow;
        private const ConsoleKey ConfirmSelection = ConsoleKey.Enter;

        private int _itemIndex = 0;
        private bool _isRunning;
        private string[] _items;

        private Dictionary<string, Action> _actions = new Dictionary<string, Action>();
        private Action _drawAquarium;

        public Menu(Dictionary<string, Action> actions, Action drawAquarium)
        {
            _actions = actions;
            _actions.Add("Выход", Exit);
            _items = _actions.Keys.ToArray();
            _drawAquarium = drawAquarium;
        }

        private int ItemIndex
        {
            get
            {
                return _itemIndex;
            }

            set
            {
                int lastIndex = _items.Length - 1;

                if (value > lastIndex)
                    value = lastIndex;

                if (value < 0)
                    value = 0;

                _itemIndex = value;
            }
        }

        public void Work()
        {
            _isRunning = true;

            while (_isRunning)
            {
                Renderer.DrawMenu(_items, _itemIndex);

                _drawAquarium.Invoke();

                ReadKey();
            }
        }

        private void ReadKey()
        {
            switch (Console.ReadKey(true).Key)
            {
                case MoveSelectionDown:
                    ItemIndex++;
                    break;

                case MoveSelectionUp:
                    ItemIndex--;
                    break;

                case ConfirmSelection:
                    _actions[_items[_itemIndex]].Invoke();
                    break;
            }
        }

        private void Exit() =>
            _isRunning = false;
    }

    class ActionBuilder
    {
        private Aquarium _aquarium;

        public ActionBuilder(Aquarium aquarium) =>
            _aquarium = aquarium;

        public Dictionary<string, Action> GiveActions()
        {
            Dictionary<string, Action> actions = new Dictionary<string, Action>
            {
                { "Купить новую рыбку.", BuyFish },
                { "Убрать мертвых рыбок.", RemoveDeadFish },
                { "Позволить рыбкам пожить...", LiveYear }
            };

            return actions;
        }

        public Action GiveDrawAquariumAction() =>
            () => Renderer.DrawAquarium(_aquarium.TellFishStatus());

        private void BuyFish() =>
            _aquarium.AddFish(new Fish());

        private void RemoveDeadFish() =>
            _aquarium.RemoveDeadFish();

        private void LiveYear() =>
            _aquarium.Live();
    }

    class Renderer
    {
        private static ConsoleColor _backgroundColor = ConsoleColor.White;
        private static ConsoleColor _foregroundColor = ConsoleColor.Black;

        private static int aquariumCursorPositionY = 10;
        private static int _lineSize = 30;

        private static char _aquariumBorder = '-';
        private static char _spaceChar = ' ';

        public static void DrawMenu(string[] items, int index)
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < items.Length; i++)
                if (i == index)
                    WriteColoredText(items[i]);
                else
                    Console.WriteLine(items[i]);
        }

        public static void DrawAquarium(string[] infoArray)
        {
            Console.CursorTop = aquariumCursorPositionY;

            Console.WriteLine(new string(_aquariumBorder, _lineSize));

            foreach (string info in infoArray)
            {
                Console.Write(new string(_spaceChar, _lineSize));

                Console.CursorLeft = 0;

                Console.WriteLine(info);
            }

            Console.WriteLine(new string(_aquariumBorder, _lineSize));
        }

        private static void WriteColoredText(string text)
        {
            Console.ForegroundColor = _foregroundColor;
            Console.BackgroundColor = _backgroundColor;

            Console.WriteLine(text);

            Console.ResetColor();
        }
    }
}
