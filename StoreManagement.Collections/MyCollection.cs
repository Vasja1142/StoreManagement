// Файл: StoreManagement.Collections/MyCollection.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Для ToArray() в CopyTo

namespace StoreManagement.Collections
{
    /// <summary>
    /// Обобщенная коллекция на основе двунаправленного списка.
    /// Реализует IEnumerable<T> и ICollection.
    /// </summary>
    /// <typeparam name="T">Тип элементов в коллекции.</typeparam>
    public class MyCollection<T> : IEnumerable<T>, ICollection // Реализуем и не-обобщенный ICollection
    {
        // Внутреннее хранилище данных
        private readonly DoublyLinkedList<T> _list;

        // Для реализации ICollection
        private readonly object _syncRoot = new object();

        // --- Свойства ---

        /// <summary>
        /// Получает количество элементов в коллекции.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Возвращает false, так как коллекция не является потокобезопасной по умолчанию.
        /// </summary>
        public bool IsSynchronized => false;

        /// <summary>
        /// Возвращает объект, который можно использовать для синхронизации доступа к коллекции.
        /// </summary>
        public object SyncRoot => _syncRoot;

        /// <summary>
        /// Возвращает true, если коллекция доступна только для чтения (в данном случае false).
        /// </summary>
        // Это свойство из ICollection<T>, но не из ICollection.
        // Если бы мы реализовывали ICollection<T>, оно бы выглядело так:
        // public bool IsReadOnly => false;


        // --- Конструкторы ---

        /// <summary>
        /// Создает пустую коллекцию.
        /// </summary>
        public MyCollection()
        {
            _list = new DoublyLinkedList<T>();
        }

        /// <summary>
        /// Создает коллекцию с указанной начальной емкостью (игнорируется для связного списка).
        /// </summary>
        /// <param name="capacity">Начальная емкость (не используется).</param>
        public MyCollection(int capacity) : this() // Вызываем конструктор по умолчанию
        {
            // Для связного списка емкость не имеет прямого смысла,
            // поэтому просто игнорируем параметр.
            // Можно было бы добавить проверку capacity > 0, но это не критично здесь.
        }

        /// <summary>
        /// Создает коллекцию как копию другой коллекции MyCollection<T> (поверхностное копирование элементов).
        /// </summary>
        /// <param name="collection">Коллекция для копирования.</param>
        public MyCollection(MyCollection<T> collection) : this()
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            // Просто добавляем те же самые элементы (ссылки)
            foreach (T item in collection._list)
            {
                _list.Add(item);
            }
        }

