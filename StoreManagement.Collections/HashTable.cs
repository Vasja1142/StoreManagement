// Файл: StoreManagement.Collections/HashTable.cs
using System;
using System.Collections.Generic; // Для IEqualityComparer

namespace StoreManagement.Collections
{
    // Состояние ячейки и HashEntry остаются без изменений

    /// <summary>
    /// Реализация хеш-таблицы с открытой адресацией (линейное пробирование).
    /// </summary>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    public class HashTable<TKey, TValue> where TKey : notnull
    {
        private HashEntry<TKey, TValue>[] _table;
        private int _count;
        private int _capacity;
        private readonly IEqualityComparer<TKey> _comparer;
        private const double MaxLoadFactor = 0.7;
        public const int DefaultCapacity = 8;

        public int Count => _count;
        public int Capacity => _capacity;

        public IEnumerable<TValue> Values // Свойство для доступа к значениям
        {
            get
            {
                for (int i = 0; i < _capacity; i++)
                {
                    if (_table[i].State == EntryState.Occupied)
                    {
                        yield return _table[i].Value;
                    }
                }
            }
        }

        public IEnumerable<TKey> Keys // Свойство для доступа к ключам (может понадобиться)
        {
            get
            {
                for (int i = 0; i < _capacity; i++)
                {
                    if (_table[i].State == EntryState.Occupied)
                    {
                        yield return _table[i].Key;
                    }
                }
            }
        }


        public HashTable(int capacity = DefaultCapacity, IEqualityComparer<TKey>? comparer = null)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Емкость должна быть положительной.");
            _capacity = GetNextPowerOfTwo(capacity);
            _table = new HashEntry<TKey, TValue>[_capacity];
            for (int i = 0; i < _capacity; i++)
            {
                _table[i] = new HashEntry<TKey, TValue>();
            }
            _count = 0;
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        /// <summary>
        /// Добавляет элемент с указанным ключом и значением.
        /// Если ключ уже существует, обновляет значение. _count увеличивается только при добавлении нового элемента.
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if ((double)(_count + 1) / _capacity >= MaxLoadFactor)
            {
                Resize(_capacity * 2);
            }

            int index = FindIndexForKey(key, out bool keyExists);

            if (keyExists)
            {
                _table[index].Value = value; // Обновляем значение, _count не меняется
            }
            else
            {
                _table[index].Key = key;
                _table[index].Value = value;
                _table[index].State = EntryState.Occupied;
                _count++; // _count меняется только при добавлении нового
            }
        }

        /// <summary>
        /// Заменяет значение для существующего ключа. Не изменяет Count.
        /// </summary>
        /// <returns>True, если ключ найден и значение заменено, иначе false.</returns>
        public bool Replace(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            int index = FindIndexForKey(key, out bool keyExists);
            if (keyExists)
            {
                _table[index].Value = value;
                return true;
            }
            return false;
        }


        public bool TryGetValue(TKey key, out TValue? value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            int index = FindIndexForKey(key, out bool keyExists);
            if (keyExists)
            {
                value = _table[index].Value;
                return true;
            }
            value = default;
            return false;
        }

        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            int index = FindIndexForKey(key, out bool keyExists);
            if (keyExists)
            {
                _table[index].State = EntryState.Deleted;
                _table[index].Key = default!; // Освобождаем ссылку на ключ
                _table[index].Value = default!; // Освобождаем ссылку на значение
                _count--;
                return true;
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            FindIndexForKey(key, out bool keyExists);
            return keyExists;
        }

        public void Clear()
        {
            _capacity = DefaultCapacity;
            _table = new HashEntry<TKey, TValue>[_capacity];
            for (int i = 0; i < _capacity; i++)
            {
                _table[i] = new HashEntry<TKey, TValue>();
            }
            _count = 0;
        }

        private int FindIndexForKey(TKey key, out bool keyExists)
        {
            int hash = Math.Abs(_comparer.GetHashCode(key!));
            int initialIndex = hash % _capacity;
            int firstDeleted = -1;

            for (int i = 0; i < _capacity; i++)
            {
                int currentIndex = (initialIndex + i) % _capacity;
                HashEntry<TKey, TValue> entry = _table[currentIndex];

                if (entry.State == EntryState.Empty)
                {
                    keyExists = false;
                    return firstDeleted != -1 ? firstDeleted : currentIndex;
                }

                if (entry.State == EntryState.Deleted)
                {
                    if (firstDeleted == -1)
                    {
                        firstDeleted = currentIndex;
                    }
                }
                else if (entry.State == EntryState.Occupied && _comparer.Equals(entry.Key, key))
                {
                    keyExists = true;
                    return currentIndex;
                }
            }
            keyExists = false;
            if (firstDeleted != -1) return firstDeleted;
            throw new InvalidOperationException("Не удалось найти место для ключа. Таблица переполнена удаленными элементами или логическая ошибка.");
        }

        private void Resize(int newCapacity)
        {
            HashEntry<TKey, TValue>[] oldTable = _table;
            int oldCapacity = _capacity;

            _capacity = GetNextPowerOfTwo(newCapacity);
            _table = new HashEntry<TKey, TValue>[_capacity];
            for (int i = 0; i < _capacity; i++)
            {
                _table[i] = new HashEntry<TKey, TValue>();
            }
            _count = 0;

            for (int i = 0; i < oldCapacity; i++)
            {
                if (oldTable[i].State == EntryState.Occupied)
                {
                    Add(oldTable[i].Key, oldTable[i].Value);
                }
            }
            // Console.WriteLine($"--- Хеш-таблица увеличена до {_capacity} ---"); // Отладка
        }

        private int GetNextPowerOfTwo(int n)
        {
            if (n <= 0) return DefaultCapacity;
            int power = 1;
            while (power < n)
            {
                power <<= 1;
                if (power <= 0) return int.MaxValue;
            }
            return power;
        }

        public void PrintDebug()
        {
            Console.WriteLine($"\n--- Содержимое хеш-таблицы (Capacity: {_capacity}, Count: {_count}) ---");
            for (int i = 0; i < _capacity; i++)
            {
                HashEntry<TKey, TValue> entry = _table[i];
                Console.Write($"[{i:D2}]: ");
                switch (entry.State)
                {
                    case EntryState.Empty: Console.WriteLine("<Empty>"); break;
                    case EntryState.Deleted: Console.WriteLine("<Deleted>"); break;
                    case EntryState.Occupied:
                        string keyStr = entry.Key?.ToString() ?? "NULL_KEY";
                        string valStr = entry.Value?.ToString() ?? "NULL_VALUE";
                        Console.WriteLine($"Key='{keyStr}', Value='{valStr}' (Hash={Math.Abs(_comparer.GetHashCode(entry.Key!)) % _capacity})");
                        break;
                }
            }
            Console.WriteLine("----------------------------------------------------");
        }
    }
}