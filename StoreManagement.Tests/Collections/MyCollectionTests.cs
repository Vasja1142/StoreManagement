// Файл: StoreManagement.Tests/Collections/MyCollectionTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreManagement.Collections;
using StoreManagement.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StoreManagement.Tests.Collections
{
    [TestClass]
    public class MyCollectionTests
    {
        // Вспомогательный класс, наследующий Goods, для тестов
        public class TestableGoods : Product // Product уже реализует ICloneable
        {
            public TestableGoods(string name = "TestGood", decimal price = 10m, string manufacturer = "TestMan", DateTime? expDate = null)
                : base(name, price, manufacturer, expDate ?? DateTime.Now.AddDays(7)) { }

            // Init и RandomInit не будут тестироваться здесь напрямую, но должны быть реализованы
            public override void Init() { /* Для теста не важно */ }
            public override void RandomInit() { /* Для теста не важно */ }
        }

        // Вспомогательный класс Goods, который "плохо" клонируется для теста
        public class BadCloneGoods : Goods
        {
            public BadCloneGoods(string n, decimal p, string m) : base(n, p, m) { }
            public override void Init() { }
            public override void RandomInit() { }
            public override object Clone() => "I am not a Goods object"; // Возвращает несовместимый тип
        }


        [TestMethod]
        public void Constructor_Default_CreatesEmptyCollection()
        {
            var collection = new MyCollection<TestableGoods>();
            Assert.AreEqual(0, collection.Count, "Default constructor should create an empty collection.");
        }

        [TestMethod]
        public void Constructor_WithCapacity_InitializesCorrectly()
        {
            int capacity = 16;
            var collection = new MyCollection<TestableGoods>(capacity);
            Assert.AreEqual(0, collection.Count, "Constructor with capacity should create an empty collection.");
            // Проверяем, что внутренняя HashTable имеет ожидаемую емкость
            var expectedHashTableCapacity = new HashTable<string, TestableGoods>(capacity).Capacity;
            Assert.AreEqual(expectedHashTableCapacity, collection._dataTable.Capacity, "Internal HashTable capacity is not set as expected.");
        }

        [TestMethod]
        public void Constructor_CopyMyCollection_CopiesItemsAndNotReferences()
        {
            var originalCollection = new MyCollection<TestableGoods>();
            var item1 = new TestableGoods("item1");
            originalCollection.Add(item1);

            var copiedCollection = new MyCollection<TestableGoods>(originalCollection);

            Assert.AreEqual(1, copiedCollection.Count, "Copied collection should have the same number of items.");
            Assert.IsTrue(copiedCollection.Contains(item1), "Copied collection should contain the same items (by value/name).");
            Assert.AreNotSame(originalCollection._dataTable, copiedCollection._dataTable, "Internal HashTable instance should be different.");
        }



        [TestMethod]
        public void Constructor_FromIEnumerable_CopiesItems()
        {
            var sourceList = new List<TestableGoods> { new TestableGoods("list_item1") };
            var collection = new MyCollection<TestableGoods>(sourceList);
            Assert.AreEqual(1, collection.Count);
            Assert.IsTrue(collection.Any(g => g.Name == "list_item1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FromIEnumerable_NullSource_ThrowsArgumentNullException()
        {
            new MyCollection<TestableGoods>((IEnumerable<TestableGoods>)null);
        }

        [TestMethod]
        public void Add_NewItem_IncreasesCountAndReturnsTrue()
        {
            var collection = new MyCollection<TestableGoods>();
            var item = new TestableGoods("newItem");
            bool result = collection.Add(item);
            Assert.IsTrue(result, "Add should return true for a new item.");
            Assert.AreEqual(1, collection.Count, "Count should increment after adding a new item.");
            Assert.IsTrue(collection.Contains(item), "Collection should contain the added item.");
        }

        [TestMethod]
        public void Add_ExistingItemName_UpdatesItemAndReturnsFalse()
        {
            var collection = new MyCollection<TestableGoods>();
            var item1 = new TestableGoods("commonName", 10m);
            collection.Add(item1);
            var item2_updated = new TestableGoods("commonName", 20m);

            bool result = collection.Add(item2_updated);

            Assert.IsFalse(result, "Add should return false when updating an existing item.");
            Assert.AreEqual(1, collection.Count, "Count should not change when updating.");
            var retrieved = collection.First(g => g.Name == "commonName");
            Assert.AreEqual(20m, retrieved.Price, "Item's property should be updated.");
            Assert.AreSame(item2_updated, retrieved, "The reference in the collection should be updated to the new item.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullItem_ThrowsArgumentNullException()
        {
            var collection = new MyCollection<TestableGoods>();
            collection.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_ItemWithNullOrEmptyName_ThrowsArgumentException()
        {
            var collection = new MyCollection<TestableGoods>();
            // TestableGoods конструктор может не позволить null имя, если есть валидация в Goods или Product
            // Goods.Name имеет сеттер, который не проверяет на null/empty.
            var item = new TestableGoods(name: null);
            collection.Add(item); // Должно упасть из-за проверки в MyCollection.Add
        }


        [TestMethod]
        public void Remove_ItemWithNullName_ReturnsFalseAndDoesNotChangeCollection()
        {
            var collection = new MyCollection<TestableGoods>();
            collection.Add(new TestableGoods("validName"));
            var itemWithNullName = new TestableGoods(name: null); // Создаем объект, но его Name невалиден для операций

            bool removed = collection.Remove(itemWithNullName); // MyCollection.Remove проверяет item.Name

            Assert.IsFalse(removed, "Remove should return false for item with null name.");
            Assert.AreEqual(1, collection.Count, "Collection count should not change.");
        }

        [TestMethod]
        public void CopyTo_ValidArrayAndIndex_CopiesElementsCorrectly()
        {
            var collection = new MyCollection<TestableGoods>();
            var item1 = new TestableGoods("c1");
            var item2 = new TestableGoods("c2");
            collection.Add(item1);
            collection.Add(item2);

            TestableGoods[] array = new TestableGoods[3];
            collection.CopyTo(array, 1); // Копируем в массив, начиная с индекса 1

            Assert.IsNull(array[0], "Element at index 0 should not be touched.");
            // Порядок элементов из HashTable не гарантирован, поэтому проверяем наличие
            var copiedElements = new List<TestableGoods> { array[1], array[2] };
            CollectionAssert.Contains(copiedElements, item1, "Item1 not found in target array.");
            CollectionAssert.Contains(copiedElements, item2, "Item2 not found in target array.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyTo_IncompatibleArrayType_ThrowsArgumentException()
        {
            var collection = new MyCollection<TestableGoods>();
            collection.Add(new TestableGoods("item"));
            // Массив другого типа (даже если object, но внутренний SetValue может упасть при несоответствии)
            // MyCollection.CopyTo ловит InvalidCastException и бросает ArgumentException
            string[] array = new string[1];
            collection.CopyTo(array, 0);
        }


        [TestMethod]
        public void ShallowCopy_CreatesNewCollectionWithSameItemReferences()
        {
            var originalCollection = new MyCollection<TestableGoods>();
            var item1 = new TestableGoods("item1", 10m);
            originalCollection.Add(item1);

            var shallowCopy = originalCollection.ShallowCopy();

            Assert.AreNotSame(originalCollection, shallowCopy, "ShallowCopy should create a new collection instance.");
            var itemFromCopy = shallowCopy.First();
            Assert.AreSame(item1, itemFromCopy, "Items in shallow copy should be the same references as in original.");

            itemFromCopy.Price = 20m; // Изменяем свойство объекта через ссылку из копии
            Assert.AreEqual(20m, item1.Price, "Change to item in shallow copy should reflect in original item.");
        }



        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Clone_ItemCloneReturnsIncompatibleType_ThrowsInvalidOperationException()
        {
            // Используем BadCloneGoods, чей Clone() возвращает строку
            var collection = new MyCollection<BadCloneGoods>();
            collection.Add(new BadCloneGoods("bad", 1m, "bad_man"));

            collection.Clone(); // Должно упасть, так как "string" не является BadCloneGoods
        }

        // Тест на ветку typeof(T).IsValueType в MyCollection.Clone() не применим,
        // так как T ограничено `where T : Goods`, а Goods - это класс (ссылочный тип).

        [TestMethod]
        public void Indexer_Get_ValidIndex_ReturnsCorrectItem()
        {
            var collection = new MyCollection<TestableGoods>();
            var item0 = new TestableGoods("idx_item0");
            var item1 = new TestableGoods("idx_item1");
            collection.Add(item0);
            collection.Add(item1);

            // Порядок не гарантирован, но мы должны получить оба элемента по индексам 0 и 1
            var retrieved0 = collection[0];
            var retrieved1 = collection[1];

            // Убедимся, что это разные элементы и они из нашей коллекции
            Assert.AreNotSame(retrieved0, retrieved1, "Indexer returned same item for different indices.");
            bool foundItem0 = (retrieved0.Name == item0.Name && retrieved1.Name == item1.Name) ||
                              (retrieved0.Name == item1.Name && retrieved1.Name == item0.Name);
            Assert.IsTrue(foundItem0, "Items retrieved by indexer do not match added items.");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Indexer_Set_ThrowsNotSupportedExceptionInMyCollection()
        {
            var collection = new MyCollection<TestableGoods>();
            collection.Add(new TestableGoods("original"));
            collection[0] = new TestableGoods("new"); // Сеттер в MyCollection не поддерживается
        }

        [TestMethod]
        public void InternalTryReplaceAt_ReplacesItemAndReturnsOldItem()
        {
            var collection = new MyCollection<TestableGoods>();
            var oldItem = new TestableGoods("oldName", 10m);
            collection.Add(oldItem);
            var newItem = new TestableGoods("newName", 20m);

            // Находим индекс oldItem (порядок не гарантирован, но для одного элемента это 0)
            // Для надежности, если бы элементов было много:
            int indexToReplace = collection.Select((item, idx) => new { item, idx })
                                           .First(x => x.item.Name == "oldName").idx;

            bool success = collection.InternalTryReplaceAt(indexToReplace, newItem, out TestableGoods replacedItem);

            Assert.IsTrue(success, "InternalTryReplaceAt should succeed.");
            Assert.AreSame(oldItem, replacedItem, "Replaced item should be the original old item.");
            Assert.AreEqual(1, collection.Count, "Count should remain the same after replacement.");
            Assert.IsTrue(collection.Contains(newItem), "Collection should contain the new item.");
            Assert.IsFalse(collection.Contains(oldItem), "Collection should no longer contain the old item by its original name key.");
            Assert.AreEqual(newItem.Price, collection.First(g => g.Name == "newName").Price);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InternalTryReplaceAt_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            var collection = new MyCollection<TestableGoods>();
            collection.InternalTryReplaceAt(0, new TestableGoods("new"), out _); // Коллекция пуста
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InternalTryReplaceAt_NullNewItem_ThrowsArgumentNullException()
        {
            var collection = new MyCollection<TestableGoods>();
            collection.Add(new TestableGoods("item"));
            collection.InternalTryReplaceAt(0, null, out _);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InternalTryReplaceAt_NewItemWithNullName_ThrowsArgumentException()
        {
            var collection = new MyCollection<TestableGoods>();
            collection.Add(new TestableGoods("item"));
            var newItemWithNullName = new TestableGoods(name: null);
            collection.InternalTryReplaceAt(0, newItemWithNullName, out _);
        }
    }
}