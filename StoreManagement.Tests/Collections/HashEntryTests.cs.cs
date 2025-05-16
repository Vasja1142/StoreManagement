// Файл: StoreManagement.Tests/Collections/HashEntryTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreManagement.Collections;

namespace StoreManagement.Tests.Collections
{
    [TestClass]
    public class HashEntryTests
    {
        [TestMethod]
        public void DefaultConstructor_SetsStateToEmpty_AndDefaultKeyValues()
        {
            // Arrange & Act
            var entry = new HashEntry<string, int>();

            // Assert
            Assert.AreEqual(EntryState.Empty, entry.State, "Default state should be Empty.");
            Assert.IsNull(entry.Key, "Default key for string should be null.");
            Assert.AreEqual(0, entry.Value, "Default value for int should be 0.");
        }

        [TestMethod]
        public void ParameterizedConstructor_SetsKeyValueAndStateToOccupied()
        {
            // Arrange
            string key = "myKey";
            int value = 123;

            // Act
            var entry = new HashEntry<string, int>(key, value);

            // Assert
            Assert.AreEqual(key, entry.Key, "Key should be initialized.");
            Assert.AreEqual(value, entry.Value, "Value should be initialized.");
            Assert.AreEqual(EntryState.Occupied, entry.State, "State should be Occupied.");
        }

        [TestMethod]
        public void Properties_CanBeSet()
        {
            // Arrange
            var entry = new HashEntry<string, int>();

            // Act
            entry.Key = "newKey";
            entry.Value = 456;
            entry.State = EntryState.Deleted;

            // Assert
            Assert.AreEqual("newKey", entry.Key, "Key property should be settable.");
            Assert.AreEqual(456, entry.Value, "Value property should be settable.");
            Assert.AreEqual(EntryState.Deleted, entry.State, "State property should be settable.");
        }
    }
}