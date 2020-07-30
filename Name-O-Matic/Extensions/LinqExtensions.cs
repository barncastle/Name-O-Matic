using System;
using System.Collections.Generic;
using System.Linq;

namespace NameOMatic.Extensions
{
    internal static class LinqExtensions
    {
        public static IEnumerable<TSource> Unique<TSource, TKey>(this IEnumerable<TSource> enumerable, Func<TSource, TKey> grouping)
        {
            return enumerable.GroupBy(grouping).Where(x => x.Count() == 1).Select(x => x.Single());
        }

        public static Dictionary<TKey, TElement> ToDictionarySafe<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var d = new Dictionary<TKey, TElement>();
            foreach (TSource element in source)
                d[keySelector(element)] = elementSelector(element);
            return d;
        }

        public static T MostCommon<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> comparer = null)
        {
            var entry = enumerable
                        .GroupBy(x => x, comparer)
                        .OrderByDescending(x => x.Count())
                        .FirstOrDefault();

            return entry == null ? default : entry.Key;
        }
    }
}
