using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMinimalDAGInterfaces
{
    /// <summary>
    /// A node in an <see cref="IMinimalDAG{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMinimalDAGNode<T>
    {
        /// <summary>
        /// The IDs of the children of this node.
        /// </summary>
        List<Guid> Children { get; set; }
        /// <summary>
        /// The IDs of the parents of this node.
        /// </summary>
        List<Guid> Parents { get; set; }
        /// <summary>
        /// The value associated with this node.
        /// </summary>
        T Value { get;  }
        /// <summary>
        /// This node's ID.
        /// </summary>
        Guid ID { get; }
        /// <summary>
        /// Whether or not this node is registered with the parent <see cref="IMinimalDAG{T}"/>
        /// </summary>
        bool IsSuffixRegistered { get; set; }
    }
}
