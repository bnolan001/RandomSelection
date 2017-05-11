using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using RandomSelection;
using System.Collections.Generic;
using RandomSelection.Library;
using System.Linq;

namespace Testing.Unit
{
    [TestClass]
    public class SelectorTests
    {
        [TestMethod]
        public void GenerateRandomIndex()
        {
            Selector selector = new Selector();
            int startNumber = 1000000000;
            while (startNumber > 0)
            {
                var index = selector.GenerateRandomIndex(startNumber);
                index.Should().BeGreaterOrEqualTo(0);
                index.Should().BeLessOrEqualTo(startNumber);
                startNumber = startNumber / 10;
            }
        }

        [TestMethod]
        public void TryAddItem()
        {
            Selector selector = new Selector();
            var item = new Item()
            {
                UniqueId = "a",
                Name = "alpha",
                Weight = 1
            };
            selector.TryAddItem(item).Should().BeTrue();
            selector.TryAddItem("b").Should().BeTrue();
            selector.TryAddItem("c", "charlie").Should().BeTrue();
            selector.TryAddItem("d", "delta", 2).Should().BeTrue();

            selector.TryAddItem("b").Should().BeFalse();
            selector.TryAddItem(item).Should().BeFalse();
            selector.TryAddItem("c", "charlie").Should().BeFalse();
            selector.TryAddItem("d", "delta", 2).Should().BeFalse();
        }

        [TestMethod]
        public void GenerateList()
        {
            Selector selector = new Selector();
            var item = new Item()
            {
                UniqueId = "a",
                Name = "alpha",
                Weight = 1
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

            var selector = new Selector();
            var randomizedList = selector.RandomizeList(orderedList);

            int orderedCt = 0;
            for (var idx = 0; idx < orderedList.Count; idx++)
            {
                if (orderedList[idx].Equals(randomizedList[idx]))
                {
                    orderedCt++;
                }
            }

            orderedCt.Should().BeLessThan(orderedList.Count);
        }

        [TestMethod]
        public void Select()
        {
            var selector = new Selector();
            selector.TryAddItem(new Item()
            {
                UniqueId = "a",
                Name = "alpha",
                Weight = 2
            });
            selector.TryAddItem(new Item()
            {
                UniqueId = "b",
                Name = "bravo",
                Weight = 1
            });
            selector.TryAddItem(new Item()
            {
                UniqueId = "c",
                Name = "charlie",
                Weight = 5
            });
            selector.TryAddItem(new Item()
            {
                UniqueId = "d",
                Name = "delta",
                Weight = 1
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