using IMinimalDAGInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGSearcher.Interfaces
{
    /// <summary>
    /// Has search functionality for a <see cref="IMinimalDAG{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMinimalDAGSearcher<T>
    {
        /// <summary>
        /// Search the <see cref="IMinimalDAG{T}"/> for full sequences that match runs of <typeparamref name="T"/> values
        /// in the <paramref name="pattern"/>, and consist only of values from the <paramref name="valuePool"/>
        /// (without replacements).
        /// </summary>
        /// <param name="valuePool">Values allowed in the matching sequence</param>
        /// <param name="pattern">The pattern to attempt to match.</param>
        /// <param name="wildCardCount">The number of wildcard values to use in the search</param>
        /// <returns></returns>
        IEnumerable<DAGSearchResult<T>> FindMatchingSequences(IEnumerable<T> valuePool, IPattern<T> pattern, int wildCardCount);

        /// <summary>
        /// Returns a bool representing if the DAG contains the selected sequence (as a whole sequence)
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<T> sequence);

        /// <summary>
        /// Search the <see cref="IMinimalDAG{T}"/> for full sequences that match runs of <typeparamref name="T"/> values (and contain the
        /// element at <paramref name="index"/>)
        /// in the <paramref name="pattern"/>, and consist only of values from the <paramref name="valuePool"/>
        /// (without replacements).
        /// </summary>
        /// <param name="valuePool">Values allowed in the matching sequence</param>
        /// <param name="pattern">The pattern to attempt to match.</param>
        /// <param name="wildCardCount">The number of wildcard values to use in the search</param>
        /// <returns></returns>
        IEnumerable<DAGSearchResult<T>> FindMatchingSequencesContainingIndex(IEnumerable<T> valuePool, IPattern<T> pattern, int wildCardCount, int index);
    }
}
