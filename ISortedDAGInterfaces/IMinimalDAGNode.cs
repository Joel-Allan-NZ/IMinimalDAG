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
        /// Returns all the <see cref="Guid"/> child IDs.
        /// </summary>
        /// <returns></returns>
        IList<Guid> GetChildIDs();

        /// <summary>
        /// Returns all the <see cref="Guid"/> parent IDs.
        /// </summary>
        /// <returns></returns>
        IList<Guid> GetParentIDs();

        /// <summary>
        /// Returns the <typeparamref name="T"/> value.
        /// </summary>
        /// <returns></returns>
        T GetValue();

        /// <summary>
        /// Adds <paramref name="childID"/> to this node's child collection.
        /// </summary>
        /// <param name="childID"></param>
        void AddChild(Guid childID);

        /// <summary>
        /// Adds <paramref name="parentID"/> to this node's parent collection.
        /// </summary>
        /// <param name="childID"></param>
        void AddParent(Guid parentID);

        /// <summary>
        /// Remove <paramref name="child"/> from this node's child collection.
        /// </summary>
        /// <param name="child"></param>
        void RemoveChildID(Guid child);

        /// <summary>
        /// Returns the <see cref="Guid"/> ID value of this node.
        /// </summary>
        /// <returns></returns>
        Guid GetID();

        /// <summary>
        /// Returns a boolean representing whether or not the current node has been registered
        ///  (intended for use with <see cref="IMinimalDAG{T}"/> construction).
        /// </summary>
        /// <returns></returns>
        bool IsRegistered();

        /// <summary>
        /// Record that this node has been registered.
        /// </summary>
        void RegisterSuffix();
    }
}
