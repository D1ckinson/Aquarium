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
            Menu menu = new Menu(actionBuilder.GiveActions());

            menu.Work();
        }
    }

    class Fish
    {
        private int _age;
        private int _maxAge = 10;

        public Fish() =>
            _age = 0;

        public bool IsAlive =>
            _age < _maxAge;

        public void GetOlder() =>
            _age++;
    }

    class Aquarium
    {
        private List<Fish> _fish = new List<Fish>();
        private int _capacity = 5;

        public Aquarium() { }

        public void AddFish(Fish fish) =>
            _fish.Add(fish);

        public void RemoveDeadFish() =>
            _fish.RemoveAll(fish => fish.IsAlive == false);

        public void LiveYear() =>
            _fish.ForEach(fish => fish.GetOlder());
    }

    class Menu
    {
        const ConsoleKey MoveSelectionUp = ConsoleKey.UpArrow;
        const ConsoleKey MoveSelectionDown = ConsoleKey.DownArrow;
        const ConsoleKey ConfirmSelection = ConsoleKey.Enter;

        private ConsoleColor _backgroundColor = ConsoleColor.White;//название
        private ConsoleColor _foregroundColor = ConsoleColor.Black;//название

        private int _itemIndex = 0;
        private bool _isRunning;
        private string[] _items;

        private Dictionary<string, Action> _actions = new Dictionary<string, Action>();

        public Menu(Dictionary<string, Action> actions)
        {
            _actions = actions;
            _actions.Add("Выход", Exit);
            _items = _actions.Keys.ToArray();
        }

        public void Work()
        {
            _isRunning = true;

            while (_isRunning)
            {
                DrawItems();

                ReadKey();

                ClampIndex();
            }
        }

        private void DrawItems()
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < _items.Length; i++)
                if (i == _itemIndex)
                    WriteColoredText(_items[i]);
                else
                    Console.WriteLine(_items[i]);
        }

        private void WriteColoredText(string text)
        {
            Console.ForegroundColor = _foregroundColor;
            Console.BackgroundColor = _backgroundColor;

            Console.WriteLine(text);

            Console.ResetColor();
        }

        private void ReadKey()
        {
            switch (Console.ReadKey().Key)
            {
                case MoveSelectionDown:
                    _itemIndex++;
                    break;

                case MoveSelectionUp:
                    _itemIndex--;
                    break;

                case ConfirmSelection:
                    _actions[_items[_itemIndex]].Invoke();
                    break;
            }
        }

        private void ClampIndex()
        {
            int lastIndex = _items.Length - 1;

            if (_itemIndex > lastIndex)
                _itemIndex = lastIndex;
            else if (_itemIndex < 0)
                _itemIndex = 0;
        }

        private void Exit() => _isRunning = false;
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

        private void BuyFish() =>
            _aquarium.AddFish(new Fish());

        private void RemoveDeadFish() =>
            _aquarium.RemoveDeadFish();

        private void LiveYear() =>
            _aquarium.LiveYear();
    }

    static class UserUtilities//название
    {
        private static Random s_random = new Random();

        public static int Next(int value) => s_random.Next(value);

        public static int Next(int minValue, int maxValue) => s_random.Next(minValue, maxValue);


    }
}
