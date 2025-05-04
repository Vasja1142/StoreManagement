// Файл: StoreManagement.Collections/HashTable.cs
using System;
using System.Collections.Generic; // Для IEqualityComparer

namespace StoreManagement.Collections
{
    /// <summary>
    /// Реализация хеш-таблицы с открытой адресацией (линейное пробирование).
    /// </summary>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    public class HashTable<TKey, TValue> where TKey : notnull // Ключ не может быть null
    {
        private HashEntry<TKey, TValue>[] _table;
        private int _count; // Количество занятых элементов
        private int _capacity;
        private readonly IEqualityComparer<TKey> _comparer; // Для сравнения ключей
        private const double MaxLoadFactor = 0.7; // Максимальный коэффициент заполнения перед ресайзом
        private const int DefaultCapacity = 8; // Начальная емкость (лучше степень двойки)

        public int Count => _count;
        public int Capacity => _capacity;

        public HashTable(int capacity = DefaultCapacity, IEqualityComparer<TKey>? comparer = null)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Емкость должна быть положительной.");

            // Желательно, чтобы емкость была степенью двойки для лучшего распределения
            // Найдем ближайшую степень двойки >= capacity
            _capacity = GetNextPowerOfTwo(capacity);
            _table = new HashEntry<TKey, TValue>[_capacity];
            // Инициализируем все ячейки как Empty (хотя конструктор HashEntry это уже делает)
            for (int i = 0; i < _capacity; i++)
            {
                _table[i] = new HashEntry<TKey, TValue>();
            }
            _count = 0;
            _comparer = comparer ?? EqualityComparer<TKey>.Default; // Используем компаратор по умолчанию, если не задан
        }

        // --- Основные операции ---

        /// <summary>
        /// Добавляет элемент с указанным ключом и значением.
        /// Если ключ уже существует, обновляет значение.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="value">Значение.</param>
        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            // Проверяем необходимость увеличения размера перед добавлением
            if ((double)(_count + 1) / _capacity >= MaxLoadFactor)
            {
                Resize(_capacity * 2);
            }

            int index = FindIndexForKey(key, out bool keyExists);

