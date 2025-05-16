// Файл: StoreManagement.Tests/Services/JournalEntryTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreManagement.Services;
using StoreManagement.Collections; // Для ChangeInfo и CollectionHandlerEventArgs
using StoreManagement.Domain;     // Для Product
using System;

namespace StoreManagement.Tests.Services
{
    [TestClass]
    public class JournalEntryTests
    {
        [TestMethod]
        public void Constructor_WithDirectParameters_InitializesPropertiesCorrectly()
        {
            // Arrange
            string collectionName = "LogCollection";
            ChangeInfo changeType = ChangeInfo.Add;
            var item = new Product("LoggedItem", 1m, "LogMan", DateTime.Now);
            string itemString = item.ToString();
            DateTime beforeCreation = DateTime.Now;

            // Act
            var entry = new JournalEntry(collectionName, changeType, item);
            DateTime afterCreation = DateTime.Now;

            // Assert
            Assert.AreEqual(collectionName, entry.CollectionName);
            Assert.AreEqual(changeType, entry.ChangeType);
            Assert.AreEqual(itemString, entry.ChangedItemInfo);
            Assert.IsTrue(entry.Timestamp >= beforeCreation && entry.Timestamp <= afterCreation, "Timestamp should be set to current time.");
        }

        [TestMethod]
        public void Constructor_WithEventArgs_InitializesPropertiesCorrectly()
        {
            // Arrange
            var eventArgs = new CollectionHandlerEventArgs(
                "EventArgsCollection",
                ChangeInfo.Remove,
                new Product("EventItem", 2m, "EventMan", DateTime.Now.AddDays(1))
            );
            string expectedItemInfo = eventArgs.ChangedItem.ToString();
            DateTime beforeCreation = DateTime.Now;

            // Act
            var entry = new JournalEntry(eventArgs);
            DateTime afterCreation = DateTime.Now;

            // Assert
            Assert.AreEqual(eventArgs.CollectionName, entry.CollectionName);
            Assert.AreEqual(eventArgs.ChangeType, entry.ChangeType);
            Assert.AreEqual(expectedItemInfo, entry.ChangedItemInfo);
            Assert.IsTrue(entry.Timestamp >= beforeCreation && entry.Timestamp <= afterCreation);
        }

        [TestMethod]
        public void ToString_FormatsEntryCorrectly()
        {
            // Arrange
            var item = new Product("ToStringTestItem", 5m, "TestFac", DateTime.Now.AddDays(3));
            var entry = new JournalEntry("TestColl", ChangeInfo.Reference, item);
            string itemInfo = item.ToString();
            if (itemInfo.Length > 50) itemInfo = itemInfo.Substring(0, 47) + "...";

            string expected = $"{entry.Timestamp:HH:mm:ss.fff} | Коллекция: 'TestColl', Тип: Reference, Элемент: [{itemInfo}]";

            // Act
            string actual = entry.ToString();

            // Assert
            Assert.AreEqual(expected, actual, "ToString output is not as expected.");
        }

        [TestMethod]
        public void ToString_WithNullItemInEventArgs_FormatsAsNA()
        {
            // Arrange
            var eventArgs = new CollectionHandlerEventArgs("NullItemColl", ChangeInfo.Add, null);
            var entry = new JournalEntry(eventArgs);

            string expected = $"{entry.Timestamp:HH:mm:ss.fff} | Коллекция: 'NullItemColl', Тип: Add, Элемент: [N/A]";

            // Act
            string actual = entry.ToString();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToString_WithLongItemInfo_TruncatesItemInfo()
        {
            // Arrange
            string longName = new string('X', 70);
            var item = new Product(longName, 1m, "LongFac", DateTime.Now);
            var entry = new JournalEntry("Longs", ChangeInfo.Add, item);

            string originalItemInfo = item.ToString();
            string truncatedItemInfo = originalItemInfo.Substring(0, 47) + "...";
            string expected = $"{entry.Timestamp:HH:mm:ss.fff} | Коллекция: 'Longs', Тип: Add, Элемент: [{truncatedItemInfo}]";

            // Act
            string actual = entry.ToString();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}