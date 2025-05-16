// Файл: StoreManagement.Tests/Services/PersistenceServiceTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreManagement.Services;
using StoreManagement.Collections;
using StoreManagement.Domain;
using System;
using System.IO; // Для Path, File
using System.Collections.Generic; // Для List

namespace StoreManagement.Tests.Services
{
    [TestClass]
    public class PersistenceServiceTests
    {
        // Вспомогательный класс, если TestGoodsProduct еще не определен глобально
        public class TestGoodsForPersistence : Product
        {
            public TestGoodsForPersistence() : base() { } // Нужен для XML сериализации
            public TestGoodsForPersistence(string name, decimal price, string manufacturer, DateTime expDate)
                : base(name, price, manufacturer, expDate) { }
            public override void Init() { /* ... */ }
            public override void RandomInit() { /* ... */ }
        }

        private string _testFilePath;

        [TestInitialize]
        public void TestInitialize()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), $"test_persistence_{Guid.NewGuid()}.dat");
            // Убедимся, что файл не существует перед тестами, которые его создают
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Очищаем после каждого теста
            if (File.Exists(_testFilePath))
            {
                try
                {
                    File.Delete(_testFilePath);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Warning: Could not delete test file '{_testFilePath}': {ex.Message}");
                }
            }
        }

        // --- SaveCollection Tests ---
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveCollection_NullCollection_ThrowsArgumentNullException()
        {
            PersistenceService.SaveCollection<TestGoodsForPersistence>(null, _testFilePath, SerializationFormat.Json);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveCollection_NullFilePath_ThrowsArgumentNullException()
        {
            var collection = new MyNewCollection<TestGoodsForPersistence>("Test");
            PersistenceService.SaveCollection(collection, null, SerializationFormat.Json);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveCollection_EmptyFilePath_ThrowsArgumentNullException()
        {
            var collection = new MyNewCollection<TestGoodsForPersistence>("Test");
            PersistenceService.SaveCollection(collection, "  ", SerializationFormat.Json); // Пробелы
        }


        // Тесты на успешное сохранение (Binary, Json, Xml) являются больше интеграционными.
        // Здесь мы можем проверить, что метод пытается выполнить операцию без ошибок, если аргументы корректны.
        [TestMethod]
        public void SaveCollection_EmptyCollection_CreatesFile_Json()
        {
            var collection = new MyNewCollection<TestGoodsForPersistence>("EmptySave");
            PersistenceService.SaveCollection(collection, _testFilePath, SerializationFormat.Json);
            Assert.IsTrue(File.Exists(_testFilePath), "File should be created for empty collection (JSON).");
            // Можно проверить, что файл содержит пустой JSON массив "[]"
            string content = File.ReadAllText(_testFilePath);
            Assert.AreEqual("[]", content.Trim().Replace(Environment.NewLine, "").Replace(" ", ""));
        }

        [TestMethod]
        public void SaveCollection_WithData_CreatesFile_Json()
        {
            var collection = new MyNewCollection<TestGoodsForPersistence>("DataSave");
            collection.Add(new TestGoodsForPersistence("Item1", 10m, "Man1", DateTime.Today));

            PersistenceService.SaveCollection(collection, _testFilePath, SerializationFormat.Json);

            Assert.IsTrue(File.Exists(_testFilePath));
            string content = File.ReadAllText(_testFilePath);
            Assert.IsTrue(content.Contains("Item1"), "Saved JSON should contain item data.");
        }

        // Аналогичные тесты можно написать для Binary и Xml, проверяя создание файла
        // и, возможно, базовую структуру содержимого.

        // --- LoadCollection Tests ---
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadCollection_NullFilePath_ThrowsArgumentNullException()
        {
            PersistenceService.LoadCollection<TestGoodsForPersistence>("Test", null, SerializationFormat.Json);
        }

        


        // Тест на успешную загрузку данных (интеграционный)
        [TestMethod]
        public void LoadCollection_LoadsDataCorrectlyFromJson()
        {
            // 1. Сохраняем данные
            var originalCollection = new MyNewCollection<TestGoodsForPersistence>("Original");
            var item1 = new TestGoodsForPersistence("LoadItem1", 25.5m, "LoaderMan", DateTime.Now.Date.AddDays(3));
            originalCollection.Add(item1);
            PersistenceService.SaveCollection(originalCollection, _testFilePath, SerializationFormat.Json);

            // 2. Загружаем данные
            var loadedCollection = PersistenceService.LoadCollection<TestGoodsForPersistence>("Loaded", _testFilePath, SerializationFormat.Json);

            // 3. Проверяем
            Assert.AreEqual(1, loadedCollection.Count, "Loaded collection should have one item.");
            var loadedItem = loadedCollection.First();
            Assert.AreEqual(item1.Name, loadedItem.Name);
            Assert.AreEqual(item1.Price, loadedItem.Price);
            Assert.AreEqual(item1.Manufacturer, loadedItem.Manufacturer);
            Assert.AreEqual(item1.ExpirationDate, loadedItem.ExpirationDate); // Сравнение DateTime
        }


        
    }
}