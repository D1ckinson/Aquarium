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
        private int _maxAge = 10;

        public bool IsAlive => Age < _maxAge;
        public int Age { get; private set; }

        public void Grow()
        {
            if (IsAlive == false)
                return;

            Age++;
        }
    }

    class Aquarium
    {
        private List<Fish> _fishes = new List<Fish>();
        private int _capacity = 5;
        private string _deadFishStatus = "x_x";

        private bool IsFull => _capacity <= _fishes.Count;

        public string[] GetFishesStatus()
        {
            string[] fishStatus = new string[_capacity];

            for (int i = 0; i < _fishes.Count; i++)            
                fishStatus[i] = _fishes[i].IsAlive ? GetFishStatus(_fishes[i].Age) : _deadFishStatus;            

            return fishStatus;
        }

        public void AddFish(Fish fish)
        {
            if (IsFull == false)
                _fishes.Add(fish);
        }

        public void RemoveDeadFish() =>
            _fishes.RemoveAll(fish => fish.IsAlive == false);

        public void Live() =>
            _fishes.ForEach(fish => fish.Grow());

        private string GetFishStatus(int age) =>
            $"Я рыбка, мой возраст - {age}.";
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
            () => Renderer.DrawAquarium(_aquarium.GetFishesStatus());

        private void BuyFish() =>
            _aquarium.AddFish(new Fish());

        private void RemoveDeadFish() =>
            _aquarium.RemoveDeadFish();

        private void LiveYear() =>
            _aquarium.Live();
    }

    class Renderer
    {
        private static ConsoleColor s_backgroundColor = ConsoleColor.White;
        private static ConsoleColor s_foregroundColor = ConsoleColor.Black;

        private static int s_aquariumCursorPositionY = 10;
        private static int s_lineSize = 30;

        private static char s_aquariumBorder = '-';
        private static char s_spaceChar = ' ';

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
            Console.CursorTop = s_aquariumCursorPositionY;

            Console.WriteLine(new string(s_aquariumBorder, s_lineSize));

            foreach (string info in infoArray)
            {
                Console.Write(new string(s_spaceChar, s_lineSize));

                Console.CursorLeft = 0;

                Console.WriteLine(info);
            }

            Console.WriteLine(new string(s_aquariumBorder, s_lineSize));
        }

        private static void WriteColoredText(string text)
        {
            Console.ForegroundColor = s_foregroundColor;
            Console.BackgroundColor = s_backgroundColor;

            Console.WriteLine(text);

            Console.ResetColor();
        }
    }
}
