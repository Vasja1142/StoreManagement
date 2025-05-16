// Файл: StoreManagement.Collections/MyCollection.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StoreManagement.Domain; // Для Goods и его наследников

namespace StoreManagement.Collections
{
    /// <summary>
    /// Обобщенная коллекция на основе HashTable.
    /// Ключом выступает свойство Name элемента типа T.
    /// Реализует IEnumerable<T> и ICollection.
    /// </summary>
    /// <typeparam name="T">Тип элементов в коллекции, должен быть наследником Goods.</typeparam>
    public class MyCollection<T> : IEnumerable<T>, ICollection where T : Goods
    {
        public readonly HashTable<string, T> _dataTable; // Используем HashTable
        private readonly object _syncRoot = new object();

        public int Count => _dataTable.Count;
        public bool IsSynchronized => false;
        public object SyncRoot => _syncRoot;

        public MyCollection()
        {
            _dataTable = new HashTable<string, T>();
        }

        public MyCollection(int capacity)
        {
            _dataTable = new HashTable<string, T>(capacity);
        }

        public MyCollection(MyCollection<T> collection) : this(collection?.Count ?? 0)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            foreach (T item in collection) // collection уже IEnumerable<T>
            {
                this.Add(item); // Наш Add, который использует item.Name как ключ
            }
        }

        public MyCollection(IEnumerable<T> collection) : this()
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            foreach (T item in collection)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Добавляет элемент в коллекцию. Если элемент с таким Name уже существует, его значение обновляется.
        /// </summary>
        /// <param name="item">Элемент для добавления. Не может быть null.</param>
        /// <returns>True, если был добавлен новый элемент; false, если существующий элемент был обновлен.</returns>
        public virtual bool Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Элемент не может быть null.");
            if (string.IsNullOrEmpty(item.Name)) throw new ArgumentException("Имя элемента (ключ) не может быть пустым или null.", nameof(item));

