// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// A base class for container blocks.
    /// </summary>
    /// <seealso cref="Block" />
    [DebuggerDisplay("{GetType().Name} Count = {Count}")]
    public abstract class ContainerBlock : Block, IList<Block>, IReadOnlyList<Block>
    {
        private Block[] _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        protected ContainerBlock(BlockParser parser) : base(parser)
        {
            _children = Array.Empty<Block>();
        }

        /// <summary>
        /// Gets the last child.
        /// </summary>
        public Block? LastChild
        {
            get
            {
                int index = Count - 1;
                Block[] children = _children;
                if ((uint)index < (uint)children.Length)
                {
                    return children[index];
                }
                return null;
            }
        }

        /// <summary>
        /// Specialize enumerator.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<Block> IEnumerable<Block>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Block item)
        {
            if (item == null)
                ThrowHelper.ArgumentNullException_item();

            if (item.Parent != null)
            {
                static void Throw() => throw new ArgumentException("Cannot add this block as it as already attached to another container (block.Parent != null)");
                Throw();
            }

            int count = Count;
            if (count == _children.Length)
            {
                EnsureCapacity(count + 1);
            }
            _children[Count++] = item;
            item.Parent = this;

            UpdateSpanEnd(item.Span.End);
        }

        private void EnsureCapacity(int min)
        {
            if (_children.Length < min)
            {
                int num = (_children.Length == 0) ? 4 : (_children.Length * 2);
                if (num < min)
                {
                    num = min;
                }

                var destinationArray = new Block[num];
                if (Count > 0)
                {
                    Array.Copy(_children, 0, destinationArray, 0, Count);
                }
                _children = destinationArray;
            }
        }

        public void Clear()
        {
            Block[] children = _children;
            for (int i = Count - 1; i >= 0 && (uint)i < (uint)children.Length; i--)
            {
                children[i].Parent = null;
                children[i] = null!;
            }

            Count = 0;
        }

        public bool Contains(Block item)
        {
            if (item == null)
                ThrowHelper.ArgumentNullException_item();

            Block[] children = _children;
            for (int i = Count - 1; (uint)i < (uint)children.Length && i >= 0; i--)
            {
                if (children[i] == item)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(Block[] array, int arrayIndex)
        {
            Array.Copy(_children, 0, array, arrayIndex, Count);
        }

        public bool Remove(Block item)
        {
            if (item == null)
                ThrowHelper.ArgumentNullException_item();

            Block[] children = _children;
            for (int i = Count - 1; (uint)i < (uint)children.Length && i >= 0; i--)
            {
                if (children[i] == item)
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public int IndexOf(Block item)
        {
            if (item == null)
                ThrowHelper.ArgumentNullException_item();

            Block[] children = _children;
            for (int i = 0; i < children.Length && i < Count; i++)
            {
                if (children[i] == item)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, Block item)
        {
            if (item == null)
                ThrowHelper.ArgumentNullException_item();

            if (item.Parent != null)
            {
                static void Throw() => throw new ArgumentException("Cannot add this block as it as already attached to another container (block.Parent != null)");
                Throw();
            }
            if ((uint)index > (uint)Count)
            {
                ThrowHelper.ArgumentOutOfRangeException_index();
            }
            if (Count == _children.Length)
            {
                EnsureCapacity(Count + 1);
            }
            if (index < Count)
            {
                Array.Copy(_children, index, _children, index + 1, Count - index);
            }
            _children[index] = item;
            Count++;
            item.Parent = this;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index > (uint)Count)
                ThrowHelper.ArgumentOutOfRangeException_index();

            Count--;
            // previous children
            var item = _children[index];
            item.Parent = null;
            if (index < Count)
            {
                Array.Copy(_children, index + 1, _children, index, Count - index);
            }
            _children[Count] = null!;
        }

        public Block this[int index]
        {
            get
            {
                var array = _children;
                if ((uint)index >= (uint)array.Length || index >= Count)
                {
                    ThrowHelper.ThrowIndexOutOfRangeException();
                    return null;
                }
                return array[index];
            }
            set
            {
                if ((uint)index >= (uint)Count) ThrowHelper.ThrowIndexOutOfRangeException();
                _children[index] = value;
            }
        }

        public void Sort(IComparer<Block> comparer)
        {
            if (comparer == null) ThrowHelper.ArgumentNullException(nameof(comparer));
            Array.Sort(_children, 0, Count, comparer);
        }

        public void Sort(Comparison<Block> comparison)
        {
            if (comparison == null) ThrowHelper.ArgumentNullException(nameof(comparison));
            Array.Sort(_children, 0, Count, new BlockComparer(comparison));
        }

        #region Nested type: Enumerator

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<Block>
        {
            private readonly ContainerBlock block;
            private int index;
            private Block current;

            internal Enumerator(ContainerBlock block)
            {
                this.block = block;
                index = 0;
                current = null!;
            }

            public Block Current => current;

            object IEnumerator.Current => Current;


            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (index < block.Count)
                {
                    current = block[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private bool MoveNextRare()
            {
                index = block.Count + 1;
                current = null!;
                return false;
            }

            void IEnumerator.Reset()
            {
                index = 0;
                current = null!;
            }
        }

        #endregion

        private sealed class BlockComparer : IComparer<Block>
        {
            private readonly Comparison<Block> comparison;

            public BlockComparer(Comparison<Block> comparison)
            {
                this.comparison = comparison;
            }

            public int Compare(Block x, Block y)
            {
                return comparison(x, y);
            }
        }
    }
}