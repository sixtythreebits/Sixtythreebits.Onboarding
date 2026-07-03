using System;
using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static class EnumerableExtensions
    {
        #region Methods
        public static bool HasElements<T>(this IEnumerable<T> collection)
        {
            return collection?.Any() == true;
        }
                
        public static IEnumerable<TResult> SelectWithNext<TSource, TResult>(this IEnumerable<TSource> inputEnumerable, Func<TSource, TSource, bool, TResult> projection)
        {
            using (var Iterator = inputEnumerable.GetEnumerator())
            {
                if (Iterator.MoveNext())
                {
                    var Current = Iterator.Current;
                    var IsLast = false;
                    while (Iterator.MoveNext())
                    {
                        var Next = Iterator.Current;
                        yield return projection(Current, Next, IsLast);
                        Current = Next;
                    }
                    IsLast = true;
                    yield return projection(Current, default(TSource), IsLast);
                }
            }
        }

        public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>(this IEnumerable<TSource> inputEnumerable, Func<TSource, TSource, bool, TResult> projection)
        {
            using (var Iterator = inputEnumerable.GetEnumerator())
            {
                var IsFirst = true;
                var Previous = default(TSource);
                while (Iterator.MoveNext())
                {
                    yield return projection(Iterator.Current, Previous, IsFirst);
                    IsFirst = false;
                    Previous = Iterator.Current;
                }
            }
        }

        public static IEnumerable<TResult> SelectWithNextAndPrevious<TSource, TResult>(this IEnumerable<TSource> inputEnumerable, Func<TSource, TSource, TSource, bool, bool, TResult> projection)
        {
            using (var Iterator = inputEnumerable.GetEnumerator())
            {
                if (Iterator.MoveNext())
                {
                    var Previous = default(TSource);
                    var Current = Iterator.Current;
                    var IsFirst = true;
                    var IsLast = false;
                    while (Iterator.MoveNext())
                    {
                        var Next = Iterator.Current;
                        yield return projection(Current, Previous, Next, IsFirst, IsLast);
                        Previous = Current;
                        Current = Next;
                        IsFirst = false;
                    }
                    IsLast = true;
                    yield return projection(Current, Previous, default(TSource), IsFirst, IsLast);
                }
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> inputEnumerable)
        {
            var rng = new Random();
            T[] elements = inputEnumerable.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                int swapIndex = rng.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        public static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> inputEnumerable, int chunkSize)
        {
            for (var i = 0; i < Math.Ceiling(inputEnumerable.Count() * 1.0 / chunkSize); i++)
            {
                yield return inputEnumerable.Skip(chunkSize * i).Take(chunkSize);
            }
        }
        #endregion
    }
}