﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NSCI
{
    public partial class OrderedList<T> : Collection<T>, IList<T>
    {
        private readonly IComparer<T> comparer;

        public OrderedList(IComparer<T> comparer)
        {
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        protected override sealed void InsertItem(int index, T item)
        {
            int insertIndex = GetInsertIndex(item);
            base.InsertItem(insertIndex, item);
        }

        protected override sealed void SetItem(int index, T item)
        {
            RemoveItem(index);
            int insertIndex = GetInsertIndex(item);
            base.InsertItem(insertIndex, item);
        }

        public new bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index == -1)
                return false;
            RemoveAt(index);
            return true;
        }

        public new int IndexOf(T item)
        {
            var foundIndex = GetInsertIndex(item);
            if (foundIndex < 0 || Count == 0 || foundIndex >= Count) // No element found
                return -1;

            //we found one Item that has the same Size, but this must not be the equals to the searched object.
            if (item.Equals(this[foundIndex]))
                return foundIndex;

            var i = foundIndex - 1;
            //search backwards
            while (i >= 0 && !item.Equals(this[i]) && this.comparer.Compare(item, this[i]) == 0)
                --i;

            if (i >= 0 && item.Equals(this[i]))
                return i;

            i = foundIndex + 1;
            //search forward
            while (i < Count && !item.Equals(this[i]) && this.comparer.Compare(item, this[i]) == 0)
                ++i;

            if (i < Count && item.Equals(this[i]))
                return i;

            return -1;
        }


        private int GetInsertIndex(T item)
        {
            if (Count == 0)
                return 0;
            var index = this.BinarySearch(item, this.comparer);
            if (index < 0)
                index = ~index;
            return index;
        }




    }

}