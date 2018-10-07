using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGSearcher
{
    /// <summary>
    /// An encapsulation of data returned from a search of a <see cref="IMinimalDAG{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DAGSearchResult<T>
    {
        /// <summary>
        /// The sequence recovered.
        /// </summary>
        public IEnumerable<T> MatchingSequence { get; private set; }

        /// <summary>
        /// The position(s) of any wild cards in the recovered sequence.
        /// </summary>
        public List<int> WildCardPositions { get; private set; }

        /// <summary>
        /// The starting index of the recovered sequence, relative to the <see cref="MinimalDAGSearcher"/> search space.
        /// </summary>
        public int SequenceStartIndex { get; private set; }

        /// <summary>
        /// The indices at which elements were added
        /// </summary>
        public Dictionary<int, T> AdditionLocations { get; set; }
        
        public DAGSearchResult(IEnumerable<T> matchingSequence, List<int> wildCardPositions, int startIndex, Dictionary<int, T> newElementPositions)
        {
            MatchingSequence = matchingSequence;
            WildCardPositions = wildCardPositions;
            SequenceStartIndex = startIndex;
            AdditionLocations = newElementPositions;
        }
    }
}
