
using IMinimalDAGInterfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations
{
    public class MinimalDAGNode<T> : IMinimalDAGNode<T>
    {
        //[JsonProperty]
        //public List<Guid> _parentsList { get; set; }
        //[JsonProperty]

        internal List<Guid> _childrenList { get; private set; }//shortterm hack

        [JsonProperty]
        public T Value { get; private set; }
        [JsonIgnore]
        public bool IsSuffixRegistered { get; set; }
        [JsonProperty]
        public Guid ID { get; private set; }
        //[JsonIgnore]
        public Dictionary<T, List<IMinimalDAGNode<T>>> Children { get; private set; } //TODO: pretty sure only parent references need to be a list with this implentation. consider fixing.
        public Dictionary<T, List<IMinimalDAGNode<T>>> Parents { get; private set; }

        public MinimalDAGNode(T value, Guid id)
        {
            Value = value;
            Parents = new Dictionary<T, List<IMinimalDAGNode<T>>>();
            Children = new Dictionary<T, List<IMinimalDAGNode<T>>>();
            ID = id;
        }

        public MinimalDAGNode(T value, Guid id, List<Guid> childIDs)
        {
            Value = value;
            ID = id;
            Parents = new Dictionary<T, List<IMinimalDAGNode<T>>>();
            Children = new Dictionary<T, List<IMinimalDAGNode<T>>>();
            _childrenList = childIDs;
        }

        public IList<Guid> GetChildIDs()
        {
            return _childrenList;
        }

        //[JsonConstructor]
        //public MinimalDAGNode(IEnumerable<Guid> Parents, IEnumerable<Guid> Children, T Value, Guid ID)
        //{
        //    this.Parents = new List<Guid>(Parents);
        //    this.Children = new List<Guid>(Children);
        //    this.Value = Value;
        //    this.ID = ID;
        //}


        /// <summary>
        /// Performs a limited equality check. They're considered to be equal if their value and child IDs are equivalent.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //same value, same children, therefore are exactly equal. Note this is basically a reference equality check for the children
            //as we're only checking their GUIDs
            if (obj != null && obj is MinimalDAGNode<T>)
            {
                var otherNode = (MinimalDAGNode<T>)obj;
                if (this.Value.Equals(otherNode.Value) && this.Children.Count == otherNode.Children.Count)
                {
                    var keys  = this.Children.Keys;
                    var other_keys = otherNode.Children.Keys;
                    if (keys.SequenceEqual(other_keys))
                    {
                        foreach (var key in keys)
                        {
                            var childIDs = this.Children[key].Select(x => x.ID);
                            var otherIDs = otherNode.Children[key].Select(x => x.ID);
                            if (!childIDs.SequenceEqual(otherIDs))
                                return false;
                        }
                        return true;
                    }
                    else
                        return false;
                    //var ids = this.Children.Values.Select(x => x.ID);
                    //var other_ids = otherNode.Children.Values.Select(x => x.ID);
                    //return ids.SequenceEqual(other_ids);
                    ////foreach(var child in this.Children)
                    //{
                    //    if (otherNode.Children.TryGetValue(child.Key, out IMinimalDAGNode<T> otherChild))
                    //    {
                    //        if (otherNode.Children[child.Key] != child.Value)
                    //            return false;
                    //    }
                    //    else
                    //        return false;
                    //}
                    //return true;
                    //return this.Children.SequenceEqual(otherNode.Children);
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the hashcode for this <see cref="MinimalDAGNode{T}"/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var con = "";
                con += Value.GetHashCode();
                foreach (var child in Children)
                    con += child.Value.GetHashCode();

                return con.GetHashCode();
            }
        }

    }
}


