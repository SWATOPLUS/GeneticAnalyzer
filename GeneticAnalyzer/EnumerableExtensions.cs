using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAnalyzer
{
    public static class EnumerableExtensions
    {
        public static T[] GetRandomItemsArray<T>(this IEnumerable<T> sourceItems, int count, Random random)
        {
            var items = sourceItems.ToArray();

            if (items.Length < count)
            {
                throw new InvalidOperationException();
            }

            for (var i = 0; i < count; i++)
            {
                var rand = random.Next(i, count);

                var temp = items[i];
                items[i] = items[rand];
                items[rand] = temp;
            }

            return items.Take(count).ToArray();
        }
    }
}
