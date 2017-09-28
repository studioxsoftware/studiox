using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace StudioX.Collections
{
    /// <summary>
    /// A shortcut for <see cref="TypeList{TBaseType}"/> to use object as base type.
    /// </summary>
    public class TypeList : TypeList<object>, ITypeList
    {
    }

    /// <summary>
    /// Extends <see cref="List{Type}"/> to add restriction a specific base type.
    /// </summary>
    /// <typeparam name="TBaseType">Base Type of <see cref="Type"/>s in this list</typeparam>
    public class TypeList<TBaseType> : ITypeList<TBaseType>
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => typeList.Count;

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the <see cref="Type"/> at the specified index.
        /// </summary>
        /// <param name="index">Index.</param>
        public Type this[int index]
        {
            get => typeList[index];
            set
            {
                CheckType(value);
                typeList[index] = value;
            }
        }

        private readonly List<Type> typeList;

        /// <summary>
        /// Creates a new <see cref="TypeList{T}"/> object.
        /// </summary>
        public TypeList()
        {
            typeList = new List<Type>();
        }

        /// <inheritdoc/>
        public void Add<T>() where T : TBaseType
        {
            typeList.Add(typeof(T));
        }

        /// <inheritdoc/>
        public void Add(Type item)
        {
            CheckType(item);
            typeList.Add(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, Type item)
        {
            typeList.Insert(index, item);
        }

        /// <inheritdoc/>
        public int IndexOf(Type item)
        {
            return typeList.IndexOf(item);
        }

        /// <inheritdoc/>
        public bool Contains<T>() where T : TBaseType
        {
            return Contains(typeof(T));
        }

        /// <inheritdoc/>
        public bool Contains(Type item)
        {
            return typeList.Contains(item);
        }

        /// <inheritdoc/>
        public void Remove<T>() where T : TBaseType
        {
            typeList.Remove(typeof(T));
        }

        /// <inheritdoc/>
        public bool Remove(Type item)
        {
            return typeList.Remove(item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            typeList.RemoveAt(index);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            typeList.Clear();
        }

        /// <inheritdoc/>
        public void CopyTo(Type[] array, int arrayIndex)
        {
            typeList.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<Type> GetEnumerator()
        {
            return typeList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return typeList.GetEnumerator();
        }

        private static void CheckType(Type item)
        {
            if (!typeof(TBaseType).GetTypeInfo().IsAssignableFrom(item))
            {
                throw new ArgumentException("Given item is not type of " + typeof(TBaseType).AssemblyQualifiedName, "item");
            }
        }
    }
}