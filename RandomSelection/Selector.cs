using BNolan.RandomSelection.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace BNolan.RandomSelection
{
    public class Selector<T> : IEnumerable<Item<T>>, IDisposable
    {
        private Dictionary<string, Item<T>> _dicItems;
        private RandomNumberGenerator _randomGenerator;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Selector()
        {
            _dicItems = new Dictionary<string, Item<T>>();
            _randomGenerator = RandomNumberGenerator.Create();
        }

        /// <summary>
        /// Gets the number of unique items currently stored in the selector.
        /// </summary>
        public int Count => _dicItems.Count;

        /// <summary>
        /// Gets the total number of entries across all items (sum of all item entry counts).
        /// </summary>
        public int TotalEntries => _dicItems.Values.Sum(x => x.Entries);

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
                throw new ArgumentNullException(nameof(item.UniqueId), "Value cannot be null or empty.");
            }

            if (item.Entries < 1)
            {
                throw new ArgumentException("Must have a value greater than 0.", nameof(item.Entries));
            }

            return TryAddItem(item.UniqueId, item.Value, item.Entries);
        }

        /// <summary>
        /// Adds the <code>item</code> to storage and returns this selector for method chaining.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>This selector instance for method chaining</returns>
        /// <exception cref="InvalidOperationException">If the UniqueId associated with the item already exists</exception>
        public Selector<T> AddItem(Item<T> item)
        {
            if (!TryAddItem(item))
            {
                throw new InvalidOperationException($"An item with UniqueId '{item.UniqueId}' already exists.");
            }
            return this;
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
        /// Adds <code>uniqueId</code> to storage and returns this selector for method chaining.
        /// </summary>
        /// <param name="uniqueId">Unique identifier that no other items will share</param>
        /// <returns>This selector instance for method chaining</returns>
        /// <exception cref="InvalidOperationException">If the UniqueId already exists</exception>
        public Selector<T> AddItem(T uniqueId)
        {
            if (!TryAddItem(uniqueId))
            {
                throw new InvalidOperationException($"An item with UniqueId '{uniqueId}' already exists.");
            }
            return this;
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
        /// Adds <code>uniqueId</code> and <code>value</code> to storage and returns this selector for method chaining.
        /// </summary>
        /// <param name="uniqueId">Unique identifier that no other items will share</param>
        /// <param name="value">Descriptive value associated with the item</param>
        /// <returns>This selector instance for method chaining</returns>
        /// <exception cref="InvalidOperationException">If the UniqueId already exists</exception>
        public Selector<T> AddItem(string uniqueId, T value)
        {
            if (!TryAddItem(uniqueId, value))
            {
                throw new InvalidOperationException($"An item with UniqueId '{uniqueId}' already exists.");
            }
            return this;
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
        /// Adds <code>uniqueId</code>, <code>value</code>, and <code>entries</code> to storage and returns this selector for method chaining.
        /// </summary>
        /// <param name="uniqueId">Unique identifier that no other items will share</param>
        /// <param name="value">Descriptive value associated with the item</param>
        /// <param name="entries">Number of entries given to this item</param>
        /// <returns>This selector instance for method chaining</returns>
        /// <exception cref="InvalidOperationException">If the UniqueId already exists</exception>
        public Selector<T> AddItem(string uniqueId, T value, int entries)
        {
            if (!TryAddItem(uniqueId, value, entries))
            {
                throw new InvalidOperationException($"An item with UniqueId '{uniqueId}' already exists.");
            }
            return this;
        }

        /// <summary>
        /// Attempts to add a collection of items to the container if none of their keys already exist.
        /// </summary>
        /// <remarks>If any item's key already exists in the container, or if adding any item fails, no
        /// items are added and the method returns false. Keys are compared in a case-insensitive manner based on the
        /// uppercase string representation of each item.</remarks>
        /// <param name="items">The list of items to add. Cannot be null. Each item's key is determined by its string representation,
        /// converted to uppercase.</param>
        /// <returns>true if all items were added successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the items parameter is null.</exception>
        public bool TryAddItems(List<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items), "Value cannot be null.");
            }
            var newKeys = new HashSet<string>(items.Select(i => i.ToString().ToUpper()));
            if (_dicItems.Any(i => newKeys.Contains(i.Key)))
            {
                return false;
            }

            foreach(var item in items)
            {
                if (!TryAddItem(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds a collection of items to the container and returns this selector for method chaining.
        /// </summary>
        /// <param name="items">The list of items to add</param>
        /// <returns>This selector instance for method chaining</returns>
        /// <exception cref="InvalidOperationException">If any item's UniqueId already exists</exception>
        public Selector<T> AddItems(List<T> items)
        {
            if (!TryAddItems(items))
            {
                throw new InvalidOperationException("One or more items already exist in the selector.");
            }
            return this;
        }

        /// <summary>
        /// Checks if an item with the specified UniqueId exists in the selector.
        /// </summary>
        /// <param name="uniqueId">The UniqueId to check (case-insensitive)</param>
        /// <returns>true if the item exists; otherwise, false</returns>
        public bool Contains(string uniqueId)
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                return false;
            }
            return _dicItems.ContainsKey(uniqueId.ToUpper());
        }

        /// <summary>
        /// Attempts to get an item by its UniqueId.
        /// </summary>
        /// <param name="uniqueId">The UniqueId of the item to retrieve (case-insensitive)</param>
        /// <param name="item">The item if found; otherwise, null</param>
        /// <returns>true if the item was found; otherwise, false</returns>
        public bool TryGetItem(string uniqueId, out Item<T> item)
        {
            item = null;
            if (string.IsNullOrEmpty(uniqueId))
            {
                return false;
            }
            return _dicItems.TryGetValue(uniqueId.ToUpper(), out item);
        }

        /// <summary>
        /// Removes an item from the selector by its UniqueId.
        /// </summary>
        /// <param name="uniqueId">The UniqueId of the item to remove (case-insensitive)</param>
        /// <returns>true if the item was removed; otherwise, false</returns>
        public bool RemoveItem(string uniqueId)
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                return false;
            }
            return _dicItems.Remove(uniqueId.ToUpper());
        }

        /// <summary>
        /// Clears all items from the selector.
        /// </summary>
        public void Clear()
        {
            _dicItems.Clear();
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
        /// Generates a random number that is used as an index in the data array.
        /// </summary>
        /// <param name="upperLimit">Length of the array which will serve as the upper limit of the number generator</param>
        /// <returns>Random number from 0 but less than <code>upperLimit</code></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <code>upperLimit</code> value is less than 1</exception>
        public int GenerateRandomIndex(int upperLimit)
        {
            if (upperLimit <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(upperLimit)}",
                    $"The value of {upperLimit} is invalid.  Please use a number greater than 0.");
            }

            int randomIndex = 0;
            byte[] maxIndex = BitConverter.GetBytes(upperLimit);
            int arraySize = 3;
            for (; arraySize > 0; arraySize--)
            {
                if (maxIndex[arraySize] != 0)
                {
                    break;
                }
            }

            // Create the byte array used for calculating the 32bit int
            byte[] fullIntArray = new byte[4];
            // Create the byte array no larger than needed for the max
            // set by the upperLimit parameter
            int randomNumberArraySize = arraySize + 1;
            byte[] randomNumber = new byte[randomNumberArraySize];
            do
            {
                // Reset the array to 0
                Array.Clear(fullIntArray, 0, fullIntArray.Length);
                // Get the random value
                _randomGenerator.GetBytes(randomNumber);

                // Copy the value to an int32 sized byte array
                Array.Copy(randomNumber, fullIntArray, randomNumberArraySize);

                randomIndex = BitConverter.ToInt32(fullIntArray, 0);
                // Keep looping until we have a valid index
            } while (randomIndex < 0 || randomIndex >= upperLimit);

            return randomIndex;
        }

        /// <summary>
        /// Takes the items added to storage and randomly selects the
        /// <code>numToSelect</code> items from the available list
        /// </summary>
        /// <param name="numToSelect">Number of items to select, default is 1</param>
        /// <returns>List of selected Items</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when numToSelect is greater than total entries or less than 1</exception>
        public List<Item<T>> RandomSelect(int numToSelect = 1)
        {
            if (numToSelect < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numToSelect), "Must select at least 1 item.");
            }

            int totalEntries = TotalEntries;
            if (numToSelect > totalEntries)
            {
                throw new ArgumentOutOfRangeException(nameof(numToSelect), 
                    $"Cannot select {numToSelect} items when only {totalEntries} total entries are available.");
            }

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

        /// <summary>
        /// Peeks at a random selection without modifying state, useful for previewing results.
        /// </summary>
        /// <param name="numToSelect">Number of items to peek at, default is 1</param>
        /// <returns>List of randomly selected Items (non-destructive)</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when numToSelect is invalid</exception>
        public List<Item<T>> Peek(int numToSelect = 1)
        {
            if (numToSelect < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numToSelect), "Must select at least 1 item.");
            }

            int totalEntries = TotalEntries;
            if (numToSelect > totalEntries)
            {
                throw new ArgumentOutOfRangeException(nameof(numToSelect),
                    $"Cannot select {numToSelect} items when only {totalEntries} total entries are available.");
            }

            // This method is identical to RandomSelect since RandomSelect doesn't modify the internal dictionary
            return RandomSelect(numToSelect);
        }

        /// <summary>
        /// Returns an enumerator for all items in the selector.
        /// </summary>
        public IEnumerator<Item<T>> GetEnumerator()
        {
            return _dicItems.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator for all items in the selector.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Disposes the random number generator.
        /// </summary>
        public void Dispose()
        {
            _randomGenerator?.Dispose();
        }
    }
}