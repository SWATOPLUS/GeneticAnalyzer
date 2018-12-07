using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAnalyzer
{
    public class GeneticProcessor<TGenome, TFitness>
    where TFitness : IComparable<TFitness>
    {
        private readonly Func<TGenome, TGenome, double, TGenome> _crossoverFunc;
        private readonly Func<TGenome, TFitness> _fitnessFunc;
        private readonly Random _random;

        public IReadOnlyList<TGenome> Population { get; private set; }

        public int Generation { get; private set; }

        public TGenome BestGenome => Population.First();

        public TFitness BestFitness => _fitnessFunc(BestGenome);

        public GeneticProcessor(IEnumerable<TGenome> population, Func<TGenome, TGenome,double, TGenome> crossoverFunc, Func<TGenome, TFitness> fitnessFunc)
        {
            _random = new Random();
            _crossoverFunc = crossoverFunc;
            _fitnessFunc = fitnessFunc;
            Population = population
                .OrderByDescending(x => _fitnessFunc(x))
                .ToArray();
        }

        public void NextGeneration(double mutationRate)
        {
            Generation++;
            var pairs = BuildPairs();

            var newGenomes = pairs
                .Select(x => _crossoverFunc(x.Item1, x.Item2, mutationRate))
                .Concat(Population)
                .OrderByDescending(x=> _fitnessFunc(x))
                .ToArray();

            var bestGenomes = newGenomes.Take(Population.Count / 2);
            var worstGenomes = newGenomes.Skip(Population.Count / 2)
                .GetRandomItemsArray(Population.Count / 2, _random);

            Population = bestGenomes.Concat(worstGenomes).ToArray();
        }

        private IEnumerable<(TGenome, TGenome)> BuildPairs()
        {
            var pairs = new List<(TGenome, TGenome)>(Population.Count);

            foreach (var x in Population)
            {
                var y = x;

                while (ReferenceEquals(x, y))
                {
                    y = Population[_random.Next() % Population.Count];
                }

                pairs.Add((x, y));
            }

            return pairs;
        }
    }
}
