using IMinimalDAGInterfaces;
using MinimalDAGSearcher.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MinimalDAGSearcher
{
    /// <summary>
    /// An encapsulation of the current search position/state.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SequenceSearchState<T>
    {
        /// <summary>
        /// The current index in the searchspace
        /// </summary>
        internal int Index { get; set; }
        /// <summary>
        /// The Current <see cref="IMinimalDAGNode{T}"/> being searched
        /// </summary>
        internal IMinimalDAGNode<T> CurrentNode { get; set; }
        /// <summary>
        /// The <typeparamref name="T"/> values still to be used.
        /// </summary>
        internal IEnumerable<T> ValuePool { get; set; }

        /// <summary>
        /// The number of available Wild card values
        /// </summary>
        internal int WildCardCount { get; set; }

        /// <summary>
        /// The <typeparamref name="T"/> values that have already been used in this search.
        /// </summary>
        internal List<T> UsedValues { get; set; }

        /// <summary>
        /// The indices at which a wild card has been used.
        /// </summary>
        internal List<int> WildCardIndices { get; set; }

        internal SequenceSearchState(IMinimalDAGNode<T> currentNode, IEnumerable<T> valuePool, int index, int wildCardCount = 0, List<T> usedValues = null)
        {
            WildCardIndices = new List<int>();
            CurrentNode = currentNode;
            ValuePool = valuePool;
            Index = index;
            UsedValues = usedValues ?? new List<T>();
            WildCardCount = wildCardCount;
        }

        internal SequenceSearchState(SequenceSearchState<T> other, IMinimalDAGNode<T> nextNode, int stepDirection, T validValue, bool usedWildCard)
        {
            Index = other.Index + stepDirection;
            CurrentNode = nextNode;
            this.WildCardIndices = new List<int>(other.WildCardIndices);
            if (usedWildCard)
            {
                WildCardCount = other.WildCardCount - 1;
                ValuePool = new List<T>(other.ValuePool);
                UsedValues = new List<T>(other.UsedValues);
                WildCardIndices.Add(other.Index);
            }
            else
            {
                ValuePool = other.ValuePool.ExceptFirst(validValue).ToList();
                UsedValues = new List<T>(other.UsedValues) { validValue };
            }
        }

        internal SequenceSearchState(SequenceSearchState<T> other, IMinimalDAGNode<T> nextNode, int stepDirection)
        {
            Index = other.Index + stepDirection;
            CurrentNode = nextNode;
            ValuePool = other.ValuePool.ToList();
            UsedValues = new List<T>(other.UsedValues) { CurrentNode.GetValue() };
        }
    }
}
