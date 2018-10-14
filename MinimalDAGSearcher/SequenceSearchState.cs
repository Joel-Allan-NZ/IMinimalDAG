using IMinimalDAGInterfaces;
using MinimalDAGSearcher.Extensions;
using System.Collections.Concurrent;
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

       // internal IMinimalDAGNode<T> PreviousNode { get; set; }
        /// <summary>
        /// The <typeparamref name="T"/> values still to be used.
        /// </summary>
        internal IEnumerable<T> ValuePool { get; set; }

        /// <summary>
        /// The number of available Wild card values
        /// </summary>
        internal int WildCardCount { get; set; }

        ///// <summary>
        ///// The <typeparamref name="T"/> values that have already been used in this search.
        ///// </summary>
        //internal List<T> UsedValues { get; set; }
        internal Dictionary<int, T> UsedValues { get; set; }

        /// <summary>
        /// The indices at which a wild card has been used.
        /// </summary>
        internal List<int> WildCardIndices { get; set; }

        internal SequenceSearchState(IMinimalDAGNode<T> currentNode, IEnumerable<T> valuePool, int index, int wildCardCount = 0, 
                                                                        Dictionary<int, T> usedValues=null, List<int> wildCardIndices = null)
        {
            WildCardIndices = wildCardIndices ?? new List<int>();
            CurrentNode = currentNode;
            ValuePool = valuePool;
            Index = index;
            UsedValues = (usedValues == null)?  new Dictionary<int, T>(): new Dictionary<int, T>(usedValues);//new List<T>();
            WildCardCount = wildCardCount;
        }

        internal SequenceSearchState(SequenceSearchState<T> other, IMinimalDAGNode<T> nextNode, int stepDirection, T UsedValue, bool wildCard)
        {
            this.WildCardIndices = new List<int>(other.WildCardIndices);
            Index = other.Index + stepDirection;
            CurrentNode = nextNode;
            UsedValues = new Dictionary<int, T>(other.UsedValues);

            //doesn't make sense:
            //if(!UsedValues.ContainsKey(Index))
            UsedValues.Add(Index, UsedValue);
            // PreviousNode = other.CurrentNode;
            if (wildCard)
            {
                WildCardCount = other.WildCardCount - 1;
                ValuePool = new List<T>(other.ValuePool);
                WildCardIndices.Add(Index);
            }
            else
            {
                WildCardCount = other.WildCardCount;
                ValuePool = other.ValuePool.ExceptFirst(UsedValue).ToList();
            }
        }

        internal SequenceSearchState(SequenceSearchState<T> other, IMinimalDAGNode<T> nextNode, int stepDirection)
        {
            Index = other.Index + stepDirection;
            CurrentNode = nextNode;
            //PreviousNode = other.CurrentNode;
            ValuePool = new List<T>(other.ValuePool);//.ToList();
            UsedValues = new Dictionary<int, T>(other.UsedValues);// { Index, CurrentNode.GetValue() };
            this.WildCardIndices = new List<int>(other.WildCardIndices);
            this.WildCardCount = other.WildCardCount;
        }
    }
}