        /// <summary>
        /// Создает коллекцию из любой перечислимой последовательности (поверхностное копирование).
        /// </summary>
        /// <param name="collection">Последовательность элементов.</param>
        public MyCollection(IEnumerable<T> collection) : this()
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            foreach (T item in collection)
            {
                _list.Add(item);
            }
        }


        // --- Методы управления коллекцией ---

        /// <summary>
        /// Добавляет элемент в конец коллекции.
        /// </summary>
        /// <param name="item">Элемент для добавления.</param>
        public void Add(T item)
        {
            _list.Add(item);
        }

        /// <summary>
        /// Добавляет элементы из указанной последовательности в конец коллекции.
        /// </summary>
        /// <param name="items">Последовательность элементов для добавления.</param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            foreach (var item in items)
            {
                _list.Add(item);
            }
        }

        /// <summary>
        /// Удаляет первое вхождение указанного элемента из коллекции.
        /// </summary>
        /// <param name="item">Элемент для удаления.</param>
        /// <returns>True, если элемент найден и удален, иначе false.</returns>
        public bool Remove(T item)
        {
            // Используем стандартный метод Remove из DoublyLinkedList,
            // который сравнивает через EqualityComparer<T>.Default.Equals()
            return _list.Remove(item);
        }

        /// <summary>
        /// **(Для ЛР13)** Удаляет элемент по указанному индексу.
        /// Требует итерации по списку.
        /// </summary>
        /// <param name="index">Индекс удаляемого элемента (начиная с 0).</param>
        /// <returns>True, если элемент успешно удален.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Если индекс вне допустимого диапазона.</exception>
        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= _list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс находится вне диапазона коллекции.");
            }

            // Итерация для поиска элемента по индексу
            int currentIndex = 0;
            T? itemToRemove = default; // Используем default, т.к. T может быть struct
            bool found = false;
            foreach (T currentItem in _list) // Используем итератор списка
            {
                if (currentIndex == index)
                {
                    itemToRemove = currentItem;
                    found = true;
                    break;
                }
                currentIndex++;
            }

            if (found)
            {
                // Удаляем найденный элемент (itemToRemove не может быть null если T - класс и найден)
                // Если T - структура, itemToRemove будет иметь значение по умолчанию, но Remove все равно сработает корректно.
                return _list.Remove(itemToRemove!);
            }
            return false; // Не должно произойти при правильной проверке индекса
        }


        /// <summary>
        /// Проверяет, содержится ли указанный элемент в коллекции.
        /// </summary>
        /// <param name="item">Элемент для поиска.</param>
        /// <returns>True, если элемент найден, иначе false.</returns>
        public bool Contains(T item)
        {
            // Можно использовать LINQ .Contains() или перебрать вручную
            foreach (T currentItem in _list)
            {
                if (EqualityComparer<T>.Default.Equals(currentItem, item))
                {
                    return true;
                }
            }
            return false;
            // Альтернатива с LINQ: return _list.Contains(item); (если DoublyLinkedList реализует ICollection<T>)
        }

        /// <summary>
        /// Очищает коллекцию, удаляя все элементы.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Копирует элементы коллекции в существующий одномерный массив, начиная с указанного индекса.
        /// Реализация для интерфейса ICollection.
        /// </summary>
        /// <param name="array">Целевой массив.</param>
        /// <param name="index">Индекс в целевом массиве, с которого начинается копирование.</param>
        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (array.Rank != 1) throw new ArgumentException("Массив должен быть одномерным.", nameof(array));
            if (array.Length - index < _list.Count) throw new ArgumentException("Недостаточно места в целевом массиве.");

            // Копируем элементы
            // Самый простой способ - использовать ToArray() и Array.CopyTo, но это создает промежуточный массив.
            // Скопируем вручную:
            int i = index;
            foreach (T item in _list)
            {
                // Проверка типа не строго обязательна, если array имеет тип object[],
                // но лучше добавить для безопасности, если array может быть типизированным.
                // if (!(array is T[] typedArray) && !(array is object[]))
                // {
                //     throw new ArgumentException("Тип целевого массива несовместим с типом элементов коллекции.", nameof(array));
                // }
                try
                {
                    array.SetValue(item, i++);
                }
                catch (InvalidCastException) // Если тип массива не object и несовместим с T
                {
                    throw new ArgumentException("Тип целевого массива несовместим с типом элементов коллекции.", nameof(array));
                }
            }
        }

        // --- Клонирование ---

        /// <summary>
        /// Создает поверхностную копию коллекции. Новый список содержит ссылки на те же элементы.
        /// </summary>
        /// <returns>Новая коллекция MyCollection<T> с теми же элементами.</returns>
        public MyCollection<T> ShallowCopy()
        {
            // Используем конструктор копирования, который делает поверхностную копию
            return new MyCollection<T>(this);
        }

        /// <summary>
        /// Создает глубокую копию коллекции.
        /// Требует, чтобы тип T реализовывал интерфейс ICloneable.
        /// </summary>
        /// <returns>Новая коллекция MyCollection<T> с клонированными элементами.</returns>
        /// <exception cref="InvalidOperationException">Если тип T не реализует ICloneable.</exception>
        public MyCollection<T> Clone()
        {
            // Проверяем, реализует ли T интерфейс ICloneable
            if (!(typeof(ICloneable).IsAssignableFrom(typeof(T))))
            {
                // Если T - структура, она копируется по значению, что уже является "глубокой" копией
                // в контексте самой структуры (но не ее полей, если они ссылочные).
                // Если T - ссылочный тип, не реализующий ICloneable, глубокое копирование невозможно стандартным способом.
                if (typeof(T).IsValueType)
                {
                    // Для структур поверхностное копирование эквивалентно глубокому (на уровне самой структуры)
                    Console.WriteLine($"Предупреждение: Тип {typeof(T).Name} является структурой. Выполняется поверхностное копирование, эквивалентное глубокому для самой структуры.");
                    return ShallowCopy();
                }
                else
                {
                    throw new InvalidOperationException($"Тип {typeof(T).Name} не реализует интерфейс ICloneable, глубокое клонирование невозможно.");
                }
            }

            MyCollection<T> newCollection = new MyCollection<T>();
            foreach (T item in this._list)
            {
                if (item == null)
                {
                    newCollection.Add(default(T)!); // Добавляем null или default для структуры
                }
                else
                {
                    // Вызываем метод Clone() через интерфейс ICloneable
                    ICloneable? cloneableItem = item as ICloneable;
                    if (cloneableItem != null)
                    {
                        // Приведение результата Clone() обратно к типу T
                        object? clonedObject = cloneableItem.Clone();
                        if (clonedObject is T clonedItem)
                        {
                            newCollection.Add(clonedItem);
                        }
                        else
                        {
                            // Это не должно произойти, если Clone реализован корректно
                            throw new InvalidOperationException($"Метод Clone() для типа {item.GetType().Name} вернул несовместимый тип {clonedObject?.GetType().Name}.");
                        }
                    }
                    else
                    {
                        // Это тоже не должно произойти из-за проверки в начале метода
                        throw new InvalidOperationException($"Элемент типа {item.GetType().Name} неожиданно не реализует ICloneable.");
                    }
                }
            }
            return newCollection;
        }


        // --- Реализация IEnumerable<T> ---

        /// <summary>
        /// Возвращает перечислитель, который осуществляет итерацию по коллекции.
        /// </summary>
        /// <returns>Перечислитель IEnumerator<T>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            // Просто делегируем вызов итератору внутреннего списка
            return _list.GetEnumerator();
        }

        // --- Реализация IEnumerable ---

        /// <summary>
        /// Возвращает перечислитель, который осуществляет итерацию по коллекции.
        /// </summary>
        /// <returns>Перечислитель IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // --- Индексатор (для ЛР13) ---
        /// <summary>
        /// Получает или задает элемент по указанному индексу.
        /// Требует итерации по списку для доступа.
        /// </summary>
        /// <param name="index">Индекс элемента.</param>
        /// <returns>Элемент по указанному индексу.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Если индекс вне допустимого диапазона.</exception>
        /// <exception cref="NotSupportedException">При попытке установить значение (сеттер не реализован в базовой версии).</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _list.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Индекс находится вне диапазона коллекции.");
                }

                // Итерация для получения элемента по индексу
                int currentIndex = 0;
                foreach (T currentItem in _list)
                {
                    if (currentIndex == index)
                    {
                        return currentItem;
                    }
                    currentIndex++;
                }
                // Сюда не должны дойти при правильной проверке индекса
                throw new InvalidOperationException("Не удалось найти элемент по индексу.");
            }
            // Сеттер добавим в MyNewCollection в ЛР13
            // set
            // {
            //     throw new NotSupportedException("Установка значения по индексу будет реализована в MyNewCollection.");
            // }
        }

        // --- Вспомогательный метод для вывода (необязательно) ---
        public void Print(string title = "Содержимое коллекции")
        {
            Console.WriteLine($"\n{title} (Count: {Count}):");
            if (Count == 0)
            {
                Console.WriteLine("Коллекция пуста.");
                return;
            }
            int i = 0;
            foreach (var item in this) // Используем GetEnumerator()
            {
                Console.WriteLine($"  [{i++}]: {item}");
            }
            Console.WriteLine("--------------------");
        }
    }
}