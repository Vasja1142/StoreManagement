// Файл: StoreManagement.Services/Journal.cs
using System;
using System.Collections.Generic;
using System.IO; // Для работы с файлами (StreamWriter)
using System.Text; // Для Encoding
using StoreManagement.Collections; // Для CollectionHandlerEventArgs

namespace StoreManagement.Services
{
    /// <summary>
    /// Класс для ведения журнала изменений в коллекциях.
    /// Хранит записи JournalEntry в памяти и дописывает их в файл journal.log.
    /// </summary>
    public class Journal
    {
        private readonly List<JournalEntry> _entries = new List<JournalEntry>();
        private readonly string _logFilePath; // Путь к файлу журнала

        // --- Конструктор ---
        /// <summary>
        /// Создает экземпляр журнала.
        /// </summary>
        /// <param name="logFileName">Имя файла журнала (по умолчанию journal.log).</param>
        public Journal(string logFileName = "journal.log")
        {
            // Определяем полный путь к файлу в папке выполнения программы
            // Environment.CurrentDirectory может быть не всегда тем, что ожидается (особенно при отладке).
            // Используем AppContext.BaseDirectory для большей надежности.
            string baseDirectory = AppContext.BaseDirectory;
            _logFilePath = Path.Combine(baseDirectory, logFileName);

            // Можно добавить начальную запись в лог при создании журнала
            // LogToFile($"--- Журнал инициализирован: {DateTime.Now} ---");
        }

        // --- Обработчики событий ---

        /// <summary>
        /// Обработчик события изменения количества элементов в коллекции (Add/Remove).
        /// Добавляет запись в память и в файл.
        /// </summary>
        public void CollectionCountChangedHandler(object source, CollectionHandlerEventArgs args)
        {
            JournalEntry entry = new JournalEntry(args);
            _entries.Add(entry);
            LogToFile(entry.ToString()); // Записываем в файл
        }

        /// <summary>
        /// Обработчик события изменения ссылки на элемент (через индексатор).
        /// Добавляет запись в память и в файл.
        /// </summary>
        public void CollectionReferenceChangedHandler(object source, CollectionHandlerEventArgs args)
        {
            JournalEntry entry = new JournalEntry(args);
            _entries.Add(entry);
            LogToFile(entry.ToString()); // Записываем в файл
        }

        // --- Работа с файлом ---

        /// <summary>
        /// Дописывает строку в файл журнала. Обрабатывает возможные ошибки ввода-вывода.
        /// </summary>
        /// <param name="message">Сообщение для записи.</param>
        private void LogToFile(string message)
        {
            try
            {
                // Используем StreamWriter с append: true для дозаписи в конец файла
                // и UTF8Encoding для корректной записи кириллицы.
                // using гарантирует закрытие и освобождение файла (Dispose).
                using (StreamWriter writer = new StreamWriter(_logFilePath, append: true, Encoding.UTF8))
                {
                    writer.WriteLine(message);
                }
            }
            catch (IOException ex)
            {
                // Обработка ошибок доступа к файлу (например, файл занят другим процессом)
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ОШИБКА ЖУРНАЛА] Не удалось записать в файл '{_logFilePath}': {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex) // Ловим другие возможные исключения
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ОШИБКА ЖУРНАЛА] Непредвиденная ошибка при записи в файл: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Считывает и возвращает содержимое файла журнала.
        /// </summary>
        /// <returns>Строка с содержимым файла или сообщение об ошибке.</returns>
        public string ReadLogFile()
        {
            try
            {
                if (!File.Exists(_logFilePath))
                {
                    return $"Файл журнала '{_logFilePath}' не найден.";
                }
                return File.ReadAllText(_logFilePath, Encoding.UTF8);
            }
            catch (IOException ex)
            {
                return $"[ОШИБКА ЧТЕНИЯ ЖУРНАЛА] Не удалось прочитать файл '{_logFilePath}': {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"[ОШИБКА ЧТЕНИЯ ЖУРНАЛА] Непредвиденная ошибка при чтении файла: {ex.Message}";
            }
        }

        // --- Вывод в консоль (остается для отладки) ---

        /// <summary>
        /// Выводит содержимое журнала (из памяти) в консоль.
        /// </summary>
        public void PrintJournal(string title = "Содержимое журнала (в памяти)")
        {
            Console.WriteLine($"\n--- {title} (Записей в памяти: {_entries.Count}) ---");
            if (_entries.Count == 0)
            {
                Console.WriteLine("Журнал (в памяти) пуст.");
            }
            else
            {
                // Выводим последние N записей, чтобы не засорять консоль
                int maxToShow = 20;
                int skipCount = Math.Max(0, _entries.Count - maxToShow);
                if (skipCount > 0) Console.WriteLine($"... (пропущено {skipCount} старых записей)");

                foreach (var entry in _entries.Skip(skipCount))
                {
                    Console.WriteLine(entry.ToString());
                }
            }
            Console.WriteLine("---------------------------------------");
        }
    }
}