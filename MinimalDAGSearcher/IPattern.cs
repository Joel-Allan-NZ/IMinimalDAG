using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGSearcher
{
    /// <summary>
    /// A search pattern for a <see cref="IMinimalDAGSearcher{T}"/>representing a partial sequence to match.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPattern<T>
    {
        /// <summary>
        /// Acceptable values for each possible index. Null values simply have no restriction, and indices containing
        /// the EmptyValue either have no restriction (if it's the only value in the collection) or represents a possibly
        /// empty space.
        /// </summary>
        HashSet<T>[] SearchSpace { get; }

        /// <summary>
        /// The boundary indices of any possible sequence in the SearchSpace. These end points are indices
        /// that are preceeded/succeeded by empty space.
        /// </summary>
        List<int[]> SequenceBoundaries { get; }

        /// <summary>
        /// A value representing an empty space in the SearchSpace.
        /// </summary>
        T EmptyValue { get; }

        /// <summary>
        /// Checks if a given index in the search space has a concrete (ie singular) value.
        /// </summary>
        /// <param name="index"></param>
        bool IsIndexConcreteValue(int index);

        /// <summary>
        /// Checks if a given index in the search space is empty (ie has no restriction on acceptable values).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsIndexEmpty(int index);

        /// <summary>
        /// Checks if a given index is part of a sequence leading back to the first index in the searchspace.
        /// This is also true when there are no sequence boundaries between the given index and the first index, as no sequence
        /// could terminate in that space.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsPrefixMaximalAtIndex(int index);

        /// <summary>
        /// Attempts to retrieve a concrete (ie singular) value from a given index in the search space, returning a boolean
        /// representing success. Returns false if there is no concrete value at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetConcreteValue(int index, out T value);

        
    }
}
