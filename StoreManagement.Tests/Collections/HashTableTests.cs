// StoreManagement.Tests/HashTableTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreManagement.Collections;

public class CaseInsensitiveStringComparer : System.Collections.Generic.IEqualityComparer<string>
{
    public bool Equals(string x, string y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return string.Equals(x, y, System.StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(string obj)
    {
        if (obj == null) return 0;
        return obj.ToLowerInvariant().GetHashCode();
    }
}

namespace StoreManagement.Tests.Collections
{
    [TestClass] // Атрибут, указывающий, что это класс с тестами
    public class HashTableTests
    {
        [TestMethod] // Атрибут, указывающий, что это тестовый метод
        public void Add_NewKey_ShouldIncreaseCount()
        {
            // Arrange (Подготовка)
            var hashTable = new HashTable<string, int>();
            string key = "testKey";
            int value = 100;

            // Act (Действие)
            hashTable.Add(key, value);

            // Assert (Проверка)
            Assert.AreEqual(1, hashTable.Count, "Count should be 1 after adding a new key.");
            Assert.IsTrue(hashTable.ContainsKey(key), "HashTable should contain the added key.");
        }

        [TestMethod]
        public void Add_ExistingKey_ShouldUpdateValueAndNotChangeCount()
        {
            // Arrange
            var hashTable = new HashTable<string, int>();
            string key = "testKey";
            hashTable.Add(key, 100); // Первоначальное добавление
            int initialCount = hashTable.Count;
            int newValue = 200;

            // Act
            hashTable.Add(key, newValue); // Повторное добавление с тем же ключом

            // Assert
            Assert.AreEqual(initialCount, hashTable.Count, "Count should not change when updating an existing key.");
            Assert.IsTrue(hashTable.TryGetValue(key, out int retrievedValue));
            Assert.AreEqual(newValue, retrievedValue, "Value should be updated for the existing key.");
        }

        [TestMethod]
        public void Remove_ExistingKey_ShouldDecreaseCountAndKeyShouldNotExist()
        {
            // Arrange
            var hashTable = new HashTable<string, string>();
            string key = "keyToRemove";
            hashTable.Add(key, "value");
            int initialCount = hashTable.Count;

            // Act
            bool removed = hashTable.Remove(key);

            // Assert
            Assert.IsTrue(removed, "Remove should return true for an existing key.");
            Assert.AreEqual(initialCount - 1, hashTable.Count, "Count should decrease by 1.");
            Assert.IsFalse(hashTable.ContainsKey(key), "Key should not exist after removal.");
        }

        [TestMethod]
        public void Remove_NonExistingKey_ShouldReturnFalseAndNotChangeCount()
        {
            // Arrange
            var hashTable = new HashTable<string, string>();
            hashTable.Add("existingKey", "value");
            int initialCount = hashTable.Count;

            // Act
            bool removed = hashTable.Remove("nonExistingKey");

            // Assert
            Assert.IsFalse(removed, "Remove should return false for a non-existing key.");
            Assert.AreEqual(initialCount, hashTable.Count, "Count should not change.");
        }

        [TestMethod]
        public void TryGetValue_ExistingKey_ShouldReturnTrueAndCorrectValue()
        {
            // Arrange
            var hashTable = new HashTable<string, double>();
            string key = "pi";
            double value = 3.14;
            hashTable.Add(key, value);

            // Act
            bool found = hashTable.TryGetValue(key, out double retrievedValue);

            // Assert
            Assert.IsTrue(found, "TryGetValue should return true for an existing key.");
            Assert.AreEqual(value, retrievedValue, 0.001, "Retrieved value should match the added value."); // 0.001 - дельта для double
        }

        [TestMethod]
        public void TryGetValue_NonExistingKey_ShouldReturnFalse()
        {
            // Arrange
            var hashTable = new HashTable<string, double>();

            // Act
            bool found = hashTable.TryGetValue("nonExistingKey", out double retrievedValue);

            // Assert
            Assert.IsFalse(found, "TryGetValue should return false for a non-existing key.");
            // Assert.AreEqual(default(double), retrievedValue); // out параметр будет default
        }

        [TestMethod]
        public void Clear_ShouldResetCountToZeroAndRemoveAllItems()
        {
            // Arrange
            var hashTable = new HashTable<int, string>();
            hashTable.Add(1, "one");
            hashTable.Add(2, "two");

            // Act
            hashTable.Clear();

            // Assert
            Assert.AreEqual(0, hashTable.Count, "Count should be 0 after Clear.");
            Assert.IsFalse(hashTable.ContainsKey(1), "HashTable should not contain any keys after Clear.");
        }

        [TestMethod]
        public void Add_TriggerResize_ShouldIncreaseCapacityAndKeepAllItems()
        {
            // Arrange
            int initialCapacity = 4; // Возьмем маленькую начальную емкость для теста
            var hashTable = new HashTable<int, string>(initialCapacity);
            // MaxLoadFactor = 0.7. initialCapacity * 0.7 = 2.8.
            // Resize должен произойти при попытке добавить 3-й элемент (когда count станет 2, и _count + 1 = 3)

            // Act & Assert - Первый Resize
            hashTable.Add(1, "Item1");
            hashTable.Add(2, "Item2");
            Assert.AreEqual(initialCapacity, hashTable.Capacity, "Capacity should be initial before resize trigger.");

            hashTable.Add(3, "Item3"); // Этот Add должен вызвать Resize

            // Assert - После первого Resize
            Assert.AreEqual(initialCapacity * 2, hashTable.Capacity, "Capacity should double after first resize.");
            Assert.AreEqual(3, hashTable.Count, "Count should be 3 after adding 3 items.");
            Assert.IsTrue(hashTable.ContainsKey(1), "Item1 should exist after resize.");
            Assert.IsTrue(hashTable.ContainsKey(2), "Item2 should exist after resize.");
            Assert.IsTrue(hashTable.ContainsKey(3), "Item3 should exist after resize.");

            // Добавим еще элементы, чтобы вызвать второй Resize
            // Новая емкость = initialCapacity * 2 = 8. 8 * 0.7 = 5.6.
            // Resize при добавлении 6-го элемента (когда count станет 5, и _count + 1 = 6)
            hashTable.Add(4, "Item4");
            hashTable.Add(5, "Item5");
            Assert.AreEqual(initialCapacity * 2, hashTable.Capacity, "Capacity should be 8 before second resize trigger.");

            hashTable.Add(6, "Item6"); // Этот Add должен вызвать второй Resize

            // Assert - После второго Resize
            Assert.AreEqual(initialCapacity * 4, hashTable.Capacity, "Capacity should double again after second resize.");
            Assert.AreEqual(6, hashTable.Count, "Count should be 6 after adding 6 items.");
            Assert.IsTrue(hashTable.ContainsKey(4), "Item4 should exist after second resize.");
            Assert.IsTrue(hashTable.ContainsKey(6), "Item6 should exist after second resize.");
        }

        [TestMethod]
        public void Add_AfterRemovingKey_ShouldReuseOrProbePastDeleted()
        {
            // Arrange
            var hashTable = new HashTable<string, int>(4); // Маленькая емкость для простоты коллизий
                                                           // Добавляем элементы так, чтобы они могли попасть в одну цепочку или рядом
                                                           // Предположим, GetHashCode("A") % 4 = i, GetHashCode("E") % 4 = i (коллизия)
                                                           // Для простоты теста, будем использовать ключи, которые точно дадут коллизию или будут рядом
                                                           // Мы не можем легко контролировать GetHashCode, поэтому сделаем общий тест

            hashTable.Add("key1", 10);
            hashTable.Add("key2", 20); // Пусть key1 и key2 не вызывают коллизий друг с другом
            hashTable.Add("keyToRemove", 30);
            hashTable.Add("key4", 40);

            // Act
            bool removed = hashTable.Remove("keyToRemove");
            Assert.IsTrue(removed, "Key 'keyToRemove' should be removed.");
            Assert.AreEqual(3, hashTable.Count, "Count should be 3 after removal.");

            // Теперь добавляем новый ключ. Он может попасть на место удаленного или должен пройти мимо.
            hashTable.Add("newKeyAfterDelete", 50);

            // Assert
            Assert.AreEqual(4, hashTable.Count, "Count should be 4 after adding new key.");
            Assert.IsTrue(hashTable.ContainsKey("newKeyAfterDelete"), "New key should be added.");
            Assert.IsTrue(hashTable.ContainsKey("key1"), "key1 should still exist.");
            Assert.IsFalse(hashTable.ContainsKey("keyToRemove"), "Removed key should not exist.");

            // Проверим, что поиск другого элемента, цепочка которого могла проходить через удаленный, работает
            Assert.IsTrue(hashTable.ContainsKey("key4"), "key4 should still be findable.");
            hashTable.TryGetValue("key4", out int val4);
            Assert.AreEqual(40, val4);
        }

        [TestMethod]
        public void Replace_ExistingKey_ShouldUpdateValue()
        {
            // Arrange
            var hashTable = new HashTable<string, int>();
            string key = "replaceKey";
            hashTable.Add(key, 100);
            int newValue = 250;

            // Act
            bool replaced = hashTable.Replace(key, newValue);

            // Assert
            Assert.IsTrue(replaced, "Replace should return true for an existing key.");
            Assert.AreEqual(1, hashTable.Count, "Count should remain 1 after Replace.");
            Assert.IsTrue(hashTable.TryGetValue(key, out int retrievedValue));
            Assert.AreEqual(newValue, retrievedValue, "Value should be updated by Replace.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ZeroCapacity_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange & Act
            var hashTable = new HashTable<int, int>(0);
            // Assert - ожидается исключение
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_NegativeCapacity_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange & Act
            var hashTable = new HashTable<int, int>(-5);
            // Assert - ожидается исключение
        }



        [TestMethod]
        public void Replace_NonExistingKey_ShouldReturnFalse()
        {
            // Arrange
            var hashTable = new HashTable<string, int>();
            hashTable.Add("someKey", 10);

            // Act
            bool replaced = hashTable.Replace("nonExistingKeyToReplace", 200);

            // Assert
            Assert.IsFalse(replaced, "Replace should return false for a non-existing key.");
            Assert.AreEqual(1, hashTable.Count); // Count не должен измениться
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))] // Ожидаем это исключение
        public void Add_NullKey_ShouldThrowArgumentNullException()
        {
            // Arrange
            var hashTable = new HashTable<string, int>();

            // Act
            hashTable.Add(null, 10); // Пытаемся добавить null ключ

            // Assert - не нужен, так как ожидается исключение
        }

        // TODO: Добавить тесты на Resize (косвенно проверяется при добавлении многих элементов),
        //       тесты на работу с удаленными ячейками (Deleted state),
        //       тесты на использование кастомного IEqualityComparer.
    }
}

namespace StoreManagement.Tests // Убедитесь, что пространство имен соответствует вашему проекту
{
    public partial class HashTableTests // Используем partial class, если тесты в разных файлах
    {
        [TestMethod]
        public void Values_Property_ReturnsAllOccupiedValues()
        {
            // Arrange
            var hashTable = new HashTable<int, string>();
            hashTable.Add(1, "One");
            hashTable.Add(2, "Two");
            hashTable.Remove(1); // Помечаем как удаленный
            hashTable.Add(3, "Three");

            // Act
            var values = hashTable.Values.ToList();

            // Assert
            Assert.AreEqual(2, values.Count, "Should only return values of occupied entries.");
            CollectionAssert.Contains(values, "Two");
            CollectionAssert.Contains(values, "Three");
            CollectionAssert.DoesNotContain(values, "One");
        }

