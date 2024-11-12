using System;
using System.Collections.Generic;

namespace Aquarium
{
    internal class Program
    {
        static void Main()
        {
            Game game = new Game();
            game.Work();
        }
    }


    class Game
    {
        private Aquarium _aquarium;

        public Game()
        {
            _aquarium = new Aquarium();
        }

        public void Work()
        {
            const string BuyFishCommand = "1";
            const string RemoveDeadFishCommand = "2";
            const string LiveCommand = "3";
            const string ExitCommand = "4";

            bool isWork = true;

            while (isWork)
            {
                DrawAquarium(_aquarium.GetFishesStatus());

                Console.WriteLine(
                    $"{BuyFishCommand} - Купить новую рыбку\n" +
                    $"{RemoveDeadFishCommand} - Убрать мертвых рыбок\n" +
                    $"{LiveCommand} - Позволить рыбкам пожить...\n" +
                    $"{ExitCommand} - Выход\n");

                switch (ReadCommand())
                {
                    case BuyFishCommand:
                        _aquarium.AddFish(new Fish());
                        break;

                    case RemoveDeadFishCommand:
                        _aquarium.RemoveDeadFish();
                        break;

                    case LiveCommand:
                        _aquarium.Live();
                        break;

                    case ExitCommand:
                        isWork = false;
                        break;

                    default:
                        break;
                }

                Console.Clear();
            }
        }

        private void DrawAquarium(List<string> fishesInfo)
        {
            int lineSize = 30;
            char aquariumBorder = '-';

            Console.WriteLine("Аквариум:");
            Console.WriteLine(new string(aquariumBorder, lineSize));

            fishesInfo.ForEach(info => Console.WriteLine(info));
            Console.WriteLine(new string(aquariumBorder, lineSize) + "\n");
        }

        private string ReadCommand()
        {
            Console.Write("Введите команду: ");

            return Console.ReadLine();
        }
    }

    class Aquarium
    {
        private List<Fish> _fishes = new List<Fish>();
        private int _capacity = 5;

        private bool IsFull => _capacity == _fishes.Count;

        public List<string> GetFishesStatus()
        {
            string deadFishStatus = "x_x";
            List<string> fishStatus = new List<string>();

            foreach (Fish fish in _fishes)
            {
                string status = fish.IsAlive ? GetFishStatus(fish) : deadFishStatus;
                fishStatus.Add(status);
            }

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

        private string GetFishStatus(Fish fish) =>
            $"Я рыбка, мой возраст - {fish.Age}.";
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
}
