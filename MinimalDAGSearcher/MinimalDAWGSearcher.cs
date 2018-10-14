using IMinimalDAGInterfaces;
using MinimalDAGImplementations;
using MinimalDAGSearcher.Extensions;
using MinimalDAGSearcher.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGSearcher
{
    /// <summary>
    /// Performance focused MinimalDAG{char} searcher. Sacrifices interface abstraction for speed.
    /// </summary>
    public class MinimalDAWGSearcher
    {
        private MinimalDAG<char> _dawg;
        public MinimalDAWGSearcher(MinimalDAG<char> dawg)
        {
            _dawg = dawg;
        }

        /// <summary>
        /// Finds possible starting <see cref="SequenceSearchState{T}"/>s for sequence searching in the <see cref="MinimalDAG{T}"/>
        /// </summary>
        /// <param name="valuePool"></param>
        /// <param name="index"></param>
        /// <param name="pattern"></param>
        /// <param name="wildCardCount"></param>
        /// <returns></returns>
        private IEnumerable<SequenceSearchState<char>> FindSearchStartPoints(List<char> valuePool, int index, Pattern<char> pattern, int wildCardCount)
        {
            IEnumerable<IMinimalDAGNode<char>> PossibleStartNodes;
            var ValidStartValues = pattern.SearchSpace[index];
            if (!pattern.IsIndexForcedEmpty(index))
            {
                if (ValidStartValues.Count == 1)
                {
                    if (pattern.TryGetHardPrefixLimit(index, out int prefixIndex) || index == 0) //hard prefix limit
                    {
                        index = prefixIndex;
                        PossibleStartNodes = _dawg.GetAllValidStartNodes().Where(x => x.Value.Equals(pattern.SearchSpace[index].First()));
                    }
                    else if (index > 0 && pattern.TryGetHardPrefixLimit(index - 1, out int softPrefixIndex)) //soft prefix limit
                    {
                        if (pattern.SearchSpace[softPrefixIndex] != null)
                        {
                            PossibleStartNodes = _dawg.GetAllValidStartNodes().Where(x => x.Value.Equals(pattern.SearchSpace[softPrefixIndex].First()));
                            var FurtherPossibleStartNodes = _dawg.GetAllValidStartNodes().Where(x => x.Value.Equals(pattern.SearchSpace[index].First()));
                            PossibleStartNodes = PossibleStartNodes.Concat(FurtherPossibleStartNodes);
                        }
                        else PossibleStartNodes = _dawg.GetAllNodesWithValue(pattern.SearchSpace[index].First());
                    }
                    else
                        PossibleStartNodes = _dawg.GetAllNodesWithValue(pattern.SearchSpace[index].First());

                    foreach (var Node in PossibleStartNodes)
                        yield return new SequenceSearchState<char>(Node, valuePool, index, wildCardCount);
                }
                else
                {
                    foreach (var ValidStart in ValidStartValues)
                    {
                        if (!ValidStart.Equals(pattern.EmptyValue))
                        {
                            PossibleStartNodes = _dawg.GetAllNodesWithValue(ValidStart);
                            var UsedDict = new Dictionary<int, char>();
                            UsedDict.Add(index, ValidStart);

                            if (wildCardCount > 0)
                            {
                                List<int> WildCardIndices = new List<int>() { index };
                                foreach (var PossibleStartNode in PossibleStartNodes)
                                    yield return new SequenceSearchState<char>(PossibleStartNode, valuePool, index, wildCardCount - 1, UsedDict, WildCardIndices);
                            }
                            if (valuePool.Contains(ValidStart))
                            {
                                var UpdatedPool = new List<char>(valuePool);
                                UpdatedPool.Remove(ValidStart);
                                foreach (var PossibleStartNode in PossibleStartNodes)
                                    yield return new SequenceSearchState<char>(PossibleStartNode, UpdatedPool, index, wildCardCount, UsedDict);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find all valid sequences containing the given index, by means of a pruned depth first search.
        /// </summary>
        /// <param name="valuePool"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private IEnumerable<DAGSearchResult<char>> FindValidSequencesContainingIndex(List<char> valuePool, int index, Pattern<char> pattern, int wildCardCount)
        {
            var SuffixBoundaries = FindValidBoundaryPositions(1, pattern);
            var PrefixBoundaries = FindValidBoundaryPositions(-1, pattern);
            IEnumerable<SequenceSearchState<char>> StartingPoints = FindSearchStartPoints(valuePool, index, pattern, wildCardCount).ToList();

            return FindMatches(StartingPoints, PrefixBoundaries, SuffixBoundaries, pattern);
        }

        /// <summary>
        /// Find all matches to the <paramref name="pattern"/> starting at the selected <paramref name="startingStates"/>
        /// </summary>
        /// <param name="startingStates"></param>
        /// <param name="prefixBoundaries"></param>
        /// <param name="suffixBoundaries"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private IEnumerable<DAGSearchResult<char>> FindMatches(IEnumerable<SequenceSearchState<char>> startingStates,
                                                                    HashSet<int> prefixBoundaries,
                                                                    HashSet<int> suffixBoundaries,
                                                                    Pattern<char> pattern)
        {
            foreach (var startingState in startingStates)
            {
                var index = startingState.Index;
                var StartNode = startingState.CurrentNode;

                var Prefixes = SequenceSearch(startingState, _dawg.GetParents, prefixBoundaries, -1, pattern).ToList();
                foreach (var Prefix in SequenceSearch(startingState, _dawg.GetParents, prefixBoundaries, -1, pattern))
                {
                    var SequenceStartIndex = Prefix.Index;
                    Prefix.Index = index;
                    Prefix.CurrentNode = StartNode;

                    foreach (var Suffix in SequenceSearch(Prefix, _dawg.GetChildren, suffixBoundaries, 1, pattern))
                    {
                        var Sequence = BuildSequence(Suffix.UsedValues, SequenceStartIndex, pattern);
                        yield return new DAGSearchResult<char>(Sequence, Suffix.WildCardIndices, SequenceStartIndex, Suffix.UsedValues);
                    }
                }
            }
        }
        /// <summary>
        /// Builds a List{<typeparamref name="T"/>} that represents the matched sequence.
        /// </summary>
        /// <param name="matchedValues"></param>
        /// <param name="startingPosition"></param>
        /// <returns></returns>
        private List<char> BuildSequence(Dictionary<int, char> matchedValues, int startingPosition, Pattern<char> pattern)
        {
            List<char> Sequence = new List<char>();
            for (int i = startingPosition; i < pattern.SearchSpace.Length; i++)
            {
                if (pattern.TryGetConcreteValue(i, out char value))
                    Sequence.Add(value);
                else if (matchedValues.TryGetValue(i, out value))
                    Sequence.Add(value);
                else
                    break;
            }
            return Sequence;
        }

        /// <summary>
        /// Searches the <see cref="IMinimalDAG{T}"/> from a starting point in a single direction, returning sequences
        /// that match the valid <see cref="Pattern{T}"/>
        /// </summary>
        /// <param name="searchState"></param>
        /// <param name="nextRelativeSelector"></param>
        /// <param name="sequenceBoundaries"></param>
        /// <param name="stepDirection"></param>
        /// <returns></returns>
        private IEnumerable<SequenceSearchState<char>> SequenceSearch(SequenceSearchState<char> searchState,
                                                                       Func<IMinimalDAGNode<char>, IEnumerable<IMinimalDAGNode<char>>> nextRelativeSelector,
                                                                       HashSet<int> sequenceBoundaries,
                                                                       int stepDirection,
                                                                       Pattern<char> pattern)
        {

            if (IsBoundaryCollision(searchState, nextRelativeSelector, sequenceBoundaries, pattern))
                yield return searchState;
            else
            {
                foreach (var sequence in DepthFirstSequenceSearch(searchState, nextRelativeSelector, sequenceBoundaries, stepDirection, pattern))
                    yield return sequence;
            }
        }

        /// <summary>
        /// Performs the actual DFS search, finding all <see cref="SequenceSearchState{T}"/> that match the <see cref="Pattern{char}"/>
        /// </summary>
        /// <param name="searchState"></param>
        /// <param name="nextRelativeSelector"></param>
        /// <param name="sequenceBoundaries"></param>
        /// <param name="stepDirection"></param>
        /// <returns></returns>
        private IEnumerable<SequenceSearchState<char>> DepthFirstSequenceSearch(SequenceSearchState<char> searchState,
                                                                       Func<IMinimalDAGNode<char>, IEnumerable<IMinimalDAGNode<char>>> nextRelativeSelector,
                                                                       HashSet<int> sequenceBoundaries,
                                                                       int stepDirection, Pattern<char> pattern)
        {
            Stack<SequenceSearchState<char>> ToCheck = new Stack<SequenceSearchState<char>>();
            ToCheck.Push(searchState);
            //NB: recursive searches with yield return build a new iterator for every method call. 
            //A recursive solution would therefore have performance impact (as well as risking stackoverflow in some very deep pattern).
            while (ToCheck.Count > 0)
            {
                var CurrentState = ToCheck.Pop();
                var Relatives = nextRelativeSelector(CurrentState.CurrentNode);
                int NextIndex = CurrentState.Index + stepDirection;

                if (IsOutOfBounds(NextIndex, pattern))
                {
                    if (IsBoundaryCollision(CurrentState, nextRelativeSelector, sequenceBoundaries, pattern))
                        yield return CurrentState;
                }
                else
                {
                    bool IsEnd = false;
                    List<IMinimalDAGNode<char>> OtherRelatives = new List<IMinimalDAGNode<char>>();

                    foreach (var Relative in Relatives)
                    {
                        if (_dawg.IsDAGBoundary(Relative))
                            IsEnd = true;
                        else
                            OtherRelatives.Add(Relative);
                    }

                    if (IsEnd && IsIndexValidBoundary(CurrentState.Index, sequenceBoundaries))
                        yield return CurrentState;

                    var NextStates = GetNextStates(CurrentState, OtherRelatives, NextIndex, stepDirection, pattern);
                    foreach (var nextState in NextStates)
                        ToCheck.Push(nextState);
                }
            }
        }

        /// <summary>
        /// From the current state, find all the next states that are valid.
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="relatives"></param>
        /// <param name="nextLocation"></param>
        /// <param name="stepDirection"></param>
        /// <returns></returns>
        private List<SequenceSearchState<char>> GetNextStates(SequenceSearchState<char> currentState, List<IMinimalDAGNode<char>> relatives, int nextLocation,
                                                            int stepDirection, Pattern<char> pattern)
        {
            List<SequenceSearchState<char>> NextStates = new List<SequenceSearchState<char>>();
            char concrete;
            if (pattern.TryGetConcreteValue(nextLocation, out concrete))
            {
                foreach (var relative in relatives.Where(x => concrete.Equals(x.Value)))
                    NextStates.Add(new SequenceSearchState<char>(currentState, relative, stepDirection));
            }
            else
            {
                var ValidValues = pattern.SearchSpace[nextLocation];
                var ValidNext = relatives;
                if (ValidValues != null)// && !ValidValues.Contains(pattern.EmptyValue))
                    ValidNext = relatives.Where(x => ValidValues.Contains(x.Value)).ToList();

                foreach (var next in ValidNext)
                {
                    if (currentState.ValuePool.Contains(next.Value))
                        NextStates.Add(new SequenceSearchState<char>(currentState, next, stepDirection, next.Value, false));
                    if (currentState.WildCardCount > 0)
                        NextStates.Add(new SequenceSearchState<char>(currentState, next, stepDirection, next.Value, true));
                }

            }
            return NextStates;
        }

        /// <summary>
        /// Checks if the selected index is out of bounds of the search space.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsOutOfBounds(int index, Pattern<char> pattern) => (index < 0 || index >= pattern.SearchSpace.Length);

        /// <summary>
        /// Checks if a node is a possible sequence start/end.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
       // private bool IsPossibleSequenceEnd(IMinimalDAGNode<char> node) => _dawg.IsDAGBoundary(node);

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
        private bool IsBoundaryCollision(SequenceSearchState<char> searchState,
                                                                Func<IMinimalDAGNode<char>, IEnumerable<IMinimalDAGNode<char>>> nextRelativeSelector,
                                                                HashSet<int> sequenceBoundaries,
                                                                Pattern<char> pattern)
        {
            if (searchState.Index == 0 || searchState.Index == pattern.SearchSpace.Length - 1) //if we're at the edge of the searchspace
            {
                if (sequenceBoundaries.Contains(searchState.Index)) //and the edge of the searchspace is a valid boundary (we're heading out of bounds)
                {
                    foreach (var Relative in nextRelativeSelector(searchState.CurrentNode))
                    {
                        if (_dawg.IsDAGBoundary(Relative))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Finds the points in which a sequence can begin/end (ie is preceeded/followed by at least one possible empty value
        /// or is at the edge of the search space.
        /// </summary>
        /// <param name="stepDirection"></param>
        /// <returns></returns>
        private HashSet<int> FindValidBoundaryPositions(int stepDirection, Pattern<char> pattern)
        {
            HashSet<int> Boundaries = new HashSet<int>();

            for (int i = 0; i < pattern.SearchSpace.Length; i++)
            {
                if (i + stepDirection < 0 || i + stepDirection >= pattern.SearchSpace.Length)
                    Boundaries.Add(i);
                else if (pattern.IsIndexPotentiallyEmpty(i + stepDirection))
                    Boundaries.Add(i);
            }
            return Boundaries;
        }

        /// <summary>
        /// Search the <see cref="IMinimalDAG{T}"/> for full sequences that match runs of <typeparamref name="T"/> values
        /// in the <paramref name="possibleValues"/> sequence, and consist only of values from the <paramref name="valuePool"/>
        /// (without replacements).
        /// </summary>
        /// <param name="valuePool">Values allowed in the matching sequence</param>
        /// <param name="possibleValues">The partial sequence(s) to match</param>
        /// <param name="emptyValue">A value in the <paramref name="possibleValues"/> that represents a currently empty space in the sequence.</param>
        /// <param name="wildCardCount">The number of wildcard values to use in the search</param>
        /// <returns></returns>
        public IEnumerable<DAGSearchResult<char>> FindMatchingSequences(List<char> valuePool, Pattern<char> pattern, int wildCardCount)
        {
            var StartingIndices = pattern.SequenceBoundaries.Select(x => x[1]).ToList();
            HashSet<int> UniqueStartingIndices = new HashSet<int>();
            foreach (var StartingIndex in StartingIndices)
            {
                if (!UniqueStartingIndices.Contains(StartingIndex))
                {
                    UniqueStartingIndices.Add(StartingIndex);
                    foreach (var Sequence in FindValidSequencesContainingIndex(valuePool, StartingIndex, pattern, wildCardCount))
                        yield return Sequence;
                }
            }
        }

        /// <summary>
        /// Search the <see cref="IMinimalDAG{T}"/> for full sequences that match runs of <typeparamref name="T"/> values
        /// in the <paramref name="possibleValues"/> sequence, and consist only of values from the <paramref name="valuePool"/>
        /// (without replacements).
        /// </summary>
        /// <param name="valuePool">Values allowed in the matching sequence</param>
        /// <param name="possibleValues">Some possible values for each index in the sequence.</param>
        /// <param name="emptyValue">A value in the <paramref name="existingValues"/> that represents a currently empty space in the sequence.</param>
        /// <param name="wildCardCount">The number of wildcard values to use in the search</param>
        /// <returns></returns>
        public IEnumerable<DAGSearchResult<char>> FindMatchingSequencesContainingIndex(List<char> valuePool, Pattern<char> pattern,
                                                                                    int wildCardCount, int index)
        {
            return FindValidSequencesContainingIndex(valuePool, index, pattern, wildCardCount);
        }

        /// <summary>
        /// Checks if a given sequence exists in its entirety in the DAG.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public bool Contains(IEnumerable<char> sequence) => _dawg.Contains(sequence);
    }
}
