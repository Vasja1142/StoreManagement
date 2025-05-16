// Файл: StoreManagement.Tests/Services/MyCollectionExtensionsTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreManagement.Services;
using StoreManagement.Collections;
using StoreManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreManagement.Tests.Services
{
    [TestClass]
    public class MyCollectionExtensionsTests
    {
        // Вспомогательный класс, если TestGoodsProduct еще не определен глобально
        public class TestGoodsProductExt : Product
        {
            public TestGoodsProductExt(string name, decimal price, string manufacturer, DateTime expDate)
                : base(name, price, manufacturer, expDate) { }
            public override void Init() { /* ... */ }
            public override void RandomInit() { /* ... */ }
        }

        private MyNewCollection<Goods> CreateSampleCollection(string name = "Sample")
        {
            var collection = new MyNewCollection<Goods>(name);
            collection.Add(new Product("Milk", 2.0m, "FarmCo", DateTime.Now.AddDays(5)));
            collection.Add(new Product("Bread", 1.5m, "BakeryInc", DateTime.Now.AddDays(2)));
            collection.Add(new Toy("Car", 15.0m, "ToyFactory", 3, "Plastic"));
            collection.Add(new Product("Cheese", 3.5m, "FarmCo", DateTime.Now.AddDays(10)));
            collection.Add(new Toy("Doll", 12.0m, "ToyFactory", 5, "Vinyl"));
            return collection;
        }

        // --- Filter ---
        [TestMethod]
        public void Filter_ReturnsItemsMatchingPredicate()
        {
            var collection = CreateSampleCollection();
            // Фильтруем только объекты Product
            var filtered = collection.Filter(item => item is Product);

            Assert.AreEqual(3, filtered.Count, "Filter should return only Product items.");
            Assert.IsTrue(filtered.All(item => item is Product), "All items in filtered collection should be Products.");
            Assert.IsTrue(filtered.Name.EndsWith("-Filtered"), "Filtered collection name is incorrect.");
        }

        [TestMethod]
        public void Filter_PredicateMatchesNoItems_ReturnsEmptyCollection()
        {
            var collection = CreateSampleCollection();
            var filtered = collection.Filter(item => item.Price > 100m); // Нет таких товаров
            Assert.AreEqual(0, filtered.Count);
        }

        [TestMethod]
        public void Filter_EmptySourceCollection_ReturnsEmptyCollection()
        {
            var emptyCollection = new MyNewCollection<Goods>("Empty");
            var filtered = emptyCollection.Filter(item => true); // Любой предикат
            Assert.AreEqual(0, filtered.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Filter_NullCollection_ThrowsArgumentNullException()
        {
            MyNewCollection<Goods> nullCollection = null;
            nullCollection.Filter(g => true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Filter_NullPredicate_ThrowsArgumentNullException()
        {
            var collection = CreateSampleCollection();
            collection.Filter(null);
        }

        // --- AggregateAverage ---
        [TestMethod]
        public void AggregateAverage_CalculatesAverageCorrectly()
        {
            var collection = new MyNewCollection<Goods>("AvgTest");
            collection.Add(new Product("P1", 10m, "M", DateTime.Now));
            collection.Add(new Product("P2", 20m, "M", DateTime.Now));
            collection.Add(new Toy("T1", 30m, "M", 0, "P")); // Goods, но не Product

            decimal average = collection.AggregateAverage(g => g.Price);
            Assert.AreEqual(20m, average, "Average price calculation is incorrect.");
        }

        [TestMethod]
        public void AggregateAverage_EmptyCollection_ReturnsZero()
        {
            var emptyCollection = new MyNewCollection<Goods>("EmptyAvg");
            decimal average = emptyCollection.AggregateAverage(g => g.Price);
            Assert.AreEqual(0m, average, "Average for empty collection should be 0.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AggregateAverage_NullCollection_ThrowsArgumentNullException()
        {
            MyNewCollection<Goods> nullCollection = null;
            nullCollection.AggregateAverage(g => g.Price);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AggregateAverage_NullSelector_ThrowsArgumentNullException()
        {
            var collection = CreateSampleCollection();
            collection.AggregateAverage(null);
        }



        [TestMethod]
        public void SortBy_EmptySourceCollection_ReturnsEmptyCollection()
        {
            var emptyCollection = new MyNewCollection<Goods>("EmptySort");
            var sorted = emptyCollection.SortBy(item => item.Name);
            Assert.AreEqual(0, sorted.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SortBy_NullCollection_ThrowsArgumentNullException()
        {
            MyNewCollection<Goods> nullCollection = null;
            nullCollection.SortBy(g => g.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SortBy_NullKeySelector_ThrowsArgumentNullException()
        {
            var collection = CreateSampleCollection();
            collection.SortBy<Goods, string>(null);
        }

        // --- GroupByCriteria ---
        [TestMethod]
        public void GroupByCriteria_GroupsItemsCorrectly_ByManufacturer()
        {
            var collection = CreateSampleCollection();
            // FarmCo: Milk, Cheese (2)
            // BakeryInc: Bread (1)
            // ToyFactory: Car, Doll (2)
            var grouped = collection.GroupByCriteria(item => item.Manufacturer).ToList();

            Assert.AreEqual(3, grouped.Count, "Should be 3 groups by manufacturer.");

            var farmCoGroup = grouped.FirstOrDefault(g => g.Key == "FarmCo");
            Assert.IsNotNull(farmCoGroup, "FarmCo group not found.");
            Assert.AreEqual(2, farmCoGroup.Count(), "FarmCo group should have 2 items.");

            var toyFactoryGroup = grouped.FirstOrDefault(g => g.Key == "ToyFactory");
            Assert.IsNotNull(toyFactoryGroup, "ToyFactory group not found.");
            Assert.AreEqual(2, toyFactoryGroup.Count(), "ToyFactory group should have 2 items.");

            var bakeryGroup = grouped.FirstOrDefault(g => g.Key == "BakeryInc");
            Assert.IsNotNull(bakeryGroup, "BakeryInc group not found.");
            Assert.AreEqual(1, bakeryGroup.Count(), "BakeryInc group should have 1 item.");
        }

        [TestMethod]
        public void GroupByCriteria_EmptySourceCollection_ReturnsEmptyEnumeration()
        {
            var emptyCollection = new MyNewCollection<Goods>("EmptyGroup");
            var grouped = emptyCollection.GroupByCriteria(item => item.Manufacturer).ToList();
            Assert.AreEqual(0, grouped.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GroupByCriteria_NullCollection_ThrowsArgumentNullException()
        {
            MyNewCollection<Goods> nullCollection = null;
            nullCollection.GroupByCriteria(g => g.Manufacturer);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GroupByCriteria_NullKeySelector_ThrowsArgumentNullException()
        {
            var collection = CreateSampleCollection();
            collection.GroupByCriteria<Goods, string>(null);
        }
    }
}