            int oldCount = _dataTable.Count;
            _dataTable.Add(item.Name, item); // HashTable.Add обновляет значение, если ключ есть, и инкрементирует _count только для новых
            return _dataTable.Count > oldCount;
        }

        /// <summary>
        /// Удаляет элемент (по его Name) из коллекции.
        /// </summary>
        /// <param name="item">Элемент для удаления. Не может быть null.</param>
        /// <returns>True, если элемент найден и удален, иначе false.</returns>
        public virtual bool Remove(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Элемент для удаления не может быть null.");
            if (string.IsNullOrEmpty(item.Name)) return false; // Нельзя удалить элемент без имени
            return _dataTable.Remove(item.Name);
        }

        /// <summary>
        /// Удаляет элемент по указанному "индексу" (порядку в текущем перечислении значений).
        /// Неэффективно для хеш-таблицы.
        /// </summary>
        /// <returns>True, если элемент успешно удален.</returns>
        public virtual bool RemoveAt(int index)
        {
            if (index < 0 || index >= _dataTable.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс находится вне диапазона коллекции.");
            }

            T? itemToRemove = default(T);
            int currentIndex = 0;
            // _dataTable.Values возвращает IEnumerable<TValue>
            foreach (T currentItem in _dataTable.Values)
            {
                if (currentIndex == index)
                {
                    itemToRemove = currentItem;
                    break;
                }
                currentIndex++;
            }

            if (itemToRemove != null && itemToRemove.Name != null) // itemToRemove.Name не должен быть null, т.к. T : Goods
            {
                return _dataTable.Remove(itemToRemove.Name);
            }
            return false;
        }

        /// <summary>
        /// Проверяет, содержится ли элемент (по его Name) в коллекции.
        /// </summary>
        public bool Contains(T item)
        {
            if (item == null || string.IsNullOrEmpty(item.Name)) return false;
            return _dataTable.ContainsKey(item.Name);
        }

        public virtual void Clear()
        {
            _dataTable.Clear();
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (array.Rank != 1) throw new ArgumentException("Массив должен быть одномерным.", nameof(array));
            if (array.Length - index < _dataTable.Count) throw new ArgumentException("Недостаточно места в целевом массиве.");

            int i = index;
            foreach (T item in _dataTable.Values)
            {
                try
                {
                    array.SetValue(item, i++);
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException("Тип целевого массива несовместим с типом элементов коллекции.", nameof(array));
                }
            }
        }

        public MyCollection<T> ShallowCopy()
        {
            return new MyCollection<T>(this);
        }

        public MyCollection<T> Clone()
        {
            if (!(typeof(ICloneable).IsAssignableFrom(typeof(T))))
            {
                if (typeof(T).IsValueType) // Маловероятно для T : Goods
                {
                    Console.WriteLine($"Предупреждение: Тип {typeof(T).Name} является структурой. Выполняется поверхностное копирование.");
                    return ShallowCopy();
                }
                else
                {
                    throw new InvalidOperationException($"Тип {typeof(T).Name} не реализует интерфейс ICloneable, глубокое клонирование невозможно.");
                }
            }

            MyCollection<T> newCollection = new MyCollection<T>(_dataTable.Capacity);
            foreach (T item in this._dataTable.Values)
            {
                // Goods не может быть null по логике добавления, но item из Values может быть default если T - структура (не наш случай)
                ICloneable cloneableItem = (ICloneable)item; // T : Goods, Goods : ICloneable
                object? clonedObject = cloneableItem.Clone();
                if (clonedObject is T clonedItem)
                {
                    newCollection.Add(clonedItem);
                }
                else
                {
                    throw new InvalidOperationException($"Метод Clone() для типа {item.GetType().Name} вернул несовместимый тип.");
                }
            }
            return newCollection;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dataTable.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _dataTable.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Индекс находится вне диапазона коллекции.");
                }
                // Неэффективно, но сохраняет API.
                return _dataTable.Values.ElementAt(index);
            }
            set
            {
                // Сеттер в базовом MyCollection не генерирует событий и не предназначен для прямой замены.
                // MyNewCollection будет отвечать за логику замены и генерацию событий.
                throw new NotSupportedException("Установка значения по индексу в базовом MyCollection не поддерживается. Используйте MyNewCollection или специфичные методы, если они будут добавлены.");
            }
        }

        /// <summary>
        /// Внутренний метод для замены элемента по индексу. Используется MyNewCollection.
        /// Находит элемент по индексу, удаляет его по старому ключу (Name) и добавляет новый элемент.
        /// </summary>
        /// <returns>True, если замена прошла успешно. oldItemReplaced содержит замененный элемент.</returns>
        public virtual bool InternalTryReplaceAt(int index, T newItem, out T? oldItemReplaced)
        {
            if (newItem == null) throw new ArgumentNullException(nameof(newItem));
            if (string.IsNullOrEmpty(newItem.Name)) throw new ArgumentException("Имя нового элемента (ключ) не может быть пустым.", nameof(newItem));

            if (index < 0 || index >= _dataTable.Count)
            {
                oldItemReplaced = default;
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            oldItemReplaced = this[index]; // Получаем старый элемент через геттер индексатора

            if (oldItemReplaced == null || string.IsNullOrEmpty(oldItemReplaced.Name))
            {
                // Этого не должно произойти, если геттер и коллекция консистентны
                return false;
            }

            // Если имя (ключ) не изменилось, и это тот же самый объект (по ссылке),
            // то можно просто обновить значение в хеш-таблице, если бы мы хранили что-то кроме самого объекта.
            // Но мы храним сам объект T. Если поля объекта T изменились, а ключ (Name) нет,
            // то HashTable.Replace(oldItemReplaced.Name, newItem) заменит ссылку.
            // Если ключ (Name) изменился, то это удаление старого и добавление нового.

            bool removed = false;
            if (!oldItemReplaced.Name.Equals(newItem.Name)) // Если ключ изменился
            {
                removed = _dataTable.Remove(oldItemReplaced.Name); // Удаляем по старому ключу
                if (!removed)
                {
                    // Старый элемент не найден по ключу, хотя мы его получили по индексу. Проблема консистентности.
                    // Для отладки можно вывести:
                    // Console.WriteLine($"[InternalTryReplaceAt] Ошибка: не удалось удалить старый элемент '{oldItemReplaced.Name}' по ключу.");
                    // _dataTable.PrintDebug();
                    // Тем не менее, продолжим добавлять новый.
                }
                _dataTable.Add(newItem.Name, newItem); // Добавляем новый (или обновляем, если такой ключ уже был от другого элемента)
            }
            else // Ключ не изменился
            {
                // Просто заменяем значение по ключу. HashTable.Replace не меняет _count.
                removed = _dataTable.Replace(newItem.Name, newItem);
            }
            // Успех, если удалось либо удалить старый (если ключ менялся), либо заменить (если ключ тот же)
            return removed || oldItemReplaced.Name.Equals(newItem.Name);
        }


        public void Print(string title = "Содержимое коллекции")
        {
            Console.WriteLine($"\n{title} (Count: {Count}):");
            if (Count == 0)
            {
                Console.WriteLine("Коллекция пуста.");
                return;
            }
            int i = 0;
            foreach (var item in this)
            {
                Console.WriteLine($"  [{i++}]: {item?.ToString() ?? "NULL_ITEM"}");
            }
            Console.WriteLine("--------------------");
        }
    }
}