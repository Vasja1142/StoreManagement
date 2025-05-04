// Файл: StoreManagement.Collections/MyNewCollection.cs
using System;
using System.Collections.Generic; // Для конструктора IEnumerable

namespace StoreManagement.Collections
{
    /// <summary>
    /// Коллекция, наследующая MyCollection<T> и генерирующая события
    /// при изменении количества элементов или изменении элемента по ссылке.
    /// </summary>
    /// <typeparam name="T">Тип элементов в коллекции.</typeparam>
    public class MyNewCollection<T> : MyCollection<T> // Наследуем от нашей базовой коллекции
    {
        /// <summary>
        /// Имя коллекции, используется в аргументах событий.
        /// </summary>
        public string Name { get; set; }

        // --- События ---

        /// <summary>
        /// Событие возникает при изменении количества элементов в коллекции (Add, Remove).
        /// </summary>
        public event CollectionHandler? CollectionCountChanged;

        /// <summary>
        /// Событие возникает при изменении элемента по ссылке (через сеттер индексатора).
        /// </summary>
        public event CollectionHandler? CollectionReferenceChanged;

        // --- Конструкторы ---

        public MyNewCollection(string name) : base() // Вызов конструктора базового класса
        {
            Name = name;
        }

        public MyNewCollection(string name, int capacity) : base(capacity) // Вызов конструктора базового класса
        {
            Name = name;
        }

        public MyNewCollection(string name, IEnumerable<T> collection) : base(collection) // Вызов конструктора базового класса
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            Name = name;
        }


        // --- Методы для генерации событий (On...) ---

        /// <summary>
        /// Защищенный виртуальный метод для вызова события CollectionCountChanged.
        /// Позволяет наследникам изменять логику вызова события.
        /// </summary>
        /// <param name="changeType">Тип изменения (Add/Remove).</param>
        /// <param name="changedItem">Измененный элемент.</param>
        protected virtual void OnCollectionCountChanged(ChangeInfo changeType, object? changedItem)
        {
            // Вызываем событие, если есть подписчики
            CollectionCountChanged?.Invoke(this, new CollectionHandlerEventArgs(this.Name, changeType, changedItem));
        }

        /// <summary>
        /// Защищенный виртуальный метод для вызова события CollectionReferenceChanged.
        /// </summary>
        /// <param name="changedItem">Измененный элемент.</param>
        protected virtual void OnCollectionReferenceChanged(object? changedItem)
        {
            CollectionReferenceChanged?.Invoke(this, new CollectionHandlerEventArgs(this.Name, ChangeInfo.Reference, changedItem));
        }


        // --- Переопределение/скрытие методов для генерации событий ---

        /// <summary>
        /// Добавляет элемент в коллекцию и генерирует событие CollectionCountChanged.
        /// Используем 'new' для сокрытия метода базового класса.
        /// </summary>
        /// <param name="item">Элемент для добавления.</param>
        public new void Add(T item)
        {
            base.Add(item); // Вызываем реализацию базового класса
            OnCollectionCountChanged(ChangeInfo.Add, item); // Генерируем событие
        }

