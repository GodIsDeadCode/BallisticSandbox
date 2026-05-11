using System;
using System.Collections.Generic;

namespace BallisticSandbox.Utility.Extensions
{
    public static class CollectionExtensions
    {
        public static int InsertIntoSortedList<T>(this IList<T> list, T item) where T : IComparable<T>, IComparable
        {
            int startIndex = 0;
            int endIndex = list.Count;

            while (startIndex < endIndex)
            {
                int middleIndex = startIndex + (endIndex - startIndex) / 2;
                T middleItem = list[middleIndex];

                if (item.CompareTo(middleItem) <= 0)
                    endIndex = middleIndex;
                else
                    startIndex = middleIndex + 1;
            }

            list.Insert(startIndex, item);
            return endIndex;
        }
    }
}
