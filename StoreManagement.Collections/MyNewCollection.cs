// Файл: StoreManagement.Collections/MyNewCollection.cs
using System;
using System.Collections.Generic;
using StoreManagement.Domain; // Для Goods

namespace StoreManagement.Collections
{
    public class MyNewCollection<T> : MyCollection<T> where T : Goods
    {
        public string Name { get; set; }

        public event CollectionHandler? CollectionCountChanged;
        public event CollectionHandler? CollectionReferenceChanged;

        public MyNewCollection(string name) : base()
        {
            Name = name;
        }

        public MyNewCollection(string name, int capacity) : base(capacity)
        {
            Name = name;
        }

        public MyNewCollection(string name, IEnumerable<T> collection) : base(collection)
        {
            Name = name;
        }

        protected virtual void OnCollectionCountChanged(ChangeInfo changeType, object? changedItem)
        {
            CollectionCountChanged?.Invoke(this, new CollectionHandlerEventArgs(this.Name, changeType, changedItem));
        }

        protected virtual void OnCollectionReferenceChanged(object? changedItem)
        {
            CollectionReferenceChanged?.Invoke(this, new CollectionHandlerEventArgs(this.Name, ChangeInfo.Reference, changedItem));
        }

        /// <summary>
        /// Добавляет элемент в коллекцию и генерирует событие CollectionCountChanged, если элемент новый.
        /// </summary>
        public new bool Add(T item) // Возвращает bool для консистентности с базовым
        {
            bool newAdded = base.Add(item); // Вызывает MyCollection.Add
            if (newAdded)
            {
                OnCollectionCountChanged(ChangeInfo.Add, item);
            }
            return newAdded;
        }

        public new void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            foreach (var item in items)
            {
                this.Add(item); // Вызывает наш переопределенный Add
            }
        }

        /// <summary>
        /// Удаляет элемент и генерирует событие CollectionCountChanged, если элемент был удален.
        /// </summary>
        public new bool Remove(T item)
        {
            bool removed = base.Remove(item); // Вызывает MyCollection.Remove
            if (removed)
            {
                OnCollectionCountChanged(ChangeInfo.Remove, item);
            }
            return removed;
        }

        /// <summary>
        /// Удаляет элемент по индексу и генерирует событие CollectionCountChanged.
        /// </summary>
        public new bool RemoveAt(int index)
        {
            if (index < 0 || index >= base.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс находится вне диапазона коллекции.");
            }
            T itemToRemove = base[index]; // Получаем элемент перед удалением

            bool removed = base.RemoveAt(index); // Вызывает MyCollection.RemoveAt
            if (removed)
            {
                OnCollectionCountChanged(ChangeInfo.Remove, itemToRemove);
            }
            return removed;
        }

        /// <summary>
        /// Очищает коллекцию и генерирует событие CollectionCountChanged для каждого удаленного элемента.
        /// </summary>
        public new void Clear()
        {
            if (base.Count > 0)
            {
                List<T> itemsToRemove = new List<T>(this); // Копируем элементы через GetEnumerator
                base.Clear(); // Очищаем базовую коллекцию
                foreach (var item in itemsToRemove)
                {
                    OnCollectionCountChanged(ChangeInfo.Remove, item);
                }
            }
            else
            {
                base.Clear(); // На случай, если базовый Clear имеет какую-то логику, даже если пуст
            }
        }

        /// <summary>
        /// Получает или задает элемент по указанному индексу.
        /// Сеттер генерирует событие CollectionReferenceChanged.
        /// </summary>
        public new T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                if (index < 0 || index >= base.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Индекс находится вне диапазона коллекции.");
                }
                if (value == null) // Добавим проверку на null для нового значения
                {
                    throw new ArgumentNullException(nameof(value), "Нельзя установить null значение через индексатор.");
                }

                // Используем InternalTryReplaceAt из базового класса MyCollection
                // Этот метод не генерирует CollectionCountChanged
                T? oldItem; // Нам не нужен oldItem для текущей реализации события
                if (base.InternalTryReplaceAt(index, value, out oldItem))
                {
                    OnCollectionReferenceChanged(value); // Передаем новый элемент в событие
                }
                else
                {
                    // Если InternalTryReplaceAt вернул false, это может означать проблему.
                    // Например, если newItem был null (мы проверили), или если индекс стал невалидным
                    // между проверкой и вызовом (маловероятно в однопоточном сценарии).
                    // Или если oldItemReplaced.Name был null/пуст внутри InternalTryReplaceAt (не должно быть).
                    throw new InvalidOperationException($"Не удалось заменить элемент по индексу {index}. Возможно, элемент был изменен или удален параллельно, или внутренняя ошибка.");
                }
            }
        }
    }
}