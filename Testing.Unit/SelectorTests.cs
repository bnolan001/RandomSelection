using BNolan.RandomSelection;
using BNolan.RandomSelection.Library;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Testing.Unit
{
    [TestFixture]
    public class SelectorTests
    {
        /// <summary>
        /// Verify that the <code>GenerateRandomIndex</code> creates numbers
        /// that fall within the range of 0 to the max value defined by
        /// the <code>upperLimit</code> parameter
        /// </summary>
        [Test]
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
        [Test]
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
            action.Should().Throw<ArgumentNullException>().WithMessage("Object cannot be null. (Parameter 'item')");
        }

        /// <summary>
        /// Verifies an exception is thrown when an Item with a null UniqueId
        /// </summary>
        [Test]
        public void TryAddItem_ArgumentNullException_NullValue()
        {
            var selector = new Selector<string>();
            Action action = () => selector.TryAddItem(new Item<string>()
            {
                UniqueId = null,
                Value = null,
                Entries = 0
            });
            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null or empty. (Parameter 'UniqueId')");

        }

        /// <summary>
        /// Verifies an exception is thrown when an item with an Entries value of 0 is used
        /// </summary>
        [Test]
        public void TryAddItem_ArgumentException_ZeroEntryValue()
        {
            var selector = new Selector<string>();
            Action action = () => selector.TryAddItem(new Item<string>()
            {
                UniqueId = "a",
                Value = null,
                Entries = 0
            });
            action.Should().Throw<ArgumentException>().WithMessage("Must have a value greater than 0. (Parameter 'Entries')");
        }

        /// <summary>
        /// Verifies an exception is thrown when an Item with a null UniqueId
        /// </summary>
        [Test]
        public void TryAddItem_ArgumentNullException_EmptyItem()
        {
            var selector = new Selector<string>();
            Action action = () => selector.TryAddItem(new Item<string>());
            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null or empty. (Parameter 'UniqueId')");
        }

        [Test]
        public void TryAddItem_ArgumentNullException_NullIdValueZeroEntries()
        {
            var selector = new Selector<string>();
            Action action = () => selector.TryAddItem(new Item<string>()
            {
                UniqueId = null,
                Value = null,
                Entries = 0
            });
            action.Should().Throw<ArgumentException>().WithMessage("Value cannot be null or empty. (Parameter 'UniqueId')");
        }

        [Test]
        public void TryAddItem_ArgumentNullException_NullValueZeroEntries()
        {
            var selector = new Selector<string>();
            Action action = () => selector.TryAddItem("a", null, 0);

            action.Should().Throw<ArgumentException>().WithMessage("Must have a value greater than 0. (Parameter 'Entries')");
        }

        /// <summary>
        /// Verifies that the values added are accepted and not randomized yet
        /// </summary>
        [Test]
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
        [Test]
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
        /// Checks the <code>RandomizeList</code> function to verify that
        /// values do not remain in the same order as to their insertion
        /// order.
        /// </summary>
        [Test]
        public void RandomizeList_Small()
        {
            var orderedList = new List<string>()
            {
                "a",
                "b",
                "c",
            };

            var retries = 3;
            var sameOrderCount = 0;
            for (int i = 0; i < retries; i++)
            {

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
                if (orderedCt == orderedList.Count)
                {
                    sameOrderCount++;
                }
            }
            // Make sure that at least one of the elements are not in the same
            // position.  Nearly all should not be.
            sameOrderCount.Should().BeLessThan(retries);
        }

        /// <summary>
        /// Checks the <code>RandomizeList</code> function to verify that
        /// values do not remain in the same order when the list is generated
        /// multiple times.
        /// </summary>
        [Test]
        public void RandomizeList_MultiCheckOrder()
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
            var randomizedListOne = selector.RandomizeList(orderedList);
            var randomizedListTwo = selector.RandomizeList(orderedList);
            var orderedCt = 0;

            for (int idx = 0; idx < orderedList.Count; idx++)
            {
                if (randomizedListOne[idx].Equals(randomizedListTwo[idx]))
                {
                    orderedCt++;
                }
            }

            orderedCt.Should().BeLessThan(orderedList.Count);
        }

        /// <summary>
        /// Verifies that the logic creates a list with the appropriate
        /// number of entries based on the <code>Item</code> definition.
        /// Also checks to see that the <code>RandomSelect</code> returns
        /// the correct number of entries for each item after randomization.
        /// </summary>
        [Test]
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

        /// <summary>
        /// Verifies that RandomSelect will select each added item from the source array
        /// </summary>
        [Test]
        public static void RandomSelect_SelectsAllElementsEventually()
        {
            var outputValues = new HashSet<string>();

            for (int numberOfSelections = 0; numberOfSelections < 1000; numberOfSelections++)
            {
                var selector = new Selector<string>();
                selector.TryAddItem("jen").Should().BeTrue();
                selector.TryAddItem("michael").Should().BeTrue();
                selector.TryAddItem("staci").Should().BeTrue();
                outputValues.Add(selector.RandomSelect(1).First().Value);
            }

            outputValues.Should().Contain("jen");
            outputValues.Should().Contain("michael");
            outputValues.Should().Contain("staci");
        }

        /// <summary>
        /// Verifies that the GenerateRandomIndex generates only numbers from 0 to 4 when an upperLimit
        /// of 5 is used.
        /// </summary>
        [Test]
        public static void GenerateRandomIndex_GeneratesAllIndicesEventually()
        {
            var outputValues = new HashSet<int>();

            for (int numberOfSelections = 0; numberOfSelections < 1000; numberOfSelections++)
            {
                var selector = new Selector<string>();

                outputValues.Add(selector.GenerateRandomIndex(5));
            }

            outputValues.Should().Contain(0);
            outputValues.Should().Contain(1);
            outputValues.Should().Contain(2);
            outputValues.Should().Contain(3);
            outputValues.Should().Contain(4);
            outputValues.Where(x => x < 0 || x > 4).Count().Should().Be(0);
        }


        /// <summary>
        /// Verifies that an exception is thrown if the caller passes an upperLimit value less than 1
        /// </summary>
        [Test]
        public static void GenerateRandomIndex_ArgumentOutOfRangeException()
        {
            var selector = new Selector<string>();

            // Exception Testing
            Action action = () => selector.GenerateRandomIndex(0);
            action.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"The value of 0 is invalid.  Please use a number greater than 0. (Parameter 'upperLimit')");

            action = () => selector.GenerateRandomIndex(-1);
            action.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"The value of -1 is invalid.  Please use a number greater than 0. (Parameter 'upperLimit')");

        }

        /// <summary>
        /// Tests randomness by verifying that GenerateRandomIndex produces a uniform distribution
        /// across the valid range using a basic statistical test
        /// </summary>
        [Test]
        public static void GenerateRandomIndex_UniformDistribution()
        {
            var selector = new Selector<string>();
            int rangeSize = 10;
            int iterations = 10000;
            var distribution = new Dictionary<int, int>();

            for (int i = 0; i < rangeSize; i++)
            {
                distribution[i] = 0;
            }

            for (int i = 0; i < iterations; i++)
            {
                int randomIndex = selector.GenerateRandomIndex(rangeSize);
                distribution[randomIndex]++;
            }

            // Each index should be selected roughly equally
            // Allow 25% deviation from expected frequency
            double expectedFrequency = (double)iterations / rangeSize;
            double tolerance = expectedFrequency * 0.25;

            foreach (var count in distribution.Values)
            {
                count.Should().BeGreaterThan((int)(expectedFrequency - tolerance));
                count.Should().BeLessThan((int)(expectedFrequency + tolerance));
            }
        }

        /// <summary>
        /// Verifies that multiple Selector instances produce different randomization results,
        /// ensuring that random seeds are not identical across instances
        /// </summary>
        [Test]
        public static void RandomSelect_IndependentInstances()
        {
            var orderedList = new List<string>() { "a", "b", "c", "d", "e", "f", "g" };
            var selector1 = new Selector<string>();
            var selector2 = new Selector<string>();

            var randomized1 = selector1.RandomizeList(orderedList);
            var randomized2 = selector2.RandomizeList(orderedList);

            // The probability of two independent random shuffles being identical is extremely low
            randomized1.SequenceEqual(randomized2).Should().BeFalse("Different instances should produce different randomizations");
        }

        /// <summary>
        /// Tests that RandomizeList properly shuffles a list with duplicate elements
        /// </summary>
        [Test]
        public void RandomizeList_WithDuplicates()
        {
            var listWithDuplicates = new List<string>() { "a", "a", "b", "b", "c", "c" };
            var selector = new Selector<string>();

            // Run randomization multiple times
            var randomized1 = selector.RandomizeList(listWithDuplicates);
            var randomized2 = selector.RandomizeList(listWithDuplicates);

            // Both should be valid permutations of the original
            randomized1.OrderBy(x => x).Should().Equal(listWithDuplicates.OrderBy(x => x));
            randomized2.OrderBy(x => x).Should().Equal(listWithDuplicates.OrderBy(x => x));

            // They should not always be the same
            randomized1.SequenceEqual(randomized2).Should().BeFalse("Multiple randomizations should produce different results");
        }

        /// <summary>
        /// Verifies that RandomSelect produces uniformly distributed selections across items
        /// when called multiple times
        /// </summary>
        [Test]
        public static void RandomSelect_UniformDistribution()
        {
            var selectionCounts = new Dictionary<string, int> { { "item1", 0 }, { "item2", 0 }, { "item3", 0 } };
            int iterations = 3000;

            for (int i = 0; i < iterations; i++)
            {
                var selector = new Selector<string>();
                selector.TryAddItem("item1").Should().BeTrue();
                selector.TryAddItem("item2").Should().BeTrue();
                selector.TryAddItem("item3").Should().BeTrue();

                var selected = selector.RandomSelect(1);
                selectionCounts[selected[0].Value.ToString()]++;
            }

            // Each item should be selected roughly 1/3 of the time
            // Allow 20% deviation
            double expectedFrequency = (double)iterations / 3;
            double tolerance = expectedFrequency * 0.20;

            foreach (var count in selectionCounts.Values)
            {
                count.Should().BeGreaterThan((int)(expectedFrequency - tolerance));
                count.Should().BeLessThan((int)(expectedFrequency + tolerance));
            }
        }

        /// <summary>
        /// Verifies that when selecting multiple items, the distribution of selections is uniform
        /// </summary>
        [Test]
        public static void RandomSelect_MultipleSelections_UniformDistribution()
        {
            var selector = new Selector<string>();
            selector.TryAddItem(new Item<string>() { UniqueId = "item1", Value = "value1", Entries = 2 });
            selector.TryAddItem(new Item<string>() { UniqueId = "item2", Value = "value2", Entries = 2 });
            selector.TryAddItem(new Item<string>() { UniqueId = "item3", Value = "value3", Entries = 2 });

            var selectionCounts = new Dictionary<string, int> { { "item1", 0 }, { "item2", 0 }, { "item3", 0 } };
            int iterations = 1000;

            for (int i = 0; i < iterations; i++)
            {
                var selector2 = new Selector<string>();
                selector2.TryAddItem(new Item<string>() { UniqueId = "item1", Value = "value1", Entries = 2 });
                selector2.TryAddItem(new Item<string>() { UniqueId = "item2", Value = "value2", Entries = 2 });
                selector2.TryAddItem(new Item<string>() { UniqueId = "item3", Value = "value3", Entries = 2 });

                var selected = selector2.RandomSelect(3);
                foreach (var item in selected)
                {
                    selectionCounts[item.UniqueId]++;
                }
            }

            // Each item should be selected roughly equally (proportional to entries)
            double expectedFrequency = (double)(iterations * 3) / 3;
            double tolerance = expectedFrequency * 0.20;

            foreach (var count in selectionCounts.Values)
            {
                count.Should().BeGreaterThan((int)(expectedFrequency - tolerance));
                count.Should().BeLessThan((int)(expectedFrequency + tolerance));
            }
        }

        /// <summary>
        /// Tests that RandomizeList consistently preserves all elements from the original list
        /// while changing their order
        /// </summary>
        [Test]
        public void RandomizeList_PreservesAllElements()
        {
            var orderedList = new List<string>() { "alpha", "bravo", "charlie", "delta", "echo" };
            var selector = new Selector<string>();

            for (int i = 0; i < 100; i++)
            {
                var randomized = selector.RandomizeList(orderedList);

                randomized.Count.Should().Be(orderedList.Count);
                randomized.OrderBy(x => x).Should().Equal(orderedList.OrderBy(x => x));
            }
        }

        /// <summary>
        /// Edge case: RandomSelect with single item should always return that item
        /// </summary>
        [Test]
        public void RandomSelect_SingleItem()
        {
            var selector = new Selector<string>();
            selector.TryAddItem("onlyItem").Should().BeTrue();

            var selected = selector.RandomSelect(1);

            selected.Count.Should().Be(1);
            selected[0].Value.Should().Be("onlyItem");
        }

        /// <summary>
        /// Edge case: RandomSelect requesting all items should return all items
        /// </summary>
        [Test]
        public void RandomSelect_AllItems()
        {
            var selector = new Selector<string>();
            selector.TryAddItem(new Item<string>() { UniqueId = "a", Value = "alpha", Entries = 2 });
            selector.TryAddItem(new Item<string>() { UniqueId = "b", Value = "bravo", Entries = 3 });

            var selected = selector.RandomSelect(5);

            selected.Count.Should().Be(5);
            var uniqueIds = selected.Select(x => x.UniqueId).ToList();
            uniqueIds.Count(x => x == "a").Should().Be(2);
            uniqueIds.Count(x => x == "b").Should().Be(3);
        }

        /// <summary>
        /// Verifies that the randomization is not just a simple rotation
        /// by testing that first and last elements change positions across runs
        /// </summary>
        [Test]
        public static void RandomizeList_NotSimpleRotation()
        {
            var orderedList = new List<string>() { "1", "2", "3", "4", "5" };
            var selector = new Selector<string>();

            var firstElementPositions = new HashSet<int>();
            var lastElementPositions = new HashSet<int>();

            for (int i = 0; i < 100; i++)
            {
                var randomized = selector.RandomizeList(orderedList);

                firstElementPositions.Add(randomized.IndexOf("1"));
                lastElementPositions.Add(randomized.IndexOf("5"));
            }

            // The first element should appear in multiple different positions
            firstElementPositions.Count.Should().BeGreaterThan(1);
            // The last element should appear in multiple different positions
            lastElementPositions.Count.Should().BeGreaterThan(1);
        }

        /// <summary>
        /// Tests the fluent API for adding items with method chaining
        /// </summary>
        [Test]
        public void AddItem_FluentAPI()
        {
            var selector = new Selector<string>();
            
            var result = selector
                .AddItem("a", "alpha")
                .AddItem("b", "bravo")
                .AddItem("c", "charlie", 2);

            result.Should().BeSameAs(selector);
            selector.Count.Should().Be(3);
            selector.TotalEntries.Should().Be(4); // 1 + 1 + 2
        }

        /// <summary>
        /// Tests that AddItem throws an exception when duplicate UniqueId is added
        /// </summary>
        [Test]
        public void AddItem_ThrowsOnDuplicate()
        {
            var selector = new Selector<string>();
            selector.AddItem("a", "alpha");

            Action action = () => selector.AddItem("a", "different");
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("*already exists*");
        }

        /// <summary>
        /// Tests the Count property
        /// </summary>
        [Test]
        public void Count_ReturnsCorrectValue()
        {
            var selector = new Selector<string>();
            selector.Count.Should().Be(0);

            selector.TryAddItem("a").Should().BeTrue();
            selector.Count.Should().Be(1);

            selector.TryAddItem("b", "bravo", 5).Should().BeTrue();
            selector.Count.Should().Be(2);
        }

        /// <summary>
        /// Tests the TotalEntries property
        /// </summary>
        [Test]
        public void TotalEntries_ReturnsCorrectValue()
        {
            var selector = new Selector<string>();
            selector.TotalEntries.Should().Be(0);

            selector.TryAddItem("a", "alpha", 1).Should().BeTrue();
            selector.TotalEntries.Should().Be(1);

            selector.TryAddItem("b", "bravo", 3).Should().BeTrue();
            selector.TotalEntries.Should().Be(4);

            selector.TryAddItem("c", "charlie", 2).Should().BeTrue();
            selector.TotalEntries.Should().Be(6);
        }

        /// <summary>
        /// Tests the Contains method
        /// </summary>
        [Test]
        public void Contains_ReturnsCorrectValue()
        {
            var selector = new Selector<string>();
            selector.Contains("a").Should().BeFalse();

            selector.TryAddItem("a", "alpha").Should().BeTrue();
            selector.Contains("a").Should().BeTrue();
            selector.Contains("A").Should().BeTrue(); // Case-insensitive
            selector.Contains("b").Should().BeFalse();
        }

        /// <summary>
        /// Tests the TryGetItem method
        /// </summary>
        [Test]
        public void TryGetItem_ReturnsItem()
        {
            var selector = new Selector<string>();
            var item = new Item<string>() { UniqueId = "a", Value = "alpha", Entries = 2 };
            selector.TryAddItem(item).Should().BeTrue();

            selector.TryGetItem("a", out var retrieved).Should().BeTrue();
            retrieved.Should().NotBeNull();
            retrieved.UniqueId.Should().Be("a");
            retrieved.Value.Should().Be("alpha");
            retrieved.Entries.Should().Be(2);

            selector.TryGetItem("nonexistent", out var notFound).Should().BeFalse();
            notFound.Should().BeNull();
        }

        /// <summary>
        /// Tests the RemoveItem method
        /// </summary>
        [Test]
        public void RemoveItem_SuccessfullyRemoves()
        {
            var selector = new Selector<string>();
            selector.TryAddItem("a", "alpha").Should().BeTrue();
            selector.TryAddItem("b", "bravo").Should().BeTrue();

            selector.Count.Should().Be(2);
            selector.RemoveItem("a").Should().BeTrue();
            selector.Count.Should().Be(1);
            selector.Contains("a").Should().BeFalse();
            selector.RemoveItem("a").Should().BeFalse();
        }

        /// <summary>
        /// Tests the Clear method
        /// </summary>
        [Test]
        public void Clear_RemovesAllItems()
        {
            var selector = new Selector<string>();
            selector.TryAddItem("a").Should().BeTrue();
            selector.TryAddItem("b").Should().BeTrue();
            selector.TryAddItem("c").Should().BeTrue();

            selector.Count.Should().Be(3);
            selector.Clear();
            selector.Count.Should().Be(0);
            selector.TotalEntries.Should().Be(0);
        }

        /// <summary>
        /// Tests the IEnumerable implementation
        /// </summary>
        [Test]
        public void IEnumerable_IteratesThroughItems()
        {
            var selector = new Selector<string>();
            selector.TryAddItem("a", "alpha").Should().BeTrue();
            selector.TryAddItem("b", "bravo").Should().BeTrue();
            selector.TryAddItem("c", "charlie").Should().BeTrue();

            var items = selector.ToList();
            items.Count.Should().Be(3);
            items.Should().Contain(x => x.UniqueId == "a");
            items.Should().Contain(x => x.UniqueId == "b");
            items.Should().Contain(x => x.UniqueId == "c");
        }

        /// <summary>
        /// Tests the Peek method for non-destructive preview
        /// </summary>
        [Test]
        public void Peek_ReturnsItemsWithoutRemoval()
        {
            var selector = new Selector<string>();
            selector.TryAddItem("a", "alpha", 2).Should().BeTrue();
            selector.TryAddItem("b", "bravo", 1).Should().BeTrue();

            var peeked1 = selector.Peek(2);
            peeked1.Count.Should().Be(2);

            var peeked2 = selector.Peek(2);
            peeked2.Count.Should().Be(2);

            // Both peeks should still be valid (non-destructive)
            selector.TotalEntries.Should().Be(3);
        }

        /// <summary>
        /// Tests that RandomSelect validates numToSelect parameter
        /// </summary>
        [Test]
        public void RandomSelect_ValidatesNumToSelect()
        {
            var selector = new Selector<string>();
            selector.TryAddItem("a").Should().BeTrue();

            Action action = () => selector.RandomSelect(0);
            action.Should().Throw<ArgumentOutOfRangeException>();

            action = () => selector.RandomSelect(-1);
            action.Should().Throw<ArgumentOutOfRangeException>();

            action = () => selector.RandomSelect(2);
            action.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Cannot select 2 items when only 1*");
        }
    }
}