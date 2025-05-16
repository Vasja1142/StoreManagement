// Файл: StoreManagement.Tests/Services/JournalTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreManagement.Services;
using StoreManagement.Collections; // Для CollectionHandlerEventArgs
using StoreManagement.Domain;     // Для Product
using System;
using System.IO; // Для Path

namespace StoreManagement.Tests.Services
{
    [TestClass]
    public class JournalTests
    {
        private string _testLogFileName = $"test_journal_{Guid.NewGuid()}.log";
        private string _testLogFilePath;

        [TestInitialize]
        public void TestInitialize()
        {
            // Определяем путь для тестового лог-файла в текущей директории выполнения тестов
            _testLogFilePath = Path.Combine(AppContext.BaseDirectory, _testLogFileName);

            // Удаляем файл, если он остался от предыдущих запусков, чтобы конструктор Journal мог его создать
            if (File.Exists(_testLogFilePath))
            {
                File.Delete(_testLogFilePath);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Очищаем после каждого теста
            if (File.Exists(_testLogFilePath))
            {
                try
                {
                    File.Delete(_testLogFilePath);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Warning: Could not delete test log file '{_testLogFilePath}': {ex.Message}");
                }
            }
        }

        [TestMethod]
        public void Constructor_CreatesLogFileAndInitializes()
        {
            // Arrange & Act
            // Конструктор должен создать файл _testLogFileName
            // Для этого теста важно, чтобы TestInitialize удалил старый файл
            var journal = new Journal(_testLogFileName);
            string actualPath = journal.GetActualLogFilePath();

            // Assert
            Assert.IsTrue(File.Exists(actualPath), $"Log file '{actualPath}' should be created by constructor.");
            Assert.AreEqual(Path.Combine(AppContext.BaseDirectory, _testLogFileName), actualPath, "Actual log file path is not as expected.");

            string content = File.ReadAllText(actualPath);
            Assert.IsTrue(content.Contains("--- Журнал инициализирован (или файл создан)"), "Log file initial entry is missing.");
        }

        [TestMethod]
        public void Constructor_DeletesExistingLogFileBeforeInitialization()
        {
            // Arrange
            // 1. Создаем "старый" файл журнала
            File.WriteAllText(_testLogFilePath, "Old log content.");
            Assert.IsTrue(File.Exists(_testLogFilePath), "Pre-condition: Old log file should exist.");

            // Act: Конструктор Journal должен удалить старый файл и создать новый
            var journal = new Journal(_testLogFileName);
            string actualPath = journal.GetActualLogFilePath();

            // Assert
            Assert.IsTrue(File.Exists(actualPath), "New log file should be created.");
            string content = File.ReadAllText(actualPath);
            Assert.IsFalse(content.Contains("Old log content."), "Old log content should have been deleted.");
            Assert.IsTrue(content.Contains("--- Журнал инициализирован"), "New log file should have initialization message.");
        }

        [TestMethod]
        public void GetActualLogFilePath_ReturnsCorrectPath()
        {
            var journal = new Journal(_testLogFileName);
            string expectedPath = Path.Combine(AppContext.BaseDirectory, _testLogFileName);
            Assert.AreEqual(expectedPath, journal.GetActualLogFilePath());
        }

        // Тесты для LogToFile и ReadLogFile с реальными файловыми операциями
        // являются больше интеграционными, но здесь мы можем проверить базовую функциональность.

        [TestMethod]
        public void LogToFile_WritesMessageToLogFile()
        {
            // Arrange
            var journal = new Journal(_testLogFileName);
            string message1 = "Test log message 1.";
            string message2 = "Another test message.";

            // Act
            journal.CollectionCountChangedHandler(this, new CollectionHandlerEventArgs("TestColl", ChangeInfo.Add, message1));
            journal.CollectionReferenceChangedHandler(this, new CollectionHandlerEventArgs("TestColl", ChangeInfo.Reference, message2));

            // Assert
            string logContent = File.ReadAllText(journal.GetActualLogFilePath());
            Assert.IsTrue(logContent.Contains(message1), "First message not found in log.");
            Assert.IsTrue(logContent.Contains(message2), "Second message not found in log.");
            Assert.IsTrue(logContent.Contains("Тип: Add"), "ChangeType.Add not logged.");
            Assert.IsTrue(logContent.Contains("Тип: Reference"), "ChangeType.Reference not logged.");
        }

       

        [TestMethod]
        public void ReadLogFile_FileDoesNotExist_ReturnsNotFoundMessage()
        {
            // Arrange
            string nonExistentFileName = $"non_existent_log_{Guid.NewGuid()}.log";
            var journal = new Journal(nonExistentFileName); // Конструктор создаст его
            string actualPath = journal.GetActualLogFilePath();
            if (File.Exists(actualPath)) File.Delete(actualPath); // Теперь удаляем его
            Assert.IsFalse(File.Exists(actualPath), "Pre-condition: Log file should not exist for this test.");

            // Act
            string result = journal.ReadLogFile();

            // Assert
            Assert.IsTrue(result.Contains("не найден"), "Message for non-existent file is incorrect.");
        }

        // Тестирование LogToFile и ReadLogFile при _logFilePath = null сложно,
        // так как конструктор всегда пытается его инициализировать.
        // Можно было бы использовать рефлексию для установки _logFilePath в null, но это плохая практика для тестов.
        // Вместо этого, можно проверить поведение, если конструктор не смог создать файл (например, из-за прав доступа),
        // но это выходит за рамки стандартных юнит-тестов и требует настройки окружения.
        // Конструктор имеет fallback, если Path.Combine не сработает, но это редкий случай.
    }
}