        [TestMethod]
        public void Values_Property_EmptyTable_ReturnsEmptyEnumeration()
        {
            // Arrange
            var hashTable = new HashTable<int, string>();

            // Act
            var values = hashTable.Values.ToList();

            // Assert
            Assert.AreEqual(0, values.Count, "Values from empty table should be empty.");
        }

        [TestMethod]
        public void Keys_Property_ReturnsAllOccupiedKeys()
        {
            // Arrange
            var hashTable = new HashTable<string, int>();
            hashTable.Add("A", 1);
            hashTable.Add("B", 2);
            hashTable.Remove("A"); // Помечаем как удаленный
            hashTable.Add("C", 3);

            // Act
            var keys = hashTable.Keys.ToList();

            // Assert
            Assert.AreEqual(2, keys.Count, "Should only return keys of occupied entries.");
            CollectionAssert.Contains(keys, "B");
            CollectionAssert.Contains(keys, "C");
            CollectionAssert.DoesNotContain(keys, "A");
        }

        [TestMethod]
        public void Keys_Property_EmptyTable_ReturnsEmptyEnumeration()
        {
            // Arrange
            var hashTable = new HashTable<string, int>();

            // Act
            var keys = hashTable.Keys.ToList();

            // Assert
            Assert.AreEqual(0, keys.Count, "Keys from empty table should be empty.");
        }

