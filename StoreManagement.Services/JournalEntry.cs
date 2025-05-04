// Файл: StoreManagement.Services/JournalEntry.cs
using System;
using StoreManagement.Collections; // Для доступа к CollectionHandlerEventArgs и ChangeInfo

namespace StoreManagement.Services
{
    /// <summary>
    /// Представляет одну запись в журнале изменений коллекции.
    /// </summary>
    public class JournalEntry
    {
        /// <summary>
        /// Имя коллекции, где произошло событие.
        /// </summary>
        public string CollectionName { get; }

        /// <summary>
        /// Тип изменения.
        /// </summary>
        public ChangeInfo ChangeType { get; }

        /// <summary>
        /// Строковое представление объекта, с которым связано изменение.
        /// </summary>
        public string ChangedItemInfo { get; }

        /// <summary>
        /// Время события.
        /// </summary>
        public DateTime Timestamp { get; }

        public JournalEntry(string collectionName, ChangeInfo changeType, object? changedItem)
        {
            CollectionName = collectionName;
            ChangeType = changeType;
            // Сохраняем строковое представление, а не сам объект,
            // чтобы избежать проблем с жизненным циклом объекта.
            ChangedItemInfo = changedItem?.ToString() ?? "N/A";
            Timestamp = DateTime.Now;
        }

        public JournalEntry(CollectionHandlerEventArgs args)
            : this(args.CollectionName, args.ChangeType, args.ChangedItem)
        {
        }

        /// <summary>
        /// Возвращает строковое представление записи журнала.
        /// </summary>
        public override string ToString()
        {
            string itemInfo = ChangedItemInfo;
            // Сократим длинные строки для читаемости
            if (itemInfo.Length > 50) itemInfo = itemInfo.Substring(0, 47) + "...";
            return $"{Timestamp:HH:mm:ss.fff} | Коллекция: '{CollectionName}', Тип: {ChangeType}, Элемент: [{itemInfo}]";
        }
    }
}