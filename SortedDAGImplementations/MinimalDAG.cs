using IMinimalDAGInterfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinimalDAGImplementations
{
    /// <summary>
    /// A minimal directed acyclic graph representing a collection of sequences of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MinimalDAG<T> : IMinimalDAG<T>
    {
        [JsonProperty]
        IMinimalDAGNode<T> _source;
        [JsonProperty]
        IMinimalDAGNode<T> _sink;
        [JsonProperty]
        IMinimalDAGNodeFactory<T> _dagNodeFactory;
        [JsonProperty]
        Dictionary<Guid, IMinimalDAGNode<T>> _nodes;
        [JsonProperty]
        IDictionary<T, List<Guid>> _nodeIDsByValue;
        [JsonProperty]
        HashSet<Guid> _nodeIDsWithNullValue;

        HashSet<IMinimalDAGNode<T>> _registrationBySuffix;

        /// <summary>
        /// Initializes and incrementally creates a minimal <see cref="IMinimalDAGNode{T}"/> from a sorted collection of sequences of values.
        /// </summary>
        /// <typeparam name="T">The type to be arranged as a <see cref="IMinimalDAG{T}"/></typeparam>
        /// <param name="sortedSequences">The ascending ordered sequences of <typeparamref name="T"/> to be minimized.</param>
        /// <param name="nodeFactory">Factory for <see cref="IMinimalDAGNode{T}"/> creation</param>
        public MinimalDAG(IEnumerable<IEnumerable<T>> sortedSequences, IMinimalDAGNodeFactory<T> nodeFactory)//, T sourceValue = default(T), T sinkValue = default(T))
        {

            _registrationBySuffix = new HashSet<IMinimalDAGNode<T>>();
            _dagNodeFactory = nodeFactory ?? new MinimalDAGNodeFactory<T>();
            _nodes = new Dictionary<Guid, IMinimalDAGNode<T>>();
            _source = _dagNodeFactory.CreateNode(default(T), Guid.NewGuid());
            _sink = _dagNodeFactory.CreateNode(default(T), Guid.NewGuid());
            _source.Children.Add(_sink.ID);

            _registrationBySuffix.Add(_sink);
            _sink.IsSuffixRegistered = true;

            var LastSequence = new List<T>() { _sink.Value }.DefaultIfEmpty(); //lazy but simple fix
            foreach (IEnumerable<T> Sequence in sortedSequences)
            {
                AddNextOrderedSequence(Sequence, LastSequence);
                LastSequence = Sequence;
            }
            HandleExistingSuffixes(_source);
            BuildParentLinks();
            BuildNodesByValueDictionary();
        }

        /// <summary>
        /// Creates a <see cref="IMinimalDAGNode{T}"/> directly from required fields. Intended for use with JSON deserialization.
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="_sink"></param>
        /// <param name="_dagNodeFactory"></param>
        /// <param name="_nodes"></param>
        /// <param name="_nodeIDsByValue"></param>
        [JsonConstructor]
        public MinimalDAG(IMinimalDAGNode<T> _source, IMinimalDAGNode<T> _sink, IMinimalDAGNodeFactory<T> _dagNodeFactory, 
                          Dictionary<Guid, IMinimalDAGNode<T>> _nodes, Dictionary<T, List<Guid>> _nodeIDsByValue, HashSet<Guid> _nodeIDsWithNullValue)
        {
            this._sink = _sink;
            this._source = _source;
            this._nodeIDsByValue = _nodeIDsByValue;
            this._nodes = _nodes;
            this._dagNodeFactory = _dagNodeFactory;
            this._nodeIDsWithNullValue = _nodeIDsWithNullValue;
        }

        /// <summary>
        /// Records a <see cref="IMinimalDAGNode{T}"/> into the register of nodes. 
        /// </summary>
        /// <param name="toRecord"></param>
        private void RecordNode(IMinimalDAGNode<T> toRecord)
        {
            _nodes[toRecord.ID] = toRecord;
        }

        /// <summary>
        /// Returns the last (ie most recently added) child <see cref="IMinimalDAGNode{T}"/> linked to the selected node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IMinimalDAGNode<T> GetLastChild(IMinimalDAGNode<T> node)
        {
            IMinimalDAGNode<T> ReturnValue = null;
            _nodes.TryGetValue(node.Children.Last(), out ReturnValue);
            return ReturnValue;
            //return Nodes[node.GetChildIDs().Last()];
        }

        /// <summary>
        /// Traverses the graph from a specified node, returning all sequences with that parent.
        /// </summary>
        /// <param name="root">The starting point of the sequence search.</param>
        /// <param name="partial">The current sequence/s being traversed</param>
        /// <returns></returns>
        private List<List<T>> GetSequences(IMinimalDAGNode<T> root, List<T> partial)
        {
            List<List<T>> Values = new List<List<T>>();
            if (root.ID.Equals(_sink.ID))
            {
                Values.Add(partial);
                return Values;
            }
            List<T> Sequence = new List<T>(partial) { root.Value };
            foreach (var Child in GetChildren(root))
            {
                foreach (var Subsequence in GetSequences(Child, Sequence))
                    Values.Add(Subsequence);
            }
            return Values;
        }

        /// <summary>
        /// Populate the NodesByValue lookup dictionary.
        /// </summary>
        private void BuildNodesByValueDictionary()
        {
            _nodeIDsByValue = new Dictionary<T, List<Guid>>();
            _nodeIDsWithNullValue = new HashSet<Guid>();
            foreach (IMinimalDAGNode<T> Node in _registrationBySuffix)
            {
                var Value = Node.Value;
                if (Value != null)
                {
                    if (_nodeIDsByValue.ContainsKey(Value))
                        _nodeIDsByValue[Value].Add(Node.ID);
                    else
                        _nodeIDsByValue.Add(Value, new List<Guid>() { Node.ID });
                }
                else
                    _nodeIDsWithNullValue.Add(Node.ID);
            }
        }

        /// <summary>
        /// Add parent links to the DAG.
        /// </summary>
        private void BuildParentLinks()
        {
            foreach (var Node in _registrationBySuffix)
                foreach (var Child in GetChildren(Node))
                    Child.Parents.Add(Node.ID);
            foreach (var Child in GetChildren(_source))
                Child.Parents.Add(_source.ID);
        }

        /// <summary>
        /// Adds a sequence of child <see cref="IMinimalDAGNode{T}"/> nodes to the selected <see cref="IMinimalDAGNode{T}"/>
        /// - the addition of a new sequence of values to the DAG.
        /// </summary>
        /// <param name="currentNode">The parent node to add the sequence to.</param>
        /// <param name="currentSuffix">A new sequence of nodes.</param>
        private void AddSuffix(IMinimalDAGNode<T> currentNode, IEnumerable<T> currentSuffix)
        {
            var CurrentNode = currentNode;
            foreach (var value in currentSuffix)
            {
                var NewNode = _dagNodeFactory.CreateNode(value, Guid.NewGuid());
                RecordNode(NewNode);
                CurrentNode.Children.Add(NewNode.ID);
                CurrentNode = NewNode;
            }
            CurrentNode.Children.Add(_sink.ID);
        }

        /// <summary>
        /// Compares the most recently added <see cref="IMinimalDAGNode{T}"/> (and children) to the existing nodes, 
        /// merging any matches.
        /// </summary>
        /// <param name="parent"></param>
        private void HandleExistingSuffixes(IMinimalDAGNode<T> parent)
        {
            var NewestChild = GetLastChild(parent);

            if (NewestChild != default(IMinimalDAGNode<T>))
            {
                if (!NewestChild.IsSuffixRegistered)
                {
                    if (NewestChild.Children.Count > 0)
                        HandleExistingSuffixes(NewestChild);
                    else
                        NewestChild.Children.Add(_sink.ID);

                    IMinimalDAGNode<T> EquivalentNode;

                    if (_registrationBySuffix.TryGetValue(NewestChild, out EquivalentNode))
                    {
                        parent.Children.Remove(NewestChild.ID);

                        _nodes.Remove(NewestChild.ID);
                        parent.Children.Add(EquivalentNode.ID);
                    }
                    else
                    {
                        _registrationBySuffix.Add(NewestChild);
                        NewestChild.IsSuffixRegistered = true;
                    }
                }
            }
        }

        /// <summary>
        /// Walks down the most recently added nodes (matching the <paramref name="longestCommonPrefix"/> nodes)
        /// and returns the last such node.
        /// </summary>
        /// <param name="longestCommonPrefix"></param>
        /// <returns></returns>
        private IMinimalDAGNode<T> TraverseSequence(IEnumerable<T> longestCommonPrefix)
        {
            var CurrentNode = _source;
            foreach (var Value in longestCommonPrefix)
                CurrentNode = GetLastChild(CurrentNode);

            return CurrentNode;
        }

        /// <summary>
        /// Adds the next sequence to the DAG.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="previousSequence"></param>
        public void AddNextOrderedSequence(IEnumerable<T> sequence, IEnumerable<T> previousSequence)
        {
            var LongestCommonPrefix = previousSequence.LongestCommonPrefix(sequence);

            var CurrentNode = TraverseSequence(LongestCommonPrefix);
            var CurrentSuffix = sequence.Skip(LongestCommonPrefix.Count());

            HandleExistingSuffixes(CurrentNode);
            AddSuffix(CurrentNode, CurrentSuffix);

        }


        /// <summary>
        /// Returns a collection of <see cref="IMinimalDAGNode{T}"/> where the value of <typeparamref name="T"/> equals <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IEnumerable<IMinimalDAGNode<T>> GetAllNodesWithValue(T value)
        {
            if (value != null)
            {
                foreach (var ID in _nodeIDsByValue[value])
                    yield return _nodes[ID];
            }
            else
                foreach (var ID in _nodeIDsWithNullValue)
                    yield return _nodes[ID];

        }

        /// <summary>
        /// Returns a boolean representing whether <paramref name="possibleBoundaryNode"/> is a sink or source <see cref="IMinimalDAGNode{T}"/>.
        /// </summary>
        /// <param name="possibleBoundaryNode"></param>
        /// <returns></returns>
        public bool IsDAGBoundary(IMinimalDAGNode<T> possibleBoundaryNode)
        {
            var ID = possibleBoundaryNode.ID;
            if (ID == _sink.ID || ID == _source.ID)
                return true;
            return false;
        }

        /// <summary>
        /// Returns a boolean representing whether this <see cref="IMinimalDAG{T}"/> contains <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public bool Contains(IEnumerable<T> sequence)
        {
            var Current = _source;
            foreach (var Value in sequence)
            {
                Current = GetChildren(Current).FirstOrDefault(x => x.Value.Equals(Value));
                if (Current == null || Current.Value.Equals(default(T)))
                    return false;

            }
            Current = GetChildren(Current).FirstOrDefault(x => x.Equals(_sink));

            if (Current == null)
                return false;

            return true;
        }

        /// <summary>
        /// Returns the <see cref="IMinimalDAGNode{T}"/> children of <paramref name="node"/>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IEnumerable<IMinimalDAGNode<T>> GetChildren(IMinimalDAGNode<T> node)
        {
            IMinimalDAGNode<T> value;
            foreach (var ID in node.Children)
            {
                if (_nodes.TryGetValue(ID, out value))
                    yield return value;
                else if (ID == _sink.ID)
                    yield return _sink;
            }
        }

        /// <summary>
        /// Returns the <see cref="IMinimalDAGNode{T}"/> parents of <paramref name="node"/>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IEnumerable<IMinimalDAGNode<T>> GetParents(IMinimalDAGNode<T> node)
        {
            IMinimalDAGNode<T> value;
            foreach (var ID in node.Parents)
            {
                if (_nodes.TryGetValue(ID, out value))
                    yield return value;
                else if (ID == _source.ID)
                    yield return _source;
            }
                //yield return _nodes[ID];
        }

        /// <summary>
        /// Recover a collection of all sequences used to create the <see cref="MinimalDAG{T}"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<List<T>> GetAllSequences()
        {
            var Values = new List<List<List<T>>>();
            foreach (var child in GetChildren(_source))
                Values.Add(GetSequences(child, new List<T>()));

            return Values.SelectMany(x => x).Where(x => x.Count > 0).ToList();
        }

        public IEnumerable<IMinimalDAGNode<T>> GetAllValidEndNodes() => GetParents(_sink);

        public IEnumerable<IMinimalDAGNode<T>> GetAllValidStartNodes() => GetChildren(_source);

        public override bool Equals(object obj)
        {
            if(obj != null && obj is MinimalDAG<T>)
            {
                var otherDAG = obj as MinimalDAG<T>;

                _nodes.SequenceEqual(otherDAG._nodes);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _nodes.GetHashCode();
        }
    }
}
