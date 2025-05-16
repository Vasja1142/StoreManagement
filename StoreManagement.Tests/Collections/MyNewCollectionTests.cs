// StoreManagement.Tests/MyNewCollectionTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreManagement.Collections;
using StoreManagement.Domain;
using System;
using System.Collections.Generic; // Для List<T>
using System.Linq;

namespace StoreManagement.Tests.Collections
{
    [TestClass]
    public partial class MyNewCollectionTests
    {
        // Вспомогательный метод для создания тестовых объектов Goods
        // (TestGoods определен в конце этого файла, как и раньше)
        private TestGoods CreateTestGood(string name = "TestGood", decimal price = 10m, int idSuffix = 0)
        {
            // Добавляем суффикс для большей уникальности имен в тестах, если нужно
            return new TestGoods(name + (idSuffix > 0 ? $"_{idSuffix}" : ""), price, "TestMan", DateTime.Now.AddDays(7));
        }

        // --- Тесты, которые у нас уже были (возможно, с небольшими уточнениями) ---
        [TestMethod]
        public void Add_NewItem_FiresCollectionCountChanged_Add_And_IncreasesCount()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("TestCollection_Add");
            CollectionHandlerEventArgs? receivedArgs = null;
            int eventFiredCount = 0;

            collection.CollectionCountChanged += (source, args) =>
            {
                receivedArgs = args;
                eventFiredCount++;
            };
            var newItem = CreateTestGood("Good1");

            // Act
            bool addedNew = collection.Add(newItem); // MyNewCollection.Add возвращает bool

            // Assert
            Assert.IsTrue(addedNew, "Add should return true for a new item.");
            Assert.AreEqual(1, eventFiredCount, "CollectionCountChanged event should fire once.");
            Assert.IsNotNull(receivedArgs, "Event arguments should not be null.");
            Assert.AreEqual(collection.Name, receivedArgs.CollectionName);
            Assert.AreEqual(ChangeInfo.Add, receivedArgs.ChangeType);
            Assert.AreSame(newItem, receivedArgs.ChangedItem);
            Assert.AreEqual(1, collection.Count);
            Assert.IsTrue(collection.Contains(newItem));
        }

        [TestMethod]
        public void Add_ExistingItemName_UpdatesItem_DoesNotFireCollectionCountChanged_CountUnchanged()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("TestCollection_Update");
            var item1 = CreateTestGood("UniqueName", 100m);
            collection.Add(item1);

            int eventFiredCount = 0;
            collection.CollectionCountChanged += (source, args) => { eventFiredCount++; };
            var item2_updated = CreateTestGood("UniqueName", 200m);

            // Act
            bool addedNew = collection.Add(item2_updated);