        [TestMethod]
        public void GetNextPowerOfTwo_VariousInputs_ChecksCapacityLogic()
        {
            // GetNextPowerOfTwo используется в конструкторе и Resize.
            // Проверяем через конструктор, как он влияет на Capacity.
            Assert.AreEqual(8, new HashTable<int, int>(5).Capacity, "Capacity for input 5 should be 8.");
            Assert.AreEqual(8, new HashTable<int, int>(8).Capacity, "Capacity for input 8 should be 8.");
            Assert.AreEqual(16, new HashTable<int, int>(9).Capacity, "Capacity for input 9 should be 16.");

            // HashTable.DefaultCapacity используется, если GetNextPowerOfTwo вернет что-то <=0 или если вход <=0
            // В коде GetNextPowerOfTwo(0) -> DefaultCapacity, GetNextPowerOfTwo(-10) -> DefaultCapacity
            Assert.AreEqual(HashTable<int, int>.DefaultCapacity, new HashTable<int, int>(0).Capacity, "Capacity for input 0 should be DefaultCapacity.");
            Assert.AreEqual(HashTable<int, int>.DefaultCapacity, new HashTable<int, int>(-10).Capacity, "Capacity for negative input should be DefaultCapacity.");

            var ht1 = new HashTable<int, int>(1); // GetNextPowerOfTwo(1) -> 1
            Assert.AreEqual(1, ht1.Capacity, "Capacity for input 1 should be 1.");
            ht1.Add(100, 100); // Должен вызвать Resize, т.к. (0+1)/1 >= MaxLoadFactor
            Assert.AreEqual(2, ht1.Capacity, "Capacity should double after first add to size 1 table.");

            // Проверка на граничное значение int.MaxValue (может быть специфично для реализации GetNextPowerOfTwo)
            // Если GetNextPowerOfTwo(int.MaxValue - 1) переполняется и возвращает <=0, то будет DefaultCapacity
            // Если он корректно возвращает int.MaxValue (как степень двойки), то так и будет.
            // В текущей реализации: power <<= 1; if (power <= 0) return int.MaxValue;
            Assert.AreEqual(int.MaxValue, new HashTable<int, int>(int.MaxValue / 2 + 1).Capacity, "Capacity near int.MaxValue.");
        }

