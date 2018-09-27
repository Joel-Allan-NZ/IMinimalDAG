using IMinimalDAGInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations
{
    public class MinimalDAGFactory<T> : IMinimalDAGFactory<T>
    {
        public IMinimalDAG<T> Create(IEnumerable<IEnumerable<T>> orderedSequences, IMinimalDAGNodeFactory<T> dagNodeFactory)
        {
            return new MinimalDAG<T>(orderedSequences, dagNodeFactory);
        }
    }
}
