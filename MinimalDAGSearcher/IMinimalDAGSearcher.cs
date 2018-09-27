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
        /// in the <paramref name="existingValues"/> sequence, and consist only of values from the <paramref name="valuePool"/>
        /// (without replacements).
        /// </summary>
        /// <param name="valuePool">Values allowed in the matching sequence</param>
        /// <param name="existingValues">The partial sequence(s) to match</param>
        /// <param name="emptyValue">A value in the <paramref name="existingValues"/> that represents a currently empty space in the sequence.</param>
        ///         /// <param name="wildCardCount">The number of wildcard values to use in the search</param>
        /// <returns></returns>
        IEnumerable<DAGSearchResult<T>> FindMatchingSequences(IEnumerable<T> valuePool,  T[] existingValues, T emptyValue, int wildCardCount);

        /// <summary>
        /// Returns a bool representing if the DAG contains the selected sequence (as a whole sequence)
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<T> sequence);

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
        IEnumerable<DAGSearchResult<T>> FindMatchingSequencesContainingIndex(IEnumerable<T> valuePool, T[] existingValues, T emptyValue, int wildCardCount, int index);
    }
}