        [TestMethod]
        public void HashTable_WithCustomComparer_BehavesCorrectly()
        {
            // Arrange
            var comparer = new CaseInsensitiveStringComparer();
            var hashTable = new HashTable<string, int>(comparer: comparer);

            // Act
            hashTable.Add("KeyOne", 1);
            hashTable.Add("keyone", 11); // Должен обновить, т.к. компаратор игнорирует регистр
            hashTable.Add("KeyTwo", 2);

            // Assert
            Assert.AreEqual(2, hashTable.Count, "Count should be 2 due to case-insensitive updates.");
            Assert.IsTrue(hashTable.ContainsKey("KEYONE"), "ContainsKey should be case-insensitive with custom comparer.");
            Assert.IsTrue(hashTable.ContainsKey("keytwo"), "ContainsKey should be case-insensitive.");

            hashTable.TryGetValue("kEyOnE", out int val1);
            Assert.AreEqual(11, val1, "Value for 'KeyOne' (case-insensitive) should be the updated one.");

            bool removed = hashTable.Remove("KEYTWO"); // Удаление также должно быть case-insensitive
            Assert.IsTrue(removed, "Remove should be case-insensitive.");
            Assert.AreEqual(1, hashTable.Count, "Count should decrease after case-insensitive remove.");
            Assert.IsFalse(hashTable.ContainsKey("KeyTwo"), "KeyTwo should be removed.");
        }
    }
}