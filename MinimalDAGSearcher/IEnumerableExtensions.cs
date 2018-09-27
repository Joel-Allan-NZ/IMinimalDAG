using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGSearcher.Extensions
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
            foreach (T item in enumerable)
            {
                if (!Found)
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
    }
}
