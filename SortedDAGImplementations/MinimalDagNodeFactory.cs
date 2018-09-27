using IMinimalDAGInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations
{
    public class MinimalDAGNodeFactory<T> : IMinimalDAGNodeFactory<T>
    {
        public IMinimalDAGNode<T> CreateNode(T value, Guid id)
        {
            return new MinimalDAGNode<T>(value, id);
        }
    }
}
