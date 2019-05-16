namespace BNolan.RandomSelection.Library
{
    public class Item<T>
    {
        /// <summary>
        /// Unique identifier for this item
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// Object to associate with the unique id 
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Number of entries this item should get. The default is one but if the item
        /// should have an entry in the pool more than one time then set the number of total entries
        /// for this item through this property
        /// </summary>
        public int Entries { get; set; }

        /// <summary>
        /// Default constructor Initializes
        /// <code>Weight</code> to 1
        /// </summary>
        public Item()
        {
            Entries = 1;
        }

        /// <summary>
        /// Constructor with <code>UniqueId</code> and
        /// <code>Name</code> set from input with
        /// <code>Weight</code>defaulted to 1
        /// </summary>
        /// <param name="uniqueId">Unique Id for the item</param>
        /// <param name="value">Object associated with the unique id</param>
        public Item(string uniqueId,
            T value)
            : this()
        {
            UniqueId = uniqueId;
            Value = value;
        }

        /// <summary>
        /// Constructor with <code>UniqueId</code>, <code>Name</code>,
        /// <code>Weight</code> set from input
        /// </summary>
        /// <param name="uniqueId">Unique Id for the item</param>
        /// <param name="value">Object associated with the unique id</param>
        /// <param name="entries">Weight associated with the item</param>
        public Item(string uniqueId,
            T value,
            int entries)
            : this(uniqueId, value)
        {
            Entries = entries;
        }

        /// <summary>
        /// Gets the string representation of the Value object associated with the Item
        /// </summary>
        /// <returns>String representation of the Item.Value</returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}