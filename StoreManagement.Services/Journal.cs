// Файл: StoreManagement.Services/Journal.cs
using System;
using System.Collections.Generic;
using StoreManagement.Collections; // Для CollectionHandlerEventArgs

namespace StoreManagement.Services
{
    /// <summary>
    /// Класс для ведения журнала изменений в коллекциях.
    /// Хранит записи JournalEntry и предоставляет обработчики событий.
    /// </summary>
    public class Journal
    {
        private readonly List<JournalEntry> _entries = new List<JournalEntry>();

        /// <summary>
        /// Обработчик события изменения количества элементов в коллекции (Add/Remove).
        /// </summary>
        /// <param name="source">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        public void CollectionCountChangedHandler(object source, CollectionHandlerEventArgs args)
        {
            // Создаем новую запись на основе аргументов события
            JournalEntry entry = new JournalEntry(args);
            _entries.Add(entry);
            // Можно добавить вывод в консоль для немедленной реакции
            // Console.WriteLine($"[Журнал {this.GetHashCode()}]: Зафиксировано событие CountChanged -> {entry}");
        }

        /// <summary>
        /// Обработчик события изменения ссылки на элемент (через индексатор).
        /// </summary>
        /// <param name="source">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        public void CollectionReferenceChangedHandler(object source, CollectionHandlerEventArgs args)
        {
            // Создаем новую запись на основе аргументов события
            JournalEntry entry = new JournalEntry(args);
            _entries.Add(entry);
            // Можно добавить вывод в консоль для немедленной реакции
            // Console.WriteLine($"[Журнал {this.GetHashCode()}]: Зафиксировано событие ReferenceChanged -> {entry}");
        }

        /// <summary>
        /// Выводит содержимое журнала в консоль.
        /// </summary>
        /// <param name="title">Заголовок для вывода.</param>
        public void PrintJournal(string title = "Содержимое журнала")
        {
            Console.WriteLine($"\n--- {title} (Записей: {_entries.Count}) ---");
            if (_entries.Count == 0)
            {
                Console.WriteLine("Журнал пуст.");
            }
            else
            {
                foreach (var entry in _entries)
                {
                    Console.WriteLine(entry.ToString());
                }
            }
            Console.WriteLine("---------------------------------------");
        }
    }
}