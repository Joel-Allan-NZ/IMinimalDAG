using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations.Tests
{
    public static class IEnumerableExtensions
    {

        public static IEnumerable<IEnumerable<T>> SequenceOrder<T>(this IEnumerable<IEnumerable<T>> sequence)
        {
            return sequence.OrderBy(x => x, new IEnumerableComparer<T>());
        }
    }

    public class IEnumerableComparer<T> : IComparer<IEnumerable<T>>
    {
        public int Compare(IEnumerable<T> x, IEnumerable<T> y)
        {
            IComparer<T> comparer = Comparer<T>.Default;
            using (IEnumerator<T> enumerator1 = x.GetEnumerator())
            using (IEnumerator<T> enumerator2 = y.GetEnumerator())
            {
                while (true)
                {
                    bool IsNotFinished1 = enumerator1.MoveNext();
                    bool IsNotFinished2 = enumerator2.MoveNext();

                    if (!IsNotFinished1 && !IsNotFinished2)
                        return 0; //both are finished and therefore equal

                    if (!IsNotFinished1)
                        return -1;

                    if (!IsNotFinished2)
                        return 1;

                    var compared = comparer.Compare(enumerator1.Current, enumerator2.Current);
                    if (compared != 0)
                        return compared;
                }
            }
        }
    }
}
