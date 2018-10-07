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
        /// Returns a new IEnumerable{<typeparamref name="T"/>} without the first occurence of <typeparamref name="T"/> <paramref name="toExclude"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="toExclude"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExceptFirst<T>(this IEnumerable<T> enumerable, T toExclude)
        {
            bool Found = false;
            foreach (T item in enumerable)
            {
                if (!Found)
                {
                    if (item.Equals(toExclude))
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
