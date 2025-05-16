// Файл: StoreManagement.Collections/HashEntry.cs
namespace StoreManagement.Collections
{
    /// <summary>
    /// Состояние ячейки в хеш-таблице с открытой адресацией.
    /// </summary>
    public enum EntryState
    {
        Empty,    // Ячейка никогда не использовалась
        Occupied, // Ячейка занята данными
        Deleted   // Ячейка ранее была занята, но элемент удален (маркер для поиска)
    }

    /// <summary>
    /// Представляет элемент (запись) в хеш-таблице с открытой адресацией.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    public class HashEntry<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public EntryState State { get; set; }

        // Конструктор по умолчанию для инициализации массива
        public HashEntry()
        {
            State = EntryState.Empty;
            // Key и Value будут default (null для ссылочных типов)
        }

        // Конструктор для добавления элемента
        public HashEntry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            State = EntryState.Occupied;
        }
    }
}