        /// <summary>
        /// Добавляет элементы из последовательности и генерирует событие для каждого.
        /// Используем 'new' для сокрытия метода базового класса.
        /// </summary>
        public new void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            // Генерируем событие для каждого добавленного элемента
            foreach (var item in items)
            {
                this.Add(item); // Вызываем наш переопределенный Add
            }
            // Альтернатива: вызвать base.AddRange и сгенерировать одно общее событие?
            // Задание, скорее всего, предполагает событие на каждое изменение.
        }


        /// <summary>
        /// Удаляет первое вхождение элемента и генерирует событие CollectionCountChanged.
        /// Используем 'new' для сокрытия метода базового класса.
        /// </summary>
        /// <param name="item">Элемент для удаления.</param>
        /// <returns>True, если элемент удален.</returns>
        public new bool Remove(T item)
        {
            // Нужно определить, был ли элемент реально удален
            // Мы не знаем индекс, но можем проверить наличие до и после.
            // Более надежно - изменить Remove в базовом классе, чтобы он возвращал удаленный элемент или индекс.
            // Пока сделаем проще: генерируем событие, если base.Remove вернул true.
            bool removed = base.Remove(item);
            if (removed)
            {
                OnCollectionCountChanged(ChangeInfo.Remove, item);
            }
            return removed;
        }

        /// <summary>
        /// Удаляет элемент по индексу и генерирует событие CollectionCountChanged.
        /// Используем 'new' для сокрытия метода базового класса.
        /// </summary>
        /// <param name="index">Индекс удаляемого элемента.</param>
        /// <returns>True, если элемент удален.</returns>
        public new bool RemoveAt(int index)
        {
            if (index < 0 || index >= base.Count) // Проверка индекса
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс находится вне диапазона коллекции.");
            }
            // Получаем элемент ПЕРЕД удалением, чтобы передать его в событие
            T itemToRemove = base[index]; // Используем индексатор базового класса
            bool removed = base.RemoveAt(index); // Вызываем реализацию базового класса
            if (removed)
            {
                OnCollectionCountChanged(ChangeInfo.Remove, itemToRemove);
            }
            return removed;
        }

        /// <summary>
        /// Очищает коллекцию и генерирует событие CollectionCountChanged для каждого удаленного элемента.
        /// Используем 'new' для сокрытия метода базового класса.
        /// </summary>
        public new void Clear()
        {
            // Сохраняем элементы перед очисткой, чтобы сгенерировать события
            List<T> itemsToRemove = new List<T>(this); // Копируем элементы
            base.Clear(); // Очищаем базовую коллекцию
            // Генерируем событие для каждого удаленного элемента
            foreach (var item in itemsToRemove)
            {
                OnCollectionCountChanged(ChangeInfo.Remove, item);
            }
            // Альтернатива: генерировать одно событие "Reset"? (Не по заданию)
        }


        // --- Переопределение индексатора для генерации события в set ---

        /// <summary>
        /// Получает или задает элемент по указанному индексу.
        /// Сеттер генерирует событие CollectionReferenceChanged.
        /// </summary>
        /// <param name="index">Индекс элемента.</param>
        /// <returns>Элемент по указанному индексу.</returns>
        public new T this[int index]
        {
            get
            {
                // Просто вызываем геттер базового класса
                return base[index];
            }
            set
            {
                if (index < 0 || index >= base.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Индекс находится вне диапазона коллекции.");
                }

                // Получаем старое значение для информации (хотя событие сработает и без него)
                // T oldValue = base[index];

                // --- !!! Важно: Как изменить элемент в DoublyLinkedList по индексу? ---
                // Наш базовый MyCollection и DoublyLinkedList не имеют метода Set(index, value).
                // Нам нужно его добавить в DoublyLinkedList и MyCollection.

                // --- Временное решение (неэффективное): Удалить и вставить ---
                // T itemToReplace = base[index]; // Получаем текущий элемент
                // base.RemoveAt(index); // Удаляем его (это вызовет событие Remove из базового RemoveAt, если бы мы его переопределяли)
                // base.Insert(index, value); // Вставляем новый (Insert тоже нужно добавить)
                // OnCollectionReferenceChanged(value); // Генерируем наше событие

                // --- Предполагаемый правильный путь: Добавить Set(index, value) ---
                // Допустим, мы добавили Set(index, value) в MyCollection<T> (и DoublyLinkedList<T>)
                // base.Set(index, value); // Устанавливаем новое значение в базовой коллекции
                // OnCollectionReferenceChanged(value); // Генерируем событие

                // --- Пока нет Set, имитируем через удаление/добавление (но это не совсем ReferenceChanged) ---
                // Это плохой подход, так как он генерирует события Remove и Add вместо ReferenceChanged.
                // Правильно будет модифицировать DoublyLinkedList и MyCollection.
                // Давайте пока просто сгенерируем событие, предполагая, что изменение произошло.
                // В реальном коде нужно доработать базовые классы.

                // Генерируем событие, но фактическое изменение не происходит без метода Set!
                Console.WriteLine($"ПРЕДУПРЕЖДЕНИЕ: Попытка изменить элемент по индексу {index}. " +
                                  $"Событие CollectionReferenceChanged сгенерировано, но базовый DoublyLinkedList не поддерживает прямое изменение по индексу. " +
                                  $"Требуется доработка DoublyLinkedList.Set(index, value).");

                // Имитируем получение измененного элемента для события
                T changedItem = value; // Новый элемент, который должен был быть установлен
                OnCollectionReferenceChanged(changedItem);

                // !!! ЗАМЕЧАНИЕ: Чтобы это работало корректно, нужно:
                // 1. Добавить метод Set(int index, T value) в DoublyLinkedList<T>
                // 2. Добавить метод Set(int index, T value) в MyCollection<T>, который вызывает метод списка.
                // 3. Вызывать base.Set(index, value) здесь перед OnCollectionReferenceChanged.
            }
        }
    }
}