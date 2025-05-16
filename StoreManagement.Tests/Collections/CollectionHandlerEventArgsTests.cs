// Файл: StoreManagement.Tests/Collections/CollectionHandlerEventArgsTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreManagement.Collections;
using StoreManagement.Domain; // Для примера с Goods в ChangedItem
using System;

namespace StoreManagement.Tests.Collections
{
    [TestClass]
    public class CollectionHandlerEventArgsTests
    {
        [TestMethod]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string collectionName = "TestCollection";
            ChangeInfo changeType = ChangeInfo.Add;
            var changedItem = new Product("TestItem", 10m, "Man", DateTime.Now);

            // Act
            var args = new CollectionHandlerEventArgs(collectionName, changeType, changedItem);

            // Assert
            Assert.AreEqual(collectionName, args.CollectionName, "CollectionName should be initialized.");
            Assert.AreEqual(changeType, args.ChangeType, "ChangeType should be initialized.");
            Assert.AreSame(changedItem, args.ChangedItem, "ChangedItem should be the same instance.");
        }


        [TestMethod]
        public void ToString_WithNullItem_FormatsAsNA()
        {
            // Arrange
            var args = new CollectionHandlerEventArgs("Orphans", ChangeInfo.Remove, null);

            // Act
            string result = args.ToString();

            // Assert
            string expected = "Коллекция: 'Orphans', Тип: Remove, Элемент: [N/A]";
            Assert.AreEqual(expected, result, "ToString format for null item is incorrect.");
        }

        [TestMethod]
        public void ToString_WithLongItemInfo_TruncatesItemInfo()
        {
            // Arrange
            string longName = new string('A', 100); // Длинное имя
            var item = new Product(longName, 1m, "LongMan", DateTime.Now);
            var args = new CollectionHandlerEventArgs("LongItems", ChangeInfo.Reference, item);

            string fullItemInfo = item.ToString();
            string truncatedItemInfo = fullItemInfo.Substring(0, 47) + "...";

            // Act
            string result = args.ToString();

            // Assert
            string expected = $"Коллекция: 'LongItems', Тип: Reference, Элемент: [{truncatedItemInfo}]";
            Assert.AreEqual(expected, result, "ToString should truncate long item info.");
        }
    }
}