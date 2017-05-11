using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSelection.Library
{
    public class Item
    {
        /// <summary>
        /// Unique identifier for this item
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// Descriptive name associated with this item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Weight or number of changes this item should get. The default is one but if the item
        /// should have an entry in the pool more than one time then set the number of total entries
        /// for this item by this property
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Default constructor Initializes
        /// <code>Weight</code> to 1
        /// </summary>
        public Item()
        {
            Weight = 1;
        }

        /// <summary>
        /// Constructor with <code>UniqueId</code> and
        /// <code>Name</code> set from input with
        /// <code>Weight</code>defaulted to 1
        /// </summary>
        /// <param name="uniqueId">Unique Id for the item</param>
        /// <param name="name">Descriptive name for the item</param>
        public Item(string uniqueId,
            string name)
            : this()
        {
            UniqueId = uniqueId;
            Name = name;
        }

        /// <summary>
        /// Constructor with <code>UniqueId</code>, <code>Name</code>,
        /// <code>Weight</code> set from input
        /// </summary>
        /// <param name="uniqueId">Unique Id for the item</param>
        /// <param name="name">Descriptive name for the item</param>
        /// <param name="weight">Weight associated with the item</param>
        public Item(string uniqueId,
            string name,
            int weight)
            : this(uniqueId, name)
        {
            Weight = weight;
        }
    }
}