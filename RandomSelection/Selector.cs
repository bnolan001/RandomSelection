using RandomSelection.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RandomSelection
{
    public class Selector
    {
        private Dictionary<string, Item> _dicItems;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Selector()
        {
            _dicItems = new Dictionary<string, Item>();
        }

        /// <summary>
        /// Adds the <code>item</code> to storage
        /// </summary>
        /// <param name="item"></param>
        /// <returns>False if the UniqueId assocaited with the item already exists</returns>
        /// <exception cref="ArgumentNullException">If the <code>item</code> or
        /// <code>item.UniqueId</code> are null or empty</exception>
        /// <exception cref="ArgumentException">If <code>item.Weight</code> is less than 1</exception>
        public bool TryAddItem(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(string.Format("{0} must have a value"), nameof(item));
            }

            if (string.IsNullOrEmpty(item.UniqueId))
            {
                throw new ArgumentNullException(string.Format("{0} must have a value"), nameof(item.UniqueId));
            }

            if (item.Weight < 1)
            {
                throw new ArgumentException(string.Format("{0} must have a value greater than 0", nameof(item.Weight)));
            }

            return TryAddItem(item.UniqueId, item.Name, item.Weight);
        }

        /// <summary>
        /// Adds <code>uniqueId</code> to storage
        /// </summary>
        /// <param name="uniqueId">Unique identifier that no other items will share</param>
        /// <returns>False if the UniqueId assocaited with the item already exists</returns>
        public bool TryAddItem(string uniqueId)
        {
            return TryAddItem(uniqueId, uniqueId, 1);
        }

        /// <summary>
        /// Adds <code>uniqueId</code> and <code>name</code> to storage
        /// </summary>
        /// <param name="uniqueId">Unique identifier that no other items will share</param>
        /// <param name="name">Descriptive name associated with the item</param>
        /// <returns>False if the UniqueId assocaited with the item already exists</returns>
        public bool TryAddItem(string uniqueId,
            string name)
        {
            return TryAddItem(uniqueId, name, 1); ;
        }

        /// <summary>
        /// Adds <code>uniqueId</code> and <code>name</code> to storage
        /// </summary>
        /// <param name="uniqueId">Unique identifier that no other items will share</param>
        /// <param name="name">Descriptive name associated with the item</param>
        /// <param name="weight">Weight, or number of entries given to this item</param>
        /// <returns>False if the UniqueId assocaited with the item already exists</returns>
        /// <exception cref="ArgumentNullException">If the <code>UniqueId</code> is null or empty</exception>
        /// <exception cref="ArgumentException">If <code>item.Weight</code> is less than 1</exception>
        public bool TryAddItem(string uniqueId,
            string name,
            int weight)
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                throw new ArgumentNullException(string.Format("{0} must have a value"), nameof(uniqueId));
            }

            if (weight < 1)
            {
                throw new ArgumentException(string.Format("{0} must have a value greater than 0", nameof(weight)));
            }
            var upperId = uniqueId.ToUpper();
            if (_dicItems.ContainsKey(upperId))
            {
                return false;
            }

            _dicItems.Add(upperId, new Item(uniqueId, name, weight));

            return true;
        }

        /// <summary>
        /// Creates a list filled with the <code>uniqueId</code> from
        /// each item based on the weight
        /// </summary>
        /// <returns></returns>
        public List<string> GenerateList()
        {
            var uniqueIdList = new List<string>();

            foreach (var item in _dicItems)
            {
                for (int entries = 0; entries < item.Value.Weight; entries++)
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
        /// <param name="items"></param>
        /// <returns></returns>
        public List<string> RandomizeList(List<string> items)
        {
            var listCount = items.Count;
            var randomizedList = new List<string>();
            randomizedList.AddRange(items);
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
        /// <returns></returns>
        public int GenerateRandomIndex(int upperLimit)
        {
            int randomIndex = 0;
            using (var randomGenerator = new RNGCryptoServiceProvider())
            {
                byte[] maxIndex = BitConverter.GetBytes(upperLimit);
                int arraySize = 3;
                for (; arraySize > 0; arraySize--)
                {
                    if (maxIndex[arraySize] != 0)
                    {
                        break;
                    }
                }
                byte[] fullIntArray = new byte[4] { 0, 0, 0, 0 };
                byte[] randomNumber = new byte[arraySize + 1];
                do
                {
                    // Get the random value
                    randomGenerator.GetBytes(randomNumber);

                    int idx = 0;
                    foreach (var bit in randomNumber)
                    {
                        fullIntArray[idx++] = bit;
                    }
                    randomIndex = BitConverter.ToInt32(fullIntArray, 0);
                } while (randomIndex < 0 || randomIndex >= upperLimit);
            }

            return randomIndex;
        }

        /// <summary>
        /// Takes the items added to storage and randomly selects the
        /// <code>numToSelect</code>
        /// </summary>
        /// <param name="numToSelect">Number of items to select</param>
        /// <returns>Selected Items</returns>
        public List<Item> RandomSelect(int numToSelect)
        {
            var selected = new List<Item>();
            var fullList = GenerateList();
            var randomizedList = RandomizeList(fullList);

            for (var selectedCt = 0; selectedCt < numToSelect; selectedCt++)
            {
                var selectedIdx = GenerateRandomIndex(randomizedList.Count);
                var uniqueKey = randomizedList[selectedIdx].ToUpper();
                selected.Add(_dicItems[uniqueKey]);
                randomizedList.RemoveAt(selectedIdx);
            }

            return selected;
        }
    }
}