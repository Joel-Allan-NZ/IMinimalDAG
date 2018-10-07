
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
        [JsonProperty]
        public List<Guid> Parents { get; set; }
        [JsonProperty]
        public List<Guid> Children { get; set; }
        [JsonProperty]
        public T Value { get; private set; }
        [JsonIgnore]
        public bool IsSuffixRegistered { get; set; }
        [JsonProperty]
        public Guid ID { get; private set; }

        public MinimalDAGNode(T value, Guid id)
        {
            Value = value;
            Parents = new List<Guid>();
            Children = new List<Guid>();
            ID = id;
        }

        //public Guid GetID() => _ID;


        [JsonConstructor]
        public MinimalDAGNode(IEnumerable<Guid> Parents, IEnumerable<Guid> Children, T Value, Guid ID)
        {
            this.Parents = new List<Guid>(Parents);
            this.Children = new List<Guid>(Children);
            this.Value = Value;
            this.ID = ID;
        }


        // public IList<Guid> GetParentIDs() => _parents;

        //public IList<Guid> GetChildIDs() => _children;

        //public void AddChild(Guid childID) => _children.Add(childID);

        //public void AddParent(Guid parentID) => _parents.Add(parentID);

        //public T GetValue() => _value;

        //public bool IsRegistered() => _isSuffixRegistered;

        //public void RegisterSuffix() => _isSuffixRegistered = true;

        //public void RemoveChildID(Guid childID) => _children.Remove(childID);


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
                    return this.Children.SequenceEqual(otherNode.Children);
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
                    con += child.GetHashCode();

                return con.GetHashCode();
            }
        }

    }
}


