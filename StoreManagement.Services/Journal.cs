// Файл: StoreManagement.Services/Journal.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
// using System.Windows.Forms; // Пока уберем, если вызывало сложности
using StoreManagement.Collections;

namespace StoreManagement.Services
{
    public class Journal
    {
        private readonly List<JournalEntry> _entries = new List<JournalEntry>();
        private readonly string _logFilePath;

        public Journal(string logFileName = "journal.log")
        {
            try
            {
                // Используем AppContext.BaseDirectory, как было изначально.
                // Это путь к папке, где находится исполняемый файл вашего UI приложения.
                string baseDirectory = AppContext.BaseDirectory;
                _logFilePath = Path.Combine(baseDirectory, logFileName);

                Console.WriteLine($"--- [ЖУРНАЛ INFO] --- Конструктор Journal вызван.");
                Console.WriteLine($"--- [ЖУРНАЛ INFO] --- BaseDirectory: {baseDirectory}");
                Console.WriteLine($"--- [ЖУРНАЛ INFO] --- Попытка использовать путь для журнала: {_logFilePath}");

                // !!! УДАЛЯЕМ СТАРЫЙ ФАЙЛ ЖУРНАЛА ПЕРЕД ПЕРВОЙ ЗАПИСЬЮ !!!
                if (File.Exists(_logFilePath))
                {
                    File.Delete(_logFilePath);
                    Console.WriteLine($"--- [ЖУРНАЛ INFO] --- Существующий файл журнала {_logFilePath} удален.");
                }

                // Сразу пытаемся что-то записать, чтобы проверить возможность создания файла
                File.AppendAllText(_logFilePath, $"--- Журнал инициализирован (или файл создан): {DateTime.Now} ---{Environment.NewLine}", Encoding.UTF8);
                Console.WriteLine($"--- [ЖУРНАЛ INFO] --- Файл журнала {_logFilePath} успешно проверен/создан/дополнен при инициализации.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n--- [ЖУРНАЛ ОШИБКА В КОНСТРУКТОРЕ] --- Не удалось инициализировать/создать файл '{_logFilePath}':");
                Console.WriteLine(ex.ToString()); // Выводим полную информацию об ошибке
                Console.ResetColor();
                // Если здесь ошибка, _logFilePath может быть недействительным, но мы его все равно сохраняем
                // чтобы последующие попытки записи/чтения также могли выдать ошибку с этим путем.
                if (string.IsNullOrEmpty(_logFilePath) && !string.IsNullOrEmpty(logFileName))
                {
                    // На крайний случай, если Path.Combine не сработал из-за плохого baseDirectory
                    _logFilePath = logFileName;
                }
                else if (string.IsNullOrEmpty(_logFilePath) && string.IsNullOrEmpty(logFileName))
                {
                    _logFilePath = "fallback_journal.log"; // Совсем крайний случай
                }
            }
        }

        // Метод для получения пути (оставляем для проверки из MainForm)
        public string GetActualLogFilePath()
        {
            return _logFilePath ?? "Путь к журналу не определен!";
        }

        private void LogToFile(string message)
        {
            if (string.IsNullOrEmpty(_logFilePath))
            {
                Console.WriteLine("--- [ЖУРНАЛ ОШИБКА] --- Путь к файлу журнала не установлен. Запись невозможна.");
                return;
            }

            Console.WriteLine($"--- [ЖУРНАЛ INFO] --- Попытка записи в LogToFile. Сообщение: {message}");
            try
            {
                File.AppendAllText(_logFilePath, $"{message}{Environment.NewLine}", Encoding.UTF8);
                Console.WriteLine($"--- [ЖУРНАЛ INFO] --- УСПЕШНО записано в лог: {message}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n--- [ЖУРНАЛ ОШИБКА ЗАПИСИ] --- Ошибка при записи в файл '{_logFilePath}':");
                Console.WriteLine(ex.ToString()); // Выводим полную информацию об ошибке
                Console.ResetColor();
            }
        }

        public string ReadLogFile()
        {
            if (string.IsNullOrEmpty(_logFilePath))
            {
                return "Путь к файлу журнала не установлен. Чтение невозможно.";
            }

            Console.WriteLine($"--- [ЖУРНАЛ INFO] --- Попытка чтения ReadLogFile. Путь: {_logFilePath}");
            try
            {
                if (!File.Exists(_logFilePath))
                {
                    Console.WriteLine($"--- [ЖУРНАЛ INFO] --- ReadLogFile: Файл НЕ НАЙДЕН по пути {_logFilePath}");
                    return $"Файл журнала '{_logFilePath}' не найден.";
                }
                Console.WriteLine($"--- [ЖУРНАЛ INFO] --- ReadLogFile: Файл НАЙДЕН. Чтение...");
                return File.ReadAllText(_logFilePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return $"[ОШИБКА ЧТЕНИЯ ЖУРНАЛА] Exception: Непредвиденная ошибка при чтении файла '{_logFilePath}': {ex.Message}";
            }
        }

        // Обработчики событий коллекции (CollectionCountChangedHandler, CollectionReferenceChangedHandler)
        // и PrintJournal остаются без изменений.
        public void CollectionCountChangedHandler(object source, CollectionHandlerEventArgs args)
        {
            JournalEntry entry = new JournalEntry(args);
            _entries.Add(entry); // Добавляем в память (если нужно)
            LogToFile(entry.ToString()); // Записываем в файл
        }

        public void CollectionReferenceChangedHandler(object source, CollectionHandlerEventArgs args)
        {
            JournalEntry entry = new JournalEntry(args);
            _entries.Add(entry);
            LogToFile(entry.ToString());
        }
        public void PrintJournal(string title = "Содержимое журнала (в памяти)") {/*...*/} // Оставляем как есть
    }
}