            if (keyExists)
            {
                // Ключ уже существует, обновляем значение
                _table[index].Value = value;
            }
            else
            {
                // Нашли пустую или удаленную ячейку для вставки
                _table[index].Key = key;
                _table[index].Value = value;
                _table[index].State = EntryState.Occupied;
                _count++;
            }
        }

        /// <summary>
        /// Пытается получить значение, связанное с указанным ключом.
        /// </summary>
        /// <param name="key">Ключ для поиска.</param>
        /// <param name="value">Возвращаемое значение, если ключ найден.</param>
        /// <returns>True, если ключ найден, иначе false.</returns>
        public bool TryGetValue(TKey key, out TValue? value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            int index = FindIndexForKey(key, out bool keyExists);

            if (keyExists)
            {
                value = _table[index].Value;
                return true;
            }
            else
            {
                value = default; // null для ссылочных, 0 для числовых и т.д.
                return false;
            }
        }

        /// <summary>
        /// Удаляет элемент с указанным ключом.
        /// </summary>
        /// <param name="key">Ключ элемента для удаления.</param>
        /// <returns>True, если элемент был найден и удален, иначе false.</returns>
        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            int index = FindIndexForKey(key, out bool keyExists);

            if (keyExists)
            {
                // Помечаем ячейку как удаленную, не очищаем полностью
                _table[index].State = EntryState.Deleted;
                // Обнуляем ссылки для сборщика мусора (опционально)
                _table[index].Key = default!;
                _table[index].Value = default!;
                _count--;
                return true;
            }
            else
            {
                // Ключ не найден
                return false;
            }
        }

        /// <summary>
        /// Проверяет, содержится ли указанный ключ в таблице.
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            FindIndexForKey(key, out bool keyExists);
            return keyExists;
        }

        /// <summary>
        /// Очищает хеш-таблицу.
        /// </summary>
        public void Clear()
        {
            // Просто пересоздаем массив
            _capacity = DefaultCapacity;
            _table = new HashEntry<TKey, TValue>[_capacity];
            for (int i = 0; i < _capacity; i++)
            {
                _table[i] = new HashEntry<TKey, TValue>();
            }
            _count = 0;
        }

        // --- Вспомогательные методы ---

        /// <summary>
        /// Находит индекс для ключа (для вставки, поиска или удаления).
        /// Использует линейное пробирование.
        /// </summary>
        /// <param name="key">Ключ для поиска.</param>
        /// <param name="keyExists">Возвращает true, если ключ найден в Occupied ячейке.</param>
        /// <returns>Индекс найденной ячейки (если keyExists=true) или индекс первой подходящей
        /// пустой/удаленной ячейки для вставки (если keyExists=false).</returns>
        private int FindIndexForKey(TKey key, out bool keyExists)
        {
            int hash = Math.Abs(_comparer.GetHashCode(key)); // Берем модуль на случай отрицательного хеша
            int index = hash % _capacity;
            int firstDeleted = -1; // Индекс первой встреченной удаленной ячейки

            for (int i = 0; i < _capacity; i++) // Цикл для предотвращения зацикливания
            {
                int currentIndex = (index + i) % _capacity; // Линейное пробирование
                HashEntry<TKey, TValue> entry = _table[currentIndex];

                if (entry.State == EntryState.Empty)
                {
                    // Нашли пустую ячейку - ключа точно нет
                    keyExists = false;
                    // Если ранее встретили удаленную, возвращаем ее индекс для вставки,
                    // иначе возвращаем индекс текущей пустой ячейки.
                    return firstDeleted != -1 ? firstDeleted : currentIndex;
                }

                if (entry.State == EntryState.Deleted)
                {
                    // Запоминаем индекс первой удаленной ячейки, но продолжаем поиск
                    if (firstDeleted == -1)
                    {
                        firstDeleted = currentIndex;
                    }
                    // Продолжаем цикл, т.к. искомый ключ может быть дальше
                }
                else if (entry.State == EntryState.Occupied && _comparer.Equals(entry.Key, key))
                {
                    // Нашли ячейку с искомым ключом
                    keyExists = true;
                    return currentIndex;
                }
                // Если ячейка Occupied, но ключ не совпадает, или Deleted - продолжаем пробирование
            }

            // Если прошли весь цикл и не вышли (таблица полна удаленными элементами?)
            // Этого не должно происходить при правильном ресайзе, но на всякий случай:
            keyExists = false;
            // Если была удаленная ячейка, вернем ее, иначе - ошибка (или нужно было ресайзить раньше)
            if (firstDeleted != -1) return firstDeleted;

            // Если таблица полностью заполнена Occupied элементами и ключ не найден,
            // или если она заполнена Deleted и Occupied и ключ не найден.
            // При правильной работе ресайза до этого не дойдет.
            // Можно бросить исключение или вернуть -1, но FindIndexForKey используется и для вставки.
            // Вставка должна была вызвать Resize раньше. Поиск просто вернет keyExists=false.
            // Для вставки вернем -1, чтобы сигнализировать о проблеме (хотя ее быть не должно).
            throw new InvalidOperationException("Не удалось найти место для вставки. Таблица переполнена?");
        }


        /// <summary>
        /// Увеличивает размер хеш-таблицы и перехеширует элементы.
        /// </summary>
        private void Resize(int newCapacity)
        {
            // Сохраняем старые данные
            HashEntry<TKey, TValue>[] oldTable = _table;
            int oldCapacity = _capacity;

            // Создаем новую таблицу
            _capacity = GetNextPowerOfTwo(newCapacity); // Убедимся, что новая емкость - степень двойки
            _table = new HashEntry<TKey, TValue>[_capacity];
            for (int i = 0; i < _capacity; i++)
            {
                _table[i] = new HashEntry<TKey, TValue>();
            }
            _count = 0; // Сбросим счетчик, т.к. будем добавлять заново

            // Переносим элементы из старой таблицы в новую
            for (int i = 0; i < oldCapacity; i++)
            {
                if (oldTable[i].State == EntryState.Occupied)
                {
                    // Используем Add для перехеширования в новую таблицу
                    // Add сама найдет правильный индекс и обработает коллизии в новой таблице
                    Add(oldTable[i].Key, oldTable[i].Value);
                }
            }
            Console.WriteLine($"--- Хеш-таблица увеличена до {_capacity} ---"); // Для отладки
        }

        /// <summary>
        /// Находит ближайшую степень двойки, большую или равную n.
        /// </summary>
        private int GetNextPowerOfTwo(int n)
        {
            if (n <= 0) return DefaultCapacity; // Возвращаем дефолтную, если n некорректно
            int power = 1;
            while (power < n)
            {
                power <<= 1; // power *= 2
                if (power <= 0) return int.MaxValue; // Защита от переполнения
            }
            return power;
        }

        /// <summary>
        /// Выводит содержимое хеш-таблицы (для отладки).
        /// </summary>
        public void PrintDebug()
        {
            Console.WriteLine($"\n--- Содержимое хеш-таблицы (Capacity: {_capacity}, Count: {_count}) ---");
            for (int i = 0; i < _capacity; i++)
            {
                HashEntry<TKey, TValue> entry = _table[i];
                Console.Write($"[{i:D2}]: "); // Индекс с ведущим нулем
                switch (entry.State)
                {
                    case EntryState.Empty:
                        Console.WriteLine("<Empty>");
                        break;
                    case EntryState.Deleted:
                        Console.WriteLine("<Deleted>");
                        break;
                    case EntryState.Occupied:
                        Console.WriteLine($"Key='{entry.Key}', Value='{entry.Value}' (Hash={Math.Abs(_comparer.GetHashCode(entry.Key)) % _capacity})");
                        break;
                }
            }
            Console.WriteLine("----------------------------------------------------");
        }
    }
}