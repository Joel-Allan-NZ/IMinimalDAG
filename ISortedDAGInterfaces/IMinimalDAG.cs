using System.Collections.Generic;

namespace IMinimalDAGInterfaces
{
    /// <summary>
    /// A directed acyclic graph with the minimum number of nodes required to contain a collection of multiple <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMinimalDAG<T>
    {
        /// <summary>
        /// The root of all sequences in the DAG. All valid starting characters/points are children of this node.
        /// </summary>
        IMinimalDAGNode<T> Source { get; }

        /// <summary>
        /// The termination point of all sequences in the DAG. All valid sequences end with this node (note that it isn't considered to be part of a sequence).
        /// </summary>
        IMinimalDAGNode<T> Sink { get; }

        /// <summary>
        /// Returns all nodes in the DAG with the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IEnumerable<IMinimalDAGNode<T>> GetAllNodesWithValue(T value);

        /// <summary>
        /// Checks if the current node is equivalent to the 'boundary' nodes of the DAG (source/sink).
        /// </summary>
        /// <param name="possibleBoundaryNode"></param>
        /// <returns></returns>
        bool IsDAGBoundary(IMinimalDAGNode<T> possibleBoundaryNode);
        
        
        /// <summary>
        /// Checks if this IMinimalDAG contains the specified sequence.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<T> sequence);

        /// <summary>
        /// Recover a collection of all sequences used to create the <see cref="IMinimalDAG{T}"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<List<T>> GetAllSequences();

        /// <summary>
        /// Returns the nodes's child nodes, with an optional set of allowed values.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="filter">optional allowed values</param>
        /// <returns></returns>
        IEnumerable<IMinimalDAGNode<T>> GetChildren(IMinimalDAGNode<T> node, HashSet<T> filter = null);

        /// <summary>
        /// Returns the node's parent nodes, with an optional set of allowed values.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<IMinimalDAGNode<T>> GetParents(IMinimalDAGNode<T> node, HashSet<T> filter = null);
        /// <summary>
        /// Returns all nodes that are end points for any sequence.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMinimalDAGNode<T>> GetAllValidEndNodes();

        /// <summary>
        /// Returns all nodes that are start points for any sequence.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMinimalDAGNode<T>> GetAllValidStartNodes();
    } 
}
