using System;

namespace Datadog.Coffeehouse.Core.Services
{
    public class RandomProvider
    {
        private readonly Random _random = new Random();

        private RandomProvider() { }

        public static RandomProvider Instance { get; } = new RandomProvider();

        public int GetRandomInt() => _random.Next();

        public int GetRandomIntBeween(int min, int max) => _random.Next(min, max + 1);

        public double GetRandomDouble() => _random.NextDouble();

        public double GetRandomDoubleBetween(double min, double max) => _random.NextDouble() * (max - min) + min;
    }
}
