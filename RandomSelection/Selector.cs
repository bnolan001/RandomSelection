using BNolan.RandomSelection.Library;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BNolan.RandomSelection
{
    public class Selector<T>
    {
        private Dictionary<string, Item<T>> _dicItems;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Selector()
        {
            _dicItems = new Dictionary<string, Item<T>>();
        }

        /// <summary>
        /// Adds the <code>item</code> to storage
        /// </summary>
        /// <param name="item"></param>
        /// <returns>False if the UniqueId assocaited with the item already exists</returns>
        /// <exception cref="ArgumentNullException">If the <code>item</code> or
        /// <code>item.UniqueId</code> are null or empty</exception>
        /// <exception cref="ArgumentException">If <code>item.Entries</code> is less than 1</exception>
        public bool TryAddItem(Item<T> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Object cannot be null.");
            }

            if (string.IsNullOrEmpty(item.UniqueId))
            {
                throw new ArgumentNullException(nameof(item.UniqueId), "Value cannot be null.");
            }

            if (item.Entries < 1)
            {
                throw new ArgumentException("Must have a value greater than 0.", nameof(item.Entries));
            }

            return TryAddItem(item.UniqueId, item.Value, item.Entries);
        }

        /// <summary>
        /// Adds <code>uniqueId</code> to storage
        /// </summary>
        /// <param name="uniqueId">Unique identifier that no other items will share</param>
        /// <returns>False if the UniqueId assocaited with the item already exists</returns>
        /// <exception cref="ArgumentNullException">If the <code>UniqueId</code> is null or empty</exception>
        public bool TryAddItem(T uniqueId)
        {
            if (uniqueId == null)
            {
                throw new ArgumentNullException(nameof(uniqueId), "Value cannot be null.");
            }
            var item = new Item<T>()
            {
                UniqueId = uniqueId.ToString(),
                Value = uniqueId,
                Entries = 1
            };
            return TryAddItem(item);
        }

        /// <summary>
        /// Adds <code>uniqueId</code> and <code>name</code> to storage
        /// </summary>
        /// <param name="uniqueId">Unique identifier that no other items will share</param>
        /// <param name="value">Descriptive name associated with the item</param>
        /// <returns>False if the UniqueId assocaited with the item already exists</returns>
        /// <exception cref="ArgumentNullException">If the <code>UniqueId</code> is null or empty</exception>
        public bool TryAddItem(string uniqueId,
            T value)
        {
            return TryAddItem(uniqueId, value, 1); ;
        }

        /// <summary>
        /// Adds <code>uniqueId</code> and <code>name</code> to storage
        /// </summary>
        /// <param name="uniqueId">Unique identifier that no other items will share</param>
        /// <param name="value">Descriptive name associated with the item</param>
        /// <param name="entries">Number of entries given to this item</param>
        /// <returns>False if the UniqueId assocaited with the item already exists</returns>
        /// <exception cref="ArgumentNullException">If the <code>UniqueId</code> is null or empty</exception>
        /// <exception cref="ArgumentException">If <code>entries</code> is less than 1</exception>
        public bool TryAddItem(string uniqueId,
            T value,
            int entries)
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                throw new ArgumentNullException(nameof(uniqueId), "Value cannot be null.");
            }

            if (entries < 1)
            {
                throw new ArgumentException("Must have a value greater than 0.", nameof(entries));
            }
            var upperId = uniqueId.ToUpper();
            if (_dicItems.ContainsKey(upperId))
            {
                return false;
            }

            _dicItems.Add(upperId, new Item<T>(uniqueId, value, entries));

            return true;
        }

        /// <summary>
        /// Creates a list filled with the <code>uniqueId</code> from
        /// each item based on the entries
        /// </summary>
        /// <returns>List of all unique ids with an instance of each
        /// id for every entry it is designated to have</returns>
        public List<string> GenerateList()
        {
            var uniqueIdList = new List<string>();

            foreach (var item in _dicItems)
            {
                // Add every instance of the object based on the number of entries
                // it is set to have
                for (int entries = 0; entries < item.Value.Entries; entries++)
                {
                    uniqueIdList.Add(item.Value.UniqueId);
                }
            }

            uniqueIdList.TrimExcess();
            return uniqueIdList;
        }

        /// <summary>
        /// Takes the list and randomizes the order of the elements
        /// </summary>
        /// <param name="items">Randomized list of items</param>
        /// <returns>An unordered, randomly sorted list of items</returns>
        public List<string> RandomizeList(List<string> items)
        {
            var listCount = items.Count;
            var randomizedList = new List<string>();
            randomizedList.AddRange(items);
            // Loop over the list once and swap the indexes at random
            for (var idx = 0; idx < listCount; idx++)
            {
                int rndmIdx = GenerateRandomIndex(listCount);
                var oldValue = randomizedList[idx];
                randomizedList[idx] = randomizedList[rndmIdx];
                randomizedList[rndmIdx] = oldValue;
            }
            return randomizedList;
        }

        /// <summary>
        /// Generates a random number that falls between 0 and the <code>upperLimit</code>
        /// </summary>
        /// <param name="upperLimit"></param>
        /// <returns>Random number from 0 to the <code>upperLimit</code></returns>
        public int GenerateRandomIndex(int upperLimit)
        {
            if (upperLimit <= 0)
            {
                throw new ArgumentOutOfRangeException("upperLimit");
            }

            using (var randomGenerator = new RNGCryptoServiceProvider())
            {
                byte[] uint32Buffer = new byte[sizeof(UInt32)];
                while (true)
                {
                    randomGenerator.GetBytes(uint32Buffer);
                    UInt32 rand = BitConverter.ToUInt32(uint32Buffer, 0);

                    Int64 max = (1 + (Int64)UInt32.MaxValue);
                    Int64 remainder = max % upperLimit;
                    if (rand < max - remainder)
                    {
                        return (Int32)(rand % upperLimit);
                    }
                }
            }
        }

        /// <summary>
        /// Takes the items added to storage and randomly selects the
        /// <code>numToSelect</code> items from the available list
        /// </summary>
        /// <param name="numToSelect">Number of items to select</param>
        /// <returns>List of selected Items</returns>
        public List<Item<T>> RandomSelect(int numToSelect)
        {
            var selected = new List<Item<T>>();
            var fullList = GenerateList();
            var randomizedList = RandomizeList(fullList);

            for (var selectedCt = 0; selectedCt < numToSelect; selectedCt++)
            {
                // Get a random index to select
                var selectedIdx = GenerateRandomIndex(randomizedList.Count);

                // Lookup the item based on the uniqueId key
                var uniqueKey = randomizedList[selectedIdx].ToUpper();
                selected.Add(_dicItems[uniqueKey]);

                // Remove this item so we don't grab it more than once
                randomizedList.RemoveAt(selectedIdx);
            }

            return selected;
        }
    }
}