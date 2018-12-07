using System;
using System.Collections;
using System.Linq;

namespace GeneticAnalyzer.Cli
{
    internal static class Program
    {
        private static Random Random { get; } = new Random();

        private static void Main()
        {
            var initial = Enumerable.Range(0, 10).Select(x => (Denormalize(Random.NextDouble())));

            var processor = new GeneticProcessor<double, double>(initial,
                (x, y, r) => Denormalize(CrossoverDouble(Normalize(x), Normalize(y), r)),
                x => Fitness(x)
            );

            foreach (var _ in Enumerable.Range(0, 200))
            {
                Console.WriteLine($"Generation {processor.Generation}, best is {processor.BestGenome}, fitness {processor.BestFitness}");
                processor.NextGeneration(0.01);
            }
        }

        private static double Fitness(double x)
        {
            return 4 * Math.Sin(x) + 2;
        }

        private static double Normalize(double x)
        {
            return (x + 1.0) / 2.0;
        }

        private static double Denormalize(double x)
        {
            return x * 2.0 - 1.0;
        }


        private static double CrossoverDouble(double normalizedDad, double normalizedMom, double mutationRate)
        {
            var d = (uint) (normalizedDad * uint.MaxValue);
            var m = (uint) (normalizedMom * uint.MaxValue);

            var dad = new BitArray(BitConverter.GetBytes(d));
            var mom = new BitArray(BitConverter.GetBytes(m));

            var result = new BitArray(dad.Length);

            for (var i = 0; i < dad.Length; i++)
            {
                if (mutationRate > Random.NextDouble())
                {
                    result[i] = Random.NextDouble() >= 0.5;
                }
                else
                {
                    if (0.5 < Random.NextDouble())
                    {
                        result[i] = mom[i];
                    }
                    else
                    {
                        result[i] = dad[i];
                    }
                }
            }

            var r = BitConverter.ToUInt32(BitArrayToByteArray(result));

            return (double)r / uint.MaxValue;
        }

        private static byte[] BitArrayToByteArray(BitArray bits)
        {
            var ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }
    }
}
