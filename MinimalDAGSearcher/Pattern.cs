using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGSearcher
{
    /// <summary>
    /// A search pattern representing a partial sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pattern<T> : IPattern<T>
    {
        /// <summary>
        /// Acceptable values for each possible index. Null values simply have no restriction, and indices containing
        /// the EmptyValue either have no restriction (if it's the only value in the collection) or represents a possibly
        /// empty space.
        /// </summary>
        public HashSet<T>[] SearchSpace { get; private set; }

        /// <summary>
        /// The boundary indices of any possible sequence in the SearchSpace. These end points are indices
        /// that are preceeded/succeeded by empty space.
        /// </summary>
        public List<int[]> SequenceBoundaries { get; private set; }

        HashSet<int> _linkedToStart;
        /// <summary>
        /// A value representing an empty space in the SearchSpace.
        /// </summary>
        public T EmptyValue { get; private set; }

        public Pattern(HashSet<T>[] searchSpace, T emptyValue = default(T))
        {
            SearchSpace = searchSpace;
            EmptyValue = emptyValue;
            _linkedToStart = new HashSet<int>();
            SequenceBoundaries = FindSequenceBoundaries();
            FindStartLinks();

        }

        public Pattern(T[] searchSpace, T emptyValue = default(T))
        {
            SearchSpace = new HashSet<T>[searchSpace.Length];
            for (int i = 0; i < searchSpace.Length; i++)
            {
                if (!searchSpace[i].Equals(emptyValue))
                    SearchSpace[i] = new HashSet<T>() { searchSpace[i] };
            }
            EmptyValue = emptyValue;
            _linkedToStart = new HashSet<int>();
            SequenceBoundaries = FindSequenceBoundaries();
            FindStartLinks();
        }

        /// <summary>
        /// Finds which indices are connected to the first index in the search space (ie can't be part of a valid sequence
        /// that doesn't contain the first index)
        /// </summary>
        private void FindStartLinks()
        {
            if (!IsIndexConcreteValue(0))
                return;

            _linkedToStart.Add(0);
            _linkedToStart.Add(1);

            for (int i =2; i<SearchSpace.Length; i++)
            {
                if (IsIndexEmpty(i) && IsIndexEmpty(i - 1))
                    break;
                _linkedToStart.Add(i);
            }
        }

        /// <summary>
        /// Populate the Sequence Boundary collection
        /// </summary>
        /// <returns></returns>
        private List<int[]> FindSequenceBoundaries()
        {
            List<int[]> Boundaries = new List<int[]>();
            bool isBlock = false;
            int blockStart = 0;
            for (int i = 0; i < SearchSpace.Length; i++)
            {
                if (SearchSpace[i] != null && !SearchSpace[i].Contains(EmptyValue))
                {
                    if (!isBlock)
                    {
                        blockStart = i;
                        isBlock = true;
                    }
                }
                else
                {
                    if (isBlock)
                    {
                        Boundaries.Add(new int[] { blockStart, i - 1 });
                        isBlock = false;
                    }
                }
            }
            if (isBlock)
                Boundaries.Add(new int[] { blockStart, SearchSpace.Length - 1 });

            return Boundaries;
        }

        /// <summary>
        /// Checks if a given index in the search space has a concrete (ie singular) value.
        /// </summary>
        /// <param name="index"></param>
        public bool IsIndexConcreteValue(int index)
        {
            return (SearchSpace[index] != null && SearchSpace[index].Count == 1 && !SearchSpace[index].Contains(EmptyValue));
        }

        /// <summary>
        /// Attempts to retrieve a concrete (ie singular) value from a given index in the search space, returning a boolean
        /// representing success. Returns false if there is no concrete value at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetConcreteValue(int index, out T value)
        {
            if (IsIndexConcreteValue(index))
            {
                value = SearchSpace[index].First();
                return true;
            }
            value = default(T);
            return false;
        }

        /// <summary>
        /// Checks if a given index in the search space is empty (ie has no restriction on acceptable values).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsIndexEmpty(int index)
        {
            return (SearchSpace[index] == null || SearchSpace[index].Contains(EmptyValue));
        }

        /// <summary>
        /// Checks if a given index is part of a sequence leading back to the first index in the searchspace.
        /// This is also true when there are no sequence boundaries between the given index and the first index, as no sequence
        /// could terminate in that space.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsPrefixMaximalAtIndex(int index) => _linkedToStart.Contains(index);
    }
}
