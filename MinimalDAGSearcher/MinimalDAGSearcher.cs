using IMinimalDAGInterfaces;
using MinimalDAGSearcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinimalDAGSearcher
{
    /// <summary>
    /// Encapsulates search functionality for an <see cref="IMinimalDAG{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MinimalDAGSearcher<T> : IMinimalDAGSearcher<T>
    {
        private IMinimalDAG<T> _dag;
        private T[] _searchSpace;
        //private T _wildCardValue;
        private int _wildCardCount;
        private T _emptyValue;

        public MinimalDAGSearcher(IMinimalDAG<T> dag)
        {
            _dag = dag;
        }

        /// <summary>
        /// Returns the starting indices of blocks of non-<paramref name="emptyValue"/> values
        /// </summary>
        /// <param name="searchSpace"></param>
        /// <param name="emptyValue"></param>
        /// <returns></returns>
        private IEnumerable<int> GetStartingPointIndices(T[] searchSpace, T emptyValue)
        {
            for (int start = 0; start < searchSpace.Length; start++)
            {
                if (!searchSpace[start].Equals(emptyValue))
                {
                    for (int end = start; end < searchSpace.Length; end++)
                    {
                        if (searchSpace[end].Equals(emptyValue))//previous node was last non empty value
                        {

                            yield return end - 1;
                            start = end;
                            break;
                        }
                        else if (end == searchSpace.Length - 1)
                        {
                            yield return end;
                            yield break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find all valid sequences 'starting' at the given start index.
        /// </summary>
        /// <param name="valuePool"></param>
        /// <param name="startingIndex"></param>
        /// <returns></returns>
        private IEnumerable<DAGSearchResult<T>> FindValidSequencesAtStartIndex(IEnumerable<T> valuePool, int startingIndex)
        {
            var PossibleStartingNodes = _dag.GetAllNodesWithValue(_searchSpace[startingIndex]);
            var PrefixBoundaries = FindValidBoundaryPositions(-1);
            var SuffixBoundaries = FindValidBoundaryPositions(1);

            foreach (var PossibleStartNode in PossibleStartingNodes)
            {
                var StartingPrefixSearchState = new SequenceSearchState<T>(PossibleStartNode, valuePool, startingIndex, _wildCardCount);

                foreach (var Prefix in DepthFirstSequenceSearch(StartingPrefixSearchState, _dag.GetParents, PrefixBoundaries, -1))
                {
                    var SequenceStartIndex = Prefix.Index;
                    Prefix.Index = startingIndex;
                    Prefix.CurrentNode = PossibleStartNode;

                    foreach (var Suffix in DepthFirstSequenceSearch(Prefix, _dag.GetChildren, SuffixBoundaries, 1))
                    {
                        var Sequence = BuildSequence(Suffix.UsedValues, SequenceStartIndex);
                        yield return new DAGSearchResult<T>(Sequence, Suffix.WildCardIndices, SequenceStartIndex, Suffix.UsedValues);
                    }

                }
            }
        }

        /// <summary>
        /// Builds a List{<typeparamref name="T"/>} that represents the matched sequence.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="startingPosition"></param>
        /// <returns></returns>
        private List<T> BuildSequence(Dictionary<int, T> values, int startingPosition)
        {
            List<T> Sequence = new List<T>();
            for(int i = startingPosition; i<_searchSpace.Length; i++)
            {
                if (!_searchSpace[i].Equals(_emptyValue))
                    Sequence.Add(_searchSpace[i]);
                else if (values.ContainsKey(i))
                    Sequence.Add(values[i]);
                else
                    break;
            }
            return Sequence;
        }

        /// <summary>
        /// Searches the <see cref="IMinimalDAG{T}"/> from a starting point in a single direction, returning sequences
        /// that match the valid sequence values and search space.
        /// </summary>
        /// <param name="searchState"></param>
        /// <param name="nextRelativeSelector"></param>
        /// <param name="sequenceBoundaries"></param>
        /// <param name="stepDirection"></param>
        /// <returns></returns>
        private IEnumerable<SequenceSearchState<T>> DepthFirstSequenceSearch(SequenceSearchState<T> searchState,
                                                                       Func<IMinimalDAGNode<T>, IEnumerable<IMinimalDAGNode<T>>> nextRelativeSelector,
                                                                       HashSet<int> sequenceBoundaries,
                                                                       int stepDirection)
        {
            Stack<SequenceSearchState<T>> ToCheck = new Stack<SequenceSearchState<T>>();
            ToCheck.Push(searchState);

            foreach (var SearchState in CheckBoundaryCollision(searchState, nextRelativeSelector, sequenceBoundaries))
                yield return SearchState;

            while (ToCheck.Count > 0)
            {
                var CurrentState = ToCheck.Pop();

                T NextValue;
                var Relatives = nextRelativeSelector(CurrentState.CurrentNode);
                var NextLocation = CurrentState.Index + stepDirection;

                if (IsOutOfBounds(NextLocation))
                {
                    foreach (var Relative in Relatives.Where(x => IsPossibleSequenceEnd(x)))
                        yield return CurrentState;
                }
                else
                {
                    bool IsEnd = false;
                    List<IMinimalDAGNode<T>> OtherRelatives = new List<IMinimalDAGNode<T>>();

                    foreach(var Relative in Relatives)
                    {
                        if (IsPossibleSequenceEnd(Relative))
                            IsEnd = true;
                        else
                            OtherRelatives.Add(Relative);
                    }

                    if (IsIndexValidBoundary(CurrentState.Index, sequenceBoundaries) && IsEnd)
                        yield return CurrentState; 

                    if (IsSpaceOccupied(NextLocation, out NextValue))
                    {
                        foreach (var Relative in OtherRelatives.Where(x => x.GetValue().Equals(NextValue)))
                            ToCheck.Push(new SequenceSearchState<T>(CurrentState, Relative, stepDirection));
                    }
                    else
                    {
                        foreach (var Relative in OtherRelatives)
                        {
                            NextValue = Relative.GetValue();
                            if (CurrentState.ValuePool.Contains(NextValue))
                                ToCheck.Push(new SequenceSearchState<T>(CurrentState, Relative, stepDirection, NextValue, false));
                            if (CurrentState.WildCardCount > 0)
                                ToCheck.Push(new SequenceSearchState<T>(CurrentState, Relative, stepDirection, NextValue, true));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the selected index is out of bounds of the search space.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsOutOfBounds(int index) => (index < 0 || index >= _searchSpace.Length);

        /// <summary>
        /// Checks if an index in the <see cref="_searchSpace"/> is the empty value.
        /// </summary>
        /// <param name="spaceIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsSpaceOccupied(int spaceIndex, out T value)
        {
            value = default(T);
            if (_searchSpace[spaceIndex].Equals(_emptyValue))
                return false;
            else
                value = _searchSpace[spaceIndex];
            return true;
        }

        /// <summary>
        /// Checks if a node is a possible sequence start/end.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsPossibleSequenceEnd(IMinimalDAGNode<T> node) => _dag.IsDAGBoundary(node);

        /// <summary>
        /// Checks if the <paramref name="index"/> is a valid end point in the search space.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sequenceBoundaries"></param>
        /// <returns></returns>
        private bool IsIndexValidBoundary(int index, HashSet<int> sequenceBoundaries) => sequenceBoundaries.Contains(index);

        /// <summary>
        /// Checks for a collision with an edge of the search space. In the event of a collision, returns all valid affixes terminating
        /// at that point, then breaks.
        /// </summary>
        /// <param name="searchState"></param>
        /// <param name="nextRelativeSelector"></param>
        /// <param name="sequenceBoundaries"></param>
        /// <returns></returns>
        private IEnumerable<SequenceSearchState<T>> CheckBoundaryCollision(SequenceSearchState<T> searchState,
                                                                Func<IMinimalDAGNode<T>, IEnumerable<IMinimalDAGNode<T>>> nextRelativeSelector,
                                                                HashSet<int> sequenceBoundaries)
        {
            if (searchState.Index == 0 || searchState.Index == _searchSpace.Length - 1) //if we're at the edge of the searchspace
            {
                if (sequenceBoundaries.Contains(searchState.Index)) //and the edge of the searchspace is a valid boundary (we're heading out of bounds)
                {
                    foreach (var Relative in nextRelativeSelector(searchState.CurrentNode))
                    {
                        if (_dag.IsDAGBoundary(Relative))
                            yield return searchState;
                    }
                    yield break;
                }
            }
        }

        /// <summary>
        /// Finds the points in which a sequence can begin/end (ie is preceeded/followed by at least one 'empty' space
        /// or is at the edge of the search space.
        /// </summary>
        /// <param name="stepDirection"></param>
        /// <returns></returns>
        private HashSet<int> FindValidBoundaryPositions(int stepDirection)
        {
            HashSet<int> Boundaries = new HashSet<int>();

            for (int i = 0; i < _searchSpace.Length; i++)
            {
                if (i + stepDirection < 0 || i + stepDirection >= _searchSpace.Length)
                    Boundaries.Add(i);
                else if (_searchSpace[i + stepDirection].Equals(_emptyValue))
                    Boundaries.Add(i);
            }
            return Boundaries;
        }

        /// <summary>
        /// Search the <see cref="IMinimalDAG{T}"/> for full sequences that match runs of <typeparamref name="T"/> values
        /// in the <paramref name="existingValues"/> sequence, and consist only of values from the <paramref name="valuePool"/>
        /// (without replacements).
        /// </summary>
        /// <param name="valuePool">Values allowed in the matching sequence</param>
        /// <param name="existingValues">The partial sequence(s) to match</param>
        /// <param name="emptyValue">A value in the <paramref name="existingValues"/> that represents a currently empty space in the sequence.</param>
        /// <param name="wildCardCount">The number of wildcard values to use in the search</param>
        /// <returns></returns>
        public IEnumerable<DAGSearchResult<T>> FindMatchingSequences(IEnumerable<T> valuePool, T[] existingValues, T emptyValue, int wildCardCount)
        {
            _searchSpace = existingValues;
            _emptyValue = emptyValue;
            _wildCardCount = wildCardCount;

            var StartingIndices = GetStartingPointIndices(existingValues, emptyValue);

            foreach (var StartingIndex in StartingIndices)
            {
                foreach (var Sequence in FindValidSequencesAtStartIndex(valuePool, StartingIndex))
                    yield return Sequence;
            }

        }

        /// <summary>
        /// Search the <see cref="IMinimalDAG{T}"/> for full sequences that match runs of <typeparamref name="T"/> values
        /// in the <paramref name="existingValues"/> sequence, and consist only of values from the <paramref name="valuePool"/>
        /// (without replacements).
        /// </summary>
        /// <param name="valuePool">Values allowed in the matching sequence</param>
        /// <param name="existingValues">The partial sequence(s) to match</param>
        /// <param name="emptyValue">A value in the <paramref name="existingValues"/> that represents a currently empty space in the sequence.</param>
        /// <param name="wildCardCount">The number of wildcard values to use in the search</param>
        /// <returns></returns>
        public IEnumerable<DAGSearchResult<T>> FindMatchingSequencesContainingIndex(IEnumerable<T> valuePool, T[] existingValues, T emptyValue, int wildCardCount, int index)
        {
            _searchSpace = existingValues;
            _wildCardCount = wildCardCount;
            _emptyValue = emptyValue;

            foreach (var Sequence in FindValidSequencesAtStartIndex(valuePool, index))
                yield return Sequence;
        }


        public bool Contains(IEnumerable<T> sequence)
        {
            return _dag.Contains(sequence);
        }


    }
}
