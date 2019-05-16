using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Collections.Generic;
using BNolan.RandomSelection.Library;
using System.Linq;
using BNolan.RandomSelection;

namespace Testing.Unit
{
    [TestClass]
    public class SelectorTests
    {
        /// <summary>
        /// Verify that the <code>GenerateRandomIndex</code> creates numbers
        /// that fall within the range of 0 to the max value defined by
        /// the <code>upperLimit</code> parameter
        /// </summary>
        [TestMethod]
        public void GenerateRandomIndex()
        {
            var selector = new Selector<string>();
            int startNumber = 1000000000;
            while (startNumber > 0)
            {
                var index = selector.GenerateRandomIndex(startNumber);
                index.Should().BeGreaterOrEqualTo(0);
                index.Should().BeLessOrEqualTo(startNumber);
                startNumber = startNumber / 10;
            }
        }

        /// <summary>
        /// Checks different conditions for adding entries to the list
        /// and verifies expected result is returned
        /// </summary>
        [TestMethod]
        public void TryAddItem_ArgumentNullException_NullObject()
        {
            // Invalid entry testing
            var selector = new Selector<string>();
            var item = new Item<string>()
            {
                UniqueId = "a",
                Value = "alpha",
                Entries = 1
            };
            selector.TryAddItem(item).Should().BeTrue();
            selector.TryAddItem("b").Should().BeTrue();
            selector.TryAddItem("c", "charlie").Should().BeTrue();
            selector.TryAddItem("d", "delta", 2).Should().BeTrue();

            // Invalid entry testing
            selector.TryAddItem("b").Should().BeFalse();
            selector.TryAddItem(item).Should().BeFalse();
            selector.TryAddItem("c", "charlie").Should().BeFalse();
            selector.TryAddItem("d", "delta", 2).Should().BeFalse();

            // Exception Testing
            Action action = () => selector.TryAddItem((Item<string>)null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: item must have a value");
        }


        [TestMethod]
        public void TryAddItem_ArgumentNullException_NullValue()
        {
            var selector = new Selector<string>();
            Action action = () => selector.TryAddItem(new Item<string>()
            {
                UniqueId = null,
                Value = null,
                Entries = 0
            });
            action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: UniqueId must have a value");

        }


        [TestMethod]
        public void TryAddItem_ArgumentException_ZeroEntryValue()
        {
            var selector = new Selector<string>();
            Action action = () => selector.TryAddItem(new Item<string>()
            {
                UniqueId = "a",
                Value = null,
                Entries = 0
            });
            action.ShouldThrow<ArgumentException>().WithMessage("Entries must have a value greater than 0");
        }

        [TestMethod]
        public void TryAddItem_ArgumentNullException_EmptyItem()
        {
            var selector = new Selector<string>();
            Action action = () => selector.TryAddItem(new Item<string>());
            action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: uniqueId must have a value");
        }

        [TestMethod]
        public void TryAddItem_ArgumentNullException_NullIdValueZeroEntries()
        {
            var selector = new Selector<string>();
            Action action = () => selector.TryAddItem(new Item<string>()
            {
                UniqueId = null,
                Value = null,
                Entries = 0
            });
            action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: UniqueId must have a value");
        }

        [TestMethod]
        public void TryAddItem_ArgumentNullException_NullValueZeroEntries()
        {
            var selector = new Selector<string>();
            Action action = () => selector.TryAddItem("a", null, 0);

            action.ShouldThrow<ArgumentException>().WithMessage("Entries must have a value greater than 0");
        }

        /// <summary>
        /// Verifies that the values added are accepted and not randomized yet
        /// </summary>
        [TestMethod]
        public void GenerateList()
        {
            var selector = new Selector<string>();
            var item = new Item<string>()
            {
                UniqueId = "a",
                Value = "alpha",
                Entries = 1
            };
            selector.TryAddItem(item).Should().BeTrue();
            selector.TryAddItem("b").Should().BeTrue();
            selector.TryAddItem("c", "charlie").Should().BeTrue();
            selector.TryAddItem("d", "delta", 2).Should().BeTrue();
            var generatedList = selector.GenerateList();

            generatedList[0].Should().Be("a");
            generatedList[1].Should().Be("b");
            generatedList[2].Should().Be("c");
            generatedList[3].Should().Be("d");
            generatedList[4].Should().Be("d");
        }

        /// <summary>
        /// Checks the <code>RandomizeList</code> function to verify that
        /// values do not remain in the same order as to their insertion
        /// order.
        /// </summary>
        [TestMethod]
        public void RandomizeList()
        {
            var orderedList = new List<string>()
            {
                "a",
                "b",
                "c",
                "d",
                "e",
                "f",
                "g"
            };

            var selector = new Selector<string>();
            var randomizedList = selector.RandomizeList(orderedList);
            // Check each index and count the number of matches
            int orderedCt = 0;
            for (var idx = 0; idx < orderedList.Count; idx++)
            {
                if (orderedList[idx].Equals(randomizedList[idx]))
                {
                    orderedCt++;
                }
            }
            // Make sure that at least one of the elements are not in the same
            // position.  Nearly all should not be.
            orderedCt.Should().BeLessThan(orderedList.Count);
        }

        /// <summary>
        /// Verifies that the logic creates a list with the appropriate
        /// number of entries based on the <code>Item</code> definition.
        /// Also checks to see that the <code>RandomSelect</code> returns
        /// the correct number of entries for each item after randomization.
        /// </summary>
        [TestMethod]
        public void Select()
        {
            var selector = new Selector<string>();
            selector.TryAddItem(new Item<string>()
            {
                UniqueId = "a",
                Value = "alpha",
                Entries = 2
            });
            selector.TryAddItem(new Item<string>()
            {
                UniqueId = "b",
                Value = "bravo",
                Entries = 1
            });
            selector.TryAddItem(new Item<string>()
            {
                UniqueId = "c",
                Value = "charlie",
                Entries = 5
            });
            selector.TryAddItem(new Item<string>()
            {
                UniqueId = "d",
                Value = "delta",
                Entries = 1
            });
            var items = selector.GenerateList();
            var selected = selector.RandomSelect(items.Count);

            (from s in selected
             where s.UniqueId == "a"
             select s).Count().Should().Be(2);

            (from s in selected
             where s.UniqueId == "b"
             select s).Count().Should().Be(1);

            (from s in selected
             where s.UniqueId == "c"
             select s).Count().Should().Be(5);

            (from s in selected
             where s.UniqueId == "d"
             select s).Count().Should().Be(1);
        }
    }
}