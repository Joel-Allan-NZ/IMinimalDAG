using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMinimalDAGInterfaces
{
    public interface IMinimalDAGFactory<T>
    {
        IMinimalDAG<T> Create(IEnumerable<IEnumerable<T>> orderedSequences, IMinimalDAGNodeFactory<T> nodeFactory);
    }
}