            // Assert
            Assert.IsFalse(addedNew, "Add should return false when updating.");
            Assert.AreEqual(0, eventFiredCount, "CollectionCountChanged should NOT fire for update.");
            Assert.AreEqual(1, collection.Count);
            var retrievedItem = collection.First(g => g.Name == "UniqueName"); // Используем First, т.к. уверены, что он там
            Assert.AreEqual(200m, retrievedItem.Price);
            Assert.AreSame(item2_updated, retrievedItem); // Проверяем, что ссылка обновилась
        }

        [TestMethod]
        public void Remove_ExistingItem_FiresCollectionCountChanged_Remove_And_DecreasesCount()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("TestCollection_Remove");
            var itemToRemove = CreateTestGood("GoodToRemove");
            collection.Add(itemToRemove);

            CollectionHandlerEventArgs? receivedArgs = null;
            int eventFiredCount = 0;
            collection.CollectionCountChanged += (source, args) => { receivedArgs = args; eventFiredCount++; };

            // Act
            bool removed = collection.Remove(itemToRemove);

            // Assert
            Assert.IsTrue(removed);
            Assert.AreEqual(1, eventFiredCount);
            Assert.IsNotNull(receivedArgs);
            Assert.AreEqual(ChangeInfo.Remove, receivedArgs.ChangeType);
            Assert.AreSame(itemToRemove, receivedArgs.ChangedItem);
            Assert.AreEqual(0, collection.Count);
            Assert.IsFalse(collection.Contains(itemToRemove));
        }

        [TestMethod]
        public void Remove_NonExistingItem_DoesNotFireEvent_CountUnchanged()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("TestCollection_RemoveNonExisting");
            collection.Add(CreateTestGood("Existing"));
            int eventFiredCount = 0;
            collection.CollectionCountChanged += (source, args) => { eventFiredCount++; };
            var itemNotPresent = CreateTestGood("NotPresent");

            // Act
            bool removed = collection.Remove(itemNotPresent);

            // Assert
            Assert.IsFalse(removed);
            Assert.AreEqual(0, eventFiredCount);
            Assert.AreEqual(1, collection.Count);
        }


        // --- Новые тесты ---

        [TestMethod]
        public void Clear_NonEmptyCollection_FiresRemoveForEachItem_CountIsZero()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("ClearTest");
            var item1 = CreateTestGood("ItemC1", 10m, 1);
            var item2 = CreateTestGood("ItemC2", 20m, 2);
            // Добавляем без отслеживания событий для этого этапа
            var tempCollectionForSetup = new MyCollection<Goods>(); // Используем базовый, чтобы не было событий
            tempCollectionForSetup.Add(item1);
            tempCollectionForSetup.Add(item2);
            collection = new MyNewCollection<Goods>("ClearTest_SetupDone", tempCollectionForSetup); // Создаем с уже добавленными

            List<object?> removedItemsInEvents = new List<object?>();
            int eventFireCount = 0;
            collection.CollectionCountChanged += (source, args) =>
            {
                if (args.ChangeType == ChangeInfo.Remove)
                {
                    removedItemsInEvents.Add(args.ChangedItem);
                }
                eventFireCount++;
            };

            // Act
            collection.Clear();

            // Assert
            Assert.AreEqual(0, collection.Count, "Collection count should be 0 after Clear.");
            Assert.AreEqual(2, eventFireCount, "CollectionCountChanged (Remove) should fire for each item during Clear.");
            Assert.AreEqual(2, removedItemsInEvents.Count, "Two items should be reported as removed.");
            // Порядок удаления из HashTable не гарантирован, поэтому проверяем наличие
            Assert.IsTrue(removedItemsInEvents.Any(ri => ReferenceEquals(ri, item1) || ri is Goods g && g.Name == item1.Name));
            Assert.IsTrue(removedItemsInEvents.Any(ri => ReferenceEquals(ri, item2) || ri is Goods g && g.Name == item2.Name));
        }

        [TestMethod]
        public void Clear_EmptyCollection_DoesNotFireEvents_CountRemainsZero()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("EmptyClearTest");
            int eventFireCount = 0;
            collection.CollectionCountChanged += (source, args) => { eventFireCount++; };

            // Act
            collection.Clear();

            // Assert
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(0, eventFireCount);
        }

        [TestMethod]
        public void RemoveAt_ExistingIndex_FiresRemoveEvent_DecreasesCount()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("RemoveAtTest");
            var item0 = CreateTestGood("ItemAt0", 10m, 1);
            var item1 = CreateTestGood("ItemAt1", 20m, 2); // Элемент, который будем удалять
            var item2 = CreateTestGood("ItemAt2", 30m, 3);
            collection.Add(item0);
            collection.Add(item1);
            collection.Add(item2);

            // Найдем фактический "индекс" item1 в текущем состоянии коллекции
            // Помним, что порядок в HashTable не гарантирован, но ElementAt(i) даст нам i-ый элемент из Values
            int indexToRemove = -1;
            for (int i = 0; i < collection.Count; i++)
            {
                if (ReferenceEquals(collection.ElementAt(i), item1)) // Сравниваем ссылки
                {
                    indexToRemove = i;
                    break;
                }
            }
            Assert.AreNotEqual(-1, indexToRemove, "Setup error: item1 not found to determine its index.");


            CollectionHandlerEventArgs? receivedArgs = null;
            int eventFireCount = 0;
            collection.CollectionCountChanged += (source, args) =>
            {
                receivedArgs = args;
                eventFireCount++;
            };

            // Act
            bool removed = collection.RemoveAt(indexToRemove);

            // Assert
            Assert.IsTrue(removed);
            Assert.AreEqual(1, eventFireCount);
            Assert.IsNotNull(receivedArgs);
            Assert.AreEqual(ChangeInfo.Remove, receivedArgs.ChangeType);
            // Важно: ChangedItem в событии должен быть тем элементом, который был на этом индексе
            Assert.AreSame(item1, receivedArgs.ChangedItem);
            Assert.AreEqual(2, collection.Count);
            Assert.IsFalse(collection.Contains(item1));
            Assert.IsTrue(collection.Contains(item0));
            Assert.IsTrue(collection.Contains(item2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RemoveAt_IndexOutOfRange_ThrowsException()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("RemoveAtRangeTest");
            collection.Add(CreateTestGood("Item1"));

            // Act
            collection.RemoveAt(5); // Несуществующий индекс
        }



        [TestMethod]
        public void Indexer_Set_NewName_FiresReferenceChanged_UpdatesCollection()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("IndexerSetNewName");
            var originalItem = CreateTestGood("OriginalName", 10m, 1);
            collection.Add(originalItem);

            // Найдем "индекс" originalItem
            int indexToSet = collection.ToList().IndexOf(originalItem); // ToList() создает копию в текущем порядке итерации
            Assert.AreNotEqual(-1, indexToSet, "Original item not found in collection for test setup.");


            CollectionHandlerEventArgs? receivedArgs = null;
            int eventFiredCount = 0;
            collection.CollectionReferenceChanged += (source, args) =>
            {
                receivedArgs = args;
                eventFiredCount++;
            };
            int countChangedEventFired = 0; // Убедимся, что CountChanged не срабатывает
            collection.CollectionCountChanged += (s, a) => countChangedEventFired++;


            var newItem = CreateTestGood("NewName", 20m, 2);

            // Act
            collection[indexToSet] = newItem;

            // Assert
            Assert.AreEqual(1, eventFiredCount, "CollectionReferenceChanged should fire once.");
            Assert.AreEqual(0, countChangedEventFired, "CollectionCountChanged should NOT fire.");
            Assert.IsNotNull(receivedArgs);
            Assert.AreEqual(ChangeInfo.Reference, receivedArgs.ChangeType);
            Assert.AreSame(newItem, receivedArgs.ChangedItem);
            Assert.AreEqual(1, collection.Count, "Count should remain 1 as it's a replacement.");
            Assert.IsTrue(collection.Contains(newItem));
            Assert.IsFalse(collection.Contains(originalItem), "Original item (with old name) should be removed.");
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Indexer_Set_IndexOutOfRange_ThrowsException()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("IndexerRangeTest");
            collection.Add(CreateTestGood("Item1"));

            // Act
            collection[5] = CreateTestGood("New"); // Несуществующий индекс
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Indexer_Set_NullValue_ThrowsArgumentNullException()
        {
            // Arrange
            var collection = new MyNewCollection<Goods>("IndexerNullTest");
            collection.Add(CreateTestGood("Item1"));

            // Act
            collection[0] = null!; // Пытаемся установить null
        }

        // Тесты для конструкторов MyNewCollection (и косвенно MyCollection)
        [TestMethod]
        public void Constructor_Default_CreatesEmptyCollection()
        {
            var collection = new MyNewCollection<Goods>("DefaultCtor");
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual("DefaultCtor", collection.Name);
        }

        [TestMethod]
        public void Constructor_FromIEnumerable_CopiesItems()
        {
            var sourceList = new List<Goods> { CreateTestGood("Src1", 1, 1), CreateTestGood("Src2", 1, 2) };
            var collection = new MyNewCollection<Goods>("FromIEnumerable", sourceList);

            Assert.AreEqual(2, collection.Count);
            Assert.IsTrue(collection.Any(g => g.Name == "Src1_1"));
            Assert.IsTrue(collection.Any(g => g.Name == "Src2_2"));
        }



        // TODO: Тесты для Clone и ShallowCopy для MyCollection/MyNewCollection
        //       Они будут похожи на тесты для доменных классов, но для коллекции в целом.
    }

    // Вспомогательный класс TestGoods остается тем же
    public class TestGoods : Product
    {
        public TestGoods(string name, decimal price, string manufacturer, DateTime expirationDate)
            : base(name, price, manufacturer, expirationDate) { }
        public override void Init() { }
        public override void RandomInit() { }
    }




}
