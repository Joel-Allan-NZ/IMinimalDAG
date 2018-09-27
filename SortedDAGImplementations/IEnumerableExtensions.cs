using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Filters out the first occurence of an object from this IEnumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="except"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExceptFirst<T>(this IEnumerable<T> enumerable, T except)
        {
            bool Found = false;
            foreach(T item in enumerable)
            {
                if(!Found)
                {
                    if (item.Equals(except))
                        Found = true;
                    else
                        yield return item;
                }
                else
                    yield return item;
            }
        }

        /// <summary>
        /// Compares two IEnumerables, returning the equal <typeparamref name="T"/> values from the start of each enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IEnumerable<T> LongestCommonPrefix<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
        {
            using (var EnumeratorOne = enumerable.GetEnumerator())
            using (var EnumeratorTwo = other.GetEnumerator())
            {
                while (EnumeratorOne.MoveNext() && EnumeratorTwo.MoveNext())
                {
                    if (EnumeratorOne.Current.Equals(EnumeratorTwo.Current))
                        yield return EnumeratorOne.Current;
                    else
                        yield break;
                }
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sequenceWithPrefix"></param>
        ///// <param name="prefix"></param>
        ///// <returns></returns>
        //public static  IEnumerable<T> GetSuffix<T>(this IEnumerable<T> sequenceWithPrefix, IEnumerable<T> prefix)
        //{
        //    var SequenceEnumerator = sequenceWithPrefix.GetEnumerator();
        //    var PrefixEnumerator = prefix.GetEnumerator();
        //    while (SequenceEnumerator.MoveNext() && PrefixEnumerator.MoveNext())
        //    {

        //    }
        //    yield return SequenceEnumerator.Current;
        //    while (SequenceEnumerator.MoveNext())
        //        yield return SequenceEnumerator.Current;
        //}
    }
}
