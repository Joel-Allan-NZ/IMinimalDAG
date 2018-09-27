using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMinimalDAGInterfaces
{
    public interface IMinimalDAGNodeFactory<T>
    {
        IMinimalDAGNode<T> CreateNode(T value, Guid id);
    }
}
