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
        //[JsonProperty]
        public IMinimalDAGNode<T> Source { get; private set; }
        //[JsonProperty]
        public IMinimalDAGNode<T> Sink { get; private set; }
        //[JsonProperty]
        internal IMinimalDAGNodeFactory<T> dagNodeFactory { get; private set; }
        //[JsonProperty]
        public Dictionary<Guid, IMinimalDAGNode<T>> Nodes { get; private set; }
        //[JsonProperty]
        IDictionary<T, List<IMinimalDAGNode<T>>> _nodeIDsByValue;
        //[JsonProperty]
        HashSet<IMinimalDAGNode<T>> _nodeIDsWithNullValue;

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
            dagNodeFactory = nodeFactory ?? new MinimalDAGNodeFactory<T>();
            Nodes = new Dictionary<Guid, IMinimalDAGNode<T>>();
            Source = dagNodeFactory.CreateNode(default(T), Guid.NewGuid());
            Sink = dagNodeFactory.CreateNode(default(T), Guid.NewGuid());
            Source.Children.Add(Sink.Value, new List<IMinimalDAGNode<T>>() { Sink });

            _registrationBySuffix.Add(Sink);
            Sink.IsSuffixRegistered = true;

            var LastSequence = new List<T>() { Sink.Value }.DefaultIfEmpty(); //lazy but simple fix
            foreach (IEnumerable<T> Sequence in sortedSequences)
            {
                AddNextOrderedSequence(Sequence, LastSequence);
                LastSequence = Sequence;
            }
            //HandleExistingSuffixes(Source);
            //BuildParentLinks();
            //BuildNodesByValueDictionary();
        }

        ///// <summary>
        ///// Creates a <see cref="IMinimalDAGNode{T}"/> directly from required fields. Intended for use with JSON deserialization.
        ///// </summary>
        ///// <param name="_source"></param>
        ///// <param name="_sink"></param>
        ///// <param name="_dagNodeFactory"></param>
        ///// <param name="_nodes"></param>
        ///// <param name="_nodeIDsByValue"></param>
        //[JsonConstructor]
        //public MinimalDAG(IMinimalDAGNode<T> _source, IMinimalDAGNode<T> _sink, IMinimalDAGNodeFactory<T> _dagNodeFactory, 
        //                  Dictionary<Guid, IMinimalDAGNode<T>> _nodes, Dictionary<T, List<Guid>> _nodeIDsByValue, HashSet<Guid> _nodeIDsWithNullValue)
        //{
        //    this.Sink = _sink;
        //    this.Source = _source;
        //    this._nodeIDsByValue = _nodeIDsByValue;
        //    this.Nodes = _nodes;
        //    this._dagNodeFactory = _dagNodeFactory;
        //    this._nodeIDsWithNullValue = _nodeIDsWithNullValue;
        //}

        public MinimalDAG(IMinimalDAGNode<T> source, IMinimalDAGNode<T> sink, IMinimalDAGNodeFactory<T> nodeFactory, Dictionary<Guid, IMinimalDAGNode<T>> nodes)
        {
            //todo: will replace old JSONconstructor version with this. results in smaller json, and the general changes result in faster serialization. possibly marginally slower
            //deserialization, but it depends on the relative speed of the deserializer compared to a couple of iterations through Nodes.
            this.Source = source;
            this.Sink = sink;
            this._nodeIDsByValue = new Dictionary<T, List<IMinimalDAGNode<T>>>();
            this._nodeIDsWithNullValue = new HashSet<IMinimalDAGNode<T>>();
            this.Nodes = nodes;
            
            foreach(var kvp in Nodes)
            {
                var children = kvp.Value.GetChildIDs();
                foreach(var child in children)
                {
                    var childNode = Nodes[child];
                    var parentNode = Nodes[kvp.Key];
                    Nodes[kvp.Key].Children.Add(childNode.Value, new List<IMinimalDAGNode<T>>() { childNode });
                    Nodes[child].Parents.Add(parentNode.Value, new List<IMinimalDAGNode<T>>() { parentNode});
                    //TODO: handle reverse lookup stuff.
                }
                if(kvp.Value.Value == null)
                    _nodeIDsWithNullValue.Add(kvp.Value);
                else
                {
                    if (!_nodeIDsByValue.ContainsKey(kvp.Value.Value))
                        _nodeIDsByValue.Add(kvp.Value.Value, new List<IMinimalDAGNode<T>>() { kvp.Value });
                    else
                        _nodeIDsByValue[kvp.Value.Value].Add(kvp.Value);
                }   
            }
        }

        /// <summary>
        /// Records a <see cref="IMinimalDAGNode{T}"/> into the register of nodes. 
        /// </summary>
        /// <param name="toRecord"></param>
        private void RecordNode(IMinimalDAGNode<T> toRecord)
        {
            Nodes[toRecord.ID] = toRecord;
        }

        /// <summary>
        /// Returns the last (ie most recently added) child <see cref="IMinimalDAGNode{T}"/> linked to the selected node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        //private IMinimalDAGNode<T> GetLastChild(IMinimalDAGNode<T> node)
        //{
        //    return (node.Children.Count > 0) ? node.Children.Values.Last() : null;
        //    //if node.Children.Count == 0)node.Children.Last();
        //    //IMinimalDAGNode<T> ReturnValue = null;
        //    //Nodes.TryGetValue(node.Children.Last(), out ReturnValue);
        //    //return ReturnValue;
        //    //return Nodes[node.GetChildIDs().Last()];
        //}

        /// <summary>
        /// Traverses the graph from a specified node, returning all sequences with that parent.
        /// </summary>
        /// <param name="root">The starting point of the sequence search.</param>
        /// <param name="partial">The current sequence/s being traversed</param>
        /// <returns></returns>
        private List<List<T>> GetSequences(IMinimalDAGNode<T> root, List<T> partial)
        {
            List<List<T>> Values = new List<List<T>>();
            if (root.ID.Equals(Sink.ID))
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
            _nodeIDsByValue = new Dictionary<T, List<IMinimalDAGNode<T>>>();// new Dictionary<T, List<Guid>>();
            _nodeIDsWithNullValue = new HashSet<IMinimalDAGNode<T>>();
            foreach (IMinimalDAGNode<T> Node in _registrationBySuffix)
            {
                var Value = Node.Value;
                if (Value != null)
                {
                    if (_nodeIDsByValue.ContainsKey(Value))
                        _nodeIDsByValue[Value].Add(Node);
                    else
                        _nodeIDsByValue.Add(Value, new List<IMinimalDAGNode<T>>() { Node });
                }
                else
                    _nodeIDsWithNullValue.Add(Node);
            }
        }

        /// <summary>
        /// Add parent links to the DAG.
        /// </summary>
        //private void BuildParentLinks()
        //{
        //    foreach (var Node in _registrationBySuffix)
        //        foreach (var child in Node.Children.Values)
        //            child.Parents.Add(Node.Value, Node);

        //    foreach (var child in Source.Children.Values)
        //        child.Parents.Add(Source.Value, Source);
        //    //foreach (var Node in _registrationBySuffix)
        //    //    foreach (var Child in GetChildren(Node))
        //    //        Child.Parents.Add(Node.Value, Node);
        //    //foreach (var Child in GetChildren(Source))
        //    //    Child.Parents.Add(Source.Value, Source);
        //}

        /// <summary>
        /// Adds a sequence of child <see cref="IMinimalDAGNode{T}"/> nodes to the selected <see cref="IMinimalDAGNode{T}"/>
        /// - the addition of a new sequence of values to the DAG.
        /// </summary>
        /// <param name="currentNode">The parent node to add the sequence to.</param>
        /// <param name="currentSuffix">A new sequence of nodes.</param>
        //private void AddSuffix(IMinimalDAGNode<T> currentNode, IEnumerable<T> currentSuffix)
        //{
        //    var CurrentNode = currentNode;
        //    foreach (var value in currentSuffix)
        //    {
        //        var NewNode = dagNodeFactory.CreateNode(value, Guid.NewGuid());
        //        RecordNode(NewNode);
        //        CurrentNode.Children.Add(NewNode.Value, NewNode);
        //        NewNode.Parents.Add(CurrentNode.Value, CurrentNode);
        //        CurrentNode = NewNode;
        //    }
        //    CurrentNode.Children.Add(Sink.Value, Sink);
        //}

        /// <summary>
        /// Compares the most recently added <see cref="IMinimalDAGNode{T}"/> (and children) to the existing nodes, 
        /// merging any matches.
        /// </summary>
        /// <param name="parent"></param>
        //private void HandleExistingSuffixes(IMinimalDAGNode<T> parent)
        //{
        //    //TODO: rewrite DAG creation with parent links established (as we're using classic dictionary parent/child edges now)
        //    //SO we look at the suffix most recently added, working from sink backwards, merging nodes as required. Break at first node we reach that has
        //    //already been handled, or cannot be merged.
        //    var lowest_descendent = parent;
        //    while(lowest_descendent.Children.Count > 0)
        //    {
        //        var last_child = lowest_descendent.Children.Values.Last();
        //        if (last_child == Sink)
        //            break;
        //        lowest_descendent = last_child;
        //    }
        //    var lowest_linked = Sink;

        //    while (lowest_linked.Parents.ContainsKey(lowest_descendent.Value))
        //    {
        //        //update links etc
        //        foreach (var p in lowest_descendent.Parents.Values)
        //        {
        //            //update references
        //            p.Children[lowest_descendent.Value] = lowest_linked.Parents[lowest_descendent.Value]; 
                    
        //            foreach(var par in lowest_linked.Parents)
        //                //
        //        }


        //    }



        //    var NewestChild = GetLastChild(parent);

        //    if (NewestChild != default(IMinimalDAGNode<T>) && NewestChild != Sink)
        //    {
        //        //if the most recent addition hasn't already been registered, check for the existence of an equivalent node.
        //        //If an equivalent node already exists, merge this node into it (ie delete this node, and update references to point to the
        //        //existing node
        //        if (!NewestChild.IsSuffixRegistered)
        //        {
        //            if (NewestChild.Children.Count > 0)
        //                HandleExistingSuffixes(NewestChild);
        //            else
        //                NewestChild.Children.Add(Sink.Value, Sink);

        //            if (_registrationBySuffix.TryGetValue(NewestChild, out IMinimalDAGNode<T> EquivalentNode)) //very slow with bad hash/equal function.
        //            {
        //                parent.Children.Remove(NewestChild.Value);

        //                Nodes.Remove(NewestChild.ID);
        //                parent.Children.Add(EquivalentNode.Value, EquivalentNode);
        //            }
        //            else
        //            {
        //                _registrationBySuffix.Add(NewestChild);
        //                NewestChild.IsSuffixRegistered = true;
                        
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Walks down the most recently added nodes (matching the <paramref name="longestCommonPrefix"/> nodes)
        /// and returns the last such node.
        /// </summary>
        /// <param name="longestCommonPrefix"></param>
        /// <returns></returns>
        private IMinimalDAGNode<T> TraverseSequence(IEnumerable<T> longestCommonPrefix)
        {
            //going 'forward' there shouldn't be multiple node choices (ie c.child[a].child[t] == cat safely)
            var CurrentNode = Source;
            foreach (var Value in longestCommonPrefix)
                CurrentNode = CurrentNode.Children[Value].Last();

            return CurrentNode;
            //var CurrentNode = Source;
            //foreach (var Value in longestCommonPrefix)
            //    CurrentNode = GetLastChild(CurrentNode);

            //return CurrentNode;
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
            AddSuffix(CurrentSuffix, CurrentNode);

            //HandleExistingSuffixes(CurrentNode);
            //AddSuffix(CurrentNode, CurrentSuffix);

        }

        /// <summary>
        /// Adds the specified suffix to the specified node in the IMinimalDAG<typeparamref name="T"/>,
        /// merging nodes as required.
        /// </summary>
        /// <param name="suffix"></param>
        /// <param name="currentNode"></param>
        public void AddSuffix(IEnumerable<T> suffix, IMinimalDAGNode<T> currentNode)
        {
            var reversed = suffix.Reverse().ToList();
            var currentSuffixElementValue = reversed[0];
            var postTailNode = Sink;
            int visitedCount = 0;
            var current_tail_children_values = new List<T>() { Sink.Value };
            bool matched = false;

             //marge while the value (and values of children) are identical
            while(visitedCount < reversed.Count)
            {
                currentSuffixElementValue = reversed[visitedCount];
                if (postTailNode.Parents.ContainsKey(currentSuffixElementValue)) //has node(s) with same value, check child values
                {
                    var sameValueNodes = postTailNode.Parents[currentSuffixElementValue];
                    foreach(var node in sameValueNodes)
                    {
                        var childValues = node.Children.Values.SelectMany(x => x.Select(y => y.Value)).ToList();
                        if (childValues.SequenceEqual(current_tail_children_values))
                        {
                            postTailNode = node;
                            visitedCount++;
                            current_tail_children_values = childValues;
                            matched = true;
                            break;
                        }
                    }
                    if (!matched)
                        break;
                    else
                        matched = false;
                    //postTailNode = postTailNode.Parents[currentSuffixElementValue];
                    //visitedCount++;            
                }
                else
                    break;
            }

            //all nodes but the very last (or first) that can't be merged must be created
            //and linked
            while(visitedCount < reversed.Count)
            {
                currentSuffixElementValue = reversed[visitedCount];
                var next_node = dagNodeFactory.CreateNode(currentSuffixElementValue, Guid.NewGuid());
                if (!postTailNode.Parents.ContainsKey(currentSuffixElementValue))
                    postTailNode.Parents.Add(currentSuffixElementValue, new List<IMinimalDAGNode<T>>() { next_node });
                else
                    postTailNode.Parents[currentSuffixElementValue].Add(next_node);

                next_node.Children.Add(postTailNode.Value, new List<IMinimalDAGNode<T>>() { postTailNode });
                postTailNode = next_node;
                visitedCount++;
            }
            if (!postTailNode.Parents.ContainsKey(currentNode.Value))
                postTailNode.Parents.Add(currentNode.Value, new List<IMinimalDAGNode<T>>() { currentNode });
            else
                postTailNode.Parents[currentNode.Value].Add(currentNode);

            if (!currentNode.Children.ContainsKey(postTailNode.Value))
                currentNode.Children.Add(postTailNode.Value, new List<IMinimalDAGNode<T>>() { postTailNode });
            else
                currentNode.Children[postTailNode.Value].Add(postTailNode);
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
                    yield return ID;
            }
            else
                foreach (var ID in _nodeIDsWithNullValue)
                    yield return ID;

        }

        /// <summary>
        /// Returns a boolean representing whether <paramref name="possibleBoundaryNode"/> is a sink or source <see cref="IMinimalDAGNode{T}"/>.
        /// </summary>
        /// <param name="possibleBoundaryNode"></param>
        /// <returns></returns>
        public bool IsDAGBoundary(IMinimalDAGNode<T> possibleBoundaryNode)
        {
            var ID = possibleBoundaryNode.ID;
            if (ID == Sink.ID || ID == Source.ID)
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
            var Current = Source;
            foreach (var Value in sequence)
            {
                Current = GetChildren(Current).FirstOrDefault(x => x.Value.Equals(Value));
                if (Current == null || Current.Value.Equals(default(T)))
                    return false;

            }
            Current = GetChildren(Current).FirstOrDefault(x => x.Equals(Sink));

            if (Current == null)
                return false;

            return true;
        }

        /// <summary>
        /// Returns the <see cref="IMinimalDAGNode{T}"/> children of <paramref name="node", with an optional set of allowed T values/>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="filter">Optional filter values</param>
        /// <returns></returns>
        public IEnumerable<IMinimalDAGNode<T>> GetChildren(IMinimalDAGNode<T> node, HashSet<T> filter = null)
        {
            if (filter != null)
            {
                foreach (var allowedValue in filter)
                    if (node.Children.TryGetValue(allowedValue, out List<IMinimalDAGNode<T>> matching))
                        foreach (var match in matching)
                            yield return match;
            }
            else
            {
                foreach (var childList in node.Children.Values)
                    foreach (var child in childList)
                        yield return child;
            }


            //if (filter != null)
            //{
            //    foreach (var child in GetChildrenFiltered(node, filter))
            //        yield return child;
            //}
            //else
            //{
            //    foreach (var childNode in node.Children)
            //    {
            //        if (Nodes.TryGetValue(childNode, out IMinimalDAGNode<T> child))
            //            yield return child;
            //        else if (childNode == Sink.ID)
            //            yield return Sink;
            //    }
            //}
        }

        //private IEnumerable<IMinimalDAGNode<T>> GetChildrenFiltered(IMinimalDAGNode<T> node, HashSet<T> filter)
        //{
        //    foreach(var ID in node.Children)
        //    {
        //        if (Nodes.TryGetValue(ID, out IMinimalDAGNode<T> child))
        //            if (filter.Contains(child.Value))
        //                yield return child;
        //    }
        //}



        /// <summary>
        /// Returns the <see cref="IMinimalDAGNode{T}"/> parents of <paramref name="node", with an optional set of allowed values./>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IEnumerable<IMinimalDAGNode<T>> GetParents(IMinimalDAGNode<T> node, HashSet<T> filter = null)
        {
            if (filter != null)
            {
                foreach (var allowedValue in filter)
                    if (node.Parents.TryGetValue(allowedValue, out List<IMinimalDAGNode<T>> parentList))
                        foreach(var parent in parentList)
                            yield return parent;
            }
            else
            {
                foreach (var parentList in node.Parents.Values)
                    foreach(var parent in parentList)
                        yield return parent;
            }
            //if (filter != null)
            //{
            //    foreach (var child in GetParentsFiltered(node, filter))
            //        yield return child;
            //}
            //else
            //{
            //    foreach (var ID in node.Parents)
            //    {
            //        if (Nodes.TryGetValue(ID, out IMinimalDAGNode<T> parent))
            //            yield return parent;
            //        else if (ID == Source.ID)
            //            yield return Source;
            //    }
            //}
        }

        //public IEnumerable<IMinimalDAGNode<T>> GetParentsFiltered(IMinimalDAGNode<T>node, HashSet<T> filter)
        //{
        //    foreach (var ID in node.Parents)
        //    {
        //        if (Nodes.TryGetValue(ID, out IMinimalDAGNode<T> child))
        //            if (filter.Contains(child.Value))
        //                yield return child;
        //    }
        //}


        /// <summary>
        /// Recover a collection of all sequences used to create the <see cref="MinimalDAG{T}"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<List<T>> GetAllSequences()
        {
            var Values = new List<List<List<T>>>();
            foreach (var child in GetChildren(Source))
                Values.Add(GetSequences(child, new List<T>()));

            return Values.SelectMany(x => x).Where(x => x.Count > 0).ToList();
        }

        public IEnumerable<IMinimalDAGNode<T>> GetAllValidEndNodes() => GetParents(Sink);

        public IEnumerable<IMinimalDAGNode<T>> GetAllValidStartNodes() => GetChildren(Source);

        public override bool Equals(object obj)
        {
            if(obj != null && obj is MinimalDAG<T>)
            {
                var otherDAG = obj as MinimalDAG<T>;

                Nodes.SequenceEqual(otherDAG.Nodes);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Nodes.GetHashCode();
        }
    }
}
