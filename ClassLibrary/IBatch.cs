using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    internal interface IBatch<T>
    {
        /// <summary>
        /// Adds an item to internal storage
        /// </summary>
        /// <param name="item"></param>
        void AddItem(T item);

        /// <summary>
        /// Return a Batch of 100 items if storage holds at least 100 items.
        /// If oldest item in storage is older than 5 minutes, return a batch of all items in storage.
        /// If number of items in storage is less than 100 and oldest item is younger than 5 minutes, return null.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetBatch();
    }
}