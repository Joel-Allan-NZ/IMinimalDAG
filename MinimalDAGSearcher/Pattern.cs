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


        Dictionary<int, int> _maxPrefixes;
        /// <summary>
        /// A value representing an empty space in the SearchSpace.
        /// </summary>
        public T EmptyValue { get; private set; }

        public Pattern(HashSet<T>[] searchSpace, T emptyValue = default(T))
        {
            SearchSpace = searchSpace;
            EmptyValue = emptyValue;
            SequenceBoundaries = FindBoundaries();
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
            //_linkedToStart = new HashSet<int>();
            SequenceBoundaries = FindBoundaries();//FindSequenceBoundaries();
            //FindStartLinks();
        }

        /// <summary>
        /// Handles maximal prefix updating for a given index (intended to be called sequentially).
        /// </summary>
        /// <param name="index"></param>
        //private void HandleMaxPrefixAtIndex(int index)
        //{
        //    //if last element was potentially empty, it's a break for maximal indices
        //    //conversely, if the last element was forced empty, it's a new maximal index for following elements
        //    if (!IsIndexPotentiallyEmpty(index - 1)) //if last wasn't empty, we inherit any maximal index from it
        //    {
        //        if (_maxPrefixes.TryGetValue(index - 1, out int max))
        //            _maxPrefixes[index] = max;
        //    }
        //    if (IsIndexForcedEmpty(index - 1))
        //        _maxPrefixes[index] = index;
        //}

        //private List<int[]> FindBoundaries()
        //{
        //    _maxPrefixes = new Dictionary<int, int>();

        //    List<int> OpenBlocks = new List<int>();

        //    foreach (int index in Enumerable.Range(0, SearchSpace.Length-1))
        //    {
        //        if (index > 0)
        //        {
        //            // cases: predecessor not empty, and has maximal value -> inherit maximal. not new block.
        //            //      : precedessor forced empty, this is a maximal value, new block.
        //            //      
        //            // seperately:
        //            //          :this is (possibly) empty, all existing blocks can be terminated (one point earlier than this index)
        //            //          :this is *not* empty, in which case it's either a continuation of an existing block, or the start of a new block.
        //            //          

        //            if (!IsIndexPotentiallyEmpty(index - 1) && _maxPrefixes.TryGetValue(index - 1, out int max))
        //                _maxPrefixes[index] = max;
        //            else if (IsIndexForcedEmpty(index - 1))
        //                _maxPrefixes[index] = index;
        //            else

        //        }


        //    }
        //}
        /// <summary>
        /// Finds the edges of mandatory blocks in the searchspace
        /// </summary>
        /// <returns></returns>
        private List<int[]> FindBoundaries()
        {
            _maxPrefixes = new Dictionary<int, int>();
            List<int[]> Boundaries = new List<int[]>();
            List<int> OpenBlockStarts = new List<int>();

            if (!IsIndexPotentiallyEmpty(0))
            {
                OpenBlockStarts.Add(0);
                _maxPrefixes[0] = 0;
            }
            foreach (int index in Enumerable.Range(1, SearchSpace.Length - 1))
            {
                if (IsIndexPotentiallyEmpty(index))
                {
                    foreach (var OpenBlock in OpenBlockStarts)
                        Boundaries.Add(new int[] { OpenBlock, index - 1 });
                    OpenBlockStarts.Clear();
                }
                else if (OpenBlockStarts.Count == 0)
                    OpenBlockStarts.Add(index);

                if (!IsIndexForcedEmpty(index))
                {
                    if (!IsIndexPotentiallyEmpty(index - 1))
                        if (_maxPrefixes.TryGetValue(index - 1, out int max))
                            _maxPrefixes[index] = max;
                }
            }
            foreach (var OpenBlock in OpenBlockStarts)
                Boundaries.Add(new int[] { OpenBlock, SearchSpace.Length - 1 });
            return Boundaries;
        }

        /// <summary>
        /// Finds the edges of blocks of values in the searchspace
        /// </summary>
        /// <returns></returns>
        //private List<int[]> FindBoundaries()
        //{
        //    _maxPrefixes = new Dictionary<int, int>();
        //    List<int[]> Boundaries = new List<int[]>();
        //    List<int> OpenBlockStarts = new List<int>();

        //    if (!IsIndexPotentiallyEmpty(0))
        //    {
        //        OpenBlockStarts.Add(0);
        //        _maxPrefixes[0] = 0;
        //    }
        //    foreach (int index in Enumerable.Range(1, SearchSpace.Length - 1))
        //    {
        //        HandleMaxPrefixAtIndex(index);

        //        if(IsIndexPotentiallyEmpty(index)) //end of open block(s)
        //        {
        //            foreach(var OpenBlock in OpenBlockStarts)
        //                Boundaries.Add(new int[] { OpenBlock, index - 1 });
        //            OpenBlockStarts.Clear();

        //            if (SearchSpace[index] != null && IsIndexPotentiallyEmpty(index-1) && !IsIndexForcedEmpty(index) && OpenBlockStarts.Count == 0)
        //            {//potentially empty point, succeeding emptiness
        //                if (index == SearchSpace.Length - 1 || IsIndexPotentiallyEmpty(index + 1)) //followed by emptiness or end
        //                    Boundaries.Add(new int[] { index, index });
        //            }
        //        }
        //        else //is concrete, start or continue block
        //        {
        //            if (OpenBlockStarts.Count == 0)
        //                OpenBlockStarts.Add(index);
        //        }

        //    }
        //    //handle any unclosed blocks
        //    foreach (var OpenBlock in OpenBlockStarts)
        //        Boundaries.Add(new int[] { OpenBlock, SearchSpace.Length - 1 });
        //    return Boundaries;
        //}

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
        public bool IsIndexPotentiallyEmpty(int index)
        {
            return (SearchSpace[index] == null || SearchSpace[index].Contains(EmptyValue));
        }

        /// <summary>
        /// Checks if a given index in the search space must be empty (ie there are no acceptable values).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsIndexForcedEmpty(int index)
        {
            return SearchSpace[index] != null && SearchSpace[index].Count == 1 && SearchSpace[index].Contains(EmptyValue);
        }


        /// <summary>
        /// Checks if a given index is part of a sequence leading back to the first index in the searchspace.
        /// This is also true when there are no sequence boundaries between the given index and the first index, as no sequence
        /// could terminate in that space.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsPrefixMaximalAtIndex(int index) => _maxPrefixes.ContainsKey(index);//true;//_linkedToStart.Contains(index);

        /// <summary>
        /// Checks if a given index in the pattern is preceeded by mandatory characters. If a mandatory prefix exists, returns that
        /// prefix via out keyword.
        /// </summary>
        /// <param name="index">The index to check for a required prefix.</param>
        /// <param name="prefixIndex">The starting point of a required prefix (if any)</param>
        /// <returns></returns>
        public bool TryGetHardPrefixLimit(int index, out int prefixIndex) => _maxPrefixes.TryGetValue(index, out prefixIndex);
    }
}
