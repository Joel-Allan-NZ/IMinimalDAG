
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
        List<Guid> _parents;
        [JsonProperty]
        List<Guid> _children;
        [JsonProperty]
        T _value;

        bool _isSuffixRegistered;
        [JsonProperty]
        Guid _ID;

        public MinimalDAGNode(T value, Guid id)
        {
            _value = value;
            _parents = new List<Guid>();
            _children = new List<Guid>();
            _ID = id;
        }

        public Guid GetID() => _ID;


        [JsonConstructor]
        public MinimalDAGNode(IEnumerable<Guid> _parents, IEnumerable<Guid> _children, T _value, Guid _ID)
        {
            this._parents = new List<Guid>(_parents);
            this._children = new List<Guid>(_children);
            this._value = _value;
            this._ID = _ID;
        }


        public IList<Guid> GetParentIDs() => _parents;

        public IList<Guid> GetChildIDs() => _children;

        public void AddChild(Guid childID) => _children.Add(childID);

        public void AddParent(Guid parentID) => _parents.Add(parentID);

        public T GetValue() => _value;

        public bool IsRegistered() => _isSuffixRegistered;

        public void RegisterSuffix() => _isSuffixRegistered = true;

        public void RemoveChildID(Guid childID) => _children.Remove(childID);


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
                if (this._value.Equals(otherNode._value) && this._children.Count == otherNode._children.Count)
                {
                    return this._children.SequenceEqual(otherNode._children);
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
                con += _value.GetHashCode();
                foreach (var child in _children)
                    con += child.GetHashCode();

                return con.GetHashCode();
            }
        }

    }
}


