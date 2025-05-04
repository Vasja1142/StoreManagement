// Файл: StoreManagement.Collections/CollectionHandlerEventArgs.cs
using System;

namespace StoreManagement.Collections
{
    /// <summary>
    /// Тип изменения, произошедшего в коллекции.
    /// </summary>
    public enum ChangeInfo
    {
        Add,      // Элемент добавлен
        Remove,   // Элемент удален
        Reference // Ссылка на элемент изменена (например, через индексатор)
    }

    /// <summary>
    /// Предоставляет данные для событий изменения коллекции CollectionCountChanged и CollectionReferenceChanged.
    /// </summary>
    public class CollectionHandlerEventArgs : EventArgs
    {
        /// <summary>
        /// Имя коллекции, в которой произошло изменение.
        /// </summary>
        public string CollectionName { get; }

        /// <summary>
        /// Тип изменения, которое произошло.
        /// </summary>
        public ChangeInfo ChangeType { get; }

        /// <summary>
        /// Объект, который был добавлен, удален или изменен.
        /// Может быть null, если информация недоступна или не применима.
        /// </summary>
        public object? ChangedItem { get; } // Используем object для универсальности

        /// <summary>
        /// Конструктор для аргументов события.
        /// </summary>
        /// <param name="collectionName">Имя коллекции.</param>
        /// <param name="changeType">Тип изменения.</param>
        /// <param name="changedItem">Затронутый объект.</param>
        public CollectionHandlerEventArgs(string collectionName, ChangeInfo changeType, object? changedItem)
        {
            CollectionName = collectionName;
            ChangeType = changeType;
            ChangedItem = changedItem;
        }

        /// <summary>
        /// Возвращает строковое представление аргументов события.
        /// </summary>
        public override string ToString()
        {
            string itemInfo = ChangedItem?.ToString() ?? "N/A";
            // Сократим длинные строки для читаемости
            if (itemInfo.Length > 50) itemInfo = itemInfo.Substring(0, 47) + "...";
            return $"Коллекция: '{CollectionName}', Тип: {ChangeType}, Элемент: [{itemInfo}]";
        }
    }
}