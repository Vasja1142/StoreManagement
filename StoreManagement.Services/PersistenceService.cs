// Файл: StoreManagement.Services/PersistenceService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // Для BinaryFormatter
using System.Text.Json;                               // Для JsonSerializer
using System.Xml.Serialization;                       // Для XmlSerializer
using StoreManagement.Collections;                    // Для MyNewCollection
using StoreManagement.Domain;                         // Для Goods

namespace StoreManagement.Services
{
    public enum SerializationFormat
    {
        Binary,
        Json,
        Xml
    }

    /// <summary>
    /// Предоставляет методы для сохранения и загрузки коллекций в различных форматах.
    /// </summary>
    public static class PersistenceService
    {
        // --- Сохранение ---

        public static void SaveCollection<T>(MyNewCollection<T> collection, string filePath, SerializationFormat format)
            where T : Goods // Уточняем, что работаем с коллекцией Goods для полиморфизма
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));

            // System.Text.Json не сериализует MyNewCollection напрямую так, как нам нужно (он видит только публичные свойства).
            // Проще всего сериализовать внутренние данные - список элементов.
            // Преобразуем коллекцию в List<T> перед сериализацией.
            List<T> listToSerialize = new List<T>(collection);

            try
            {
                switch (format)
                {
                    case SerializationFormat.Binary:
                        SaveBinary(listToSerialize, filePath);
                        break;
                    case SerializationFormat.Json:
                        SaveJson(listToSerialize, filePath);
                        break;
                    case SerializationFormat.Xml:
                        SaveXml(listToSerialize, filePath);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(format), "Неподдерживаемый формат сериализации.");
                }
                Console.WriteLine($"Коллекция '{collection.Name}' успешно сохранена в '{filePath}' (формат: {format})");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ОШИБКА СОХРАНЕНИЯ] Не удалось сохранить коллекцию в '{filePath}': {ex.Message}");
                // Дополнительно можно вывести InnerException, если есть
                if (ex.InnerException != null) Console.WriteLine($"  Внутренняя ошибка: {ex.InnerException.Message}");
                Console.ResetColor();
                // Можно пробросить исключение дальше, если нужно остановить выполнение
                // throw;
            }
        }

        private static void SaveBinary<T>(List<T> list, string filePath)
        {
#pragma warning disable SYSLIB0011 // Подавление предупреждения об устаревшем BinaryFormatter
            // BinaryFormatter требует, чтобы все типы были [Serializable]
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                formatter.Serialize(fs, list);
            }
#pragma warning restore SYSLIB0011
        }

        private static void SaveJson<T>(List<T> list, string filePath)
        {
            // Используем System.Text.Json
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // Для читаемости файла
            };
            string jsonString = JsonSerializer.Serialize(list, options);
            File.WriteAllText(filePath, jsonString);
        }

        private static void SaveXml<T>(List<T> list, string filePath)
            where T : Goods // Уточнение для XmlSerializer
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(fs, list);
            }
        }


        // --- Загрузка ---

        public static MyNewCollection<T> LoadCollection<T>(string collectionName, string filePath, SerializationFormat format)
             where T : Goods // Уточняем, что работаем с коллекцией Goods
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n[ПРЕДУПРЕЖДЕНИЕ] Файл для загрузки не найден: '{filePath}'");
                Console.ResetColor();
                return new MyNewCollection<T>(collectionName); // Возвращаем пустую коллекцию
            }

            List<T>? loadedList = null; // Инициализируем null

            try
            {
                switch (format)
                {
                    case SerializationFormat.Binary:
                        loadedList = LoadBinary<T>(filePath);
                        break;
                    case SerializationFormat.Json:
                        loadedList = LoadJson<T>(filePath);
                        break;
                    case SerializationFormat.Xml:
                        loadedList = LoadXml<T>(filePath);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(format), "Неподдерживаемый формат десериализации.");
                }

                Console.WriteLine($"Коллекция '{collectionName}' успешно загружена из '{filePath}' (формат: {format})");
                // Создаем MyNewCollection из загруженного списка
                return new MyNewCollection<T>(collectionName, loadedList ?? new List<T>());
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ОШИБКА ЗАГРУЗКИ] Не удалось загрузить коллекцию из '{filePath}': {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"  Внутренняя ошибка: {ex.InnerException.Message}");
                Console.ResetColor();
                // Возвращаем пустую коллекцию в случае ошибки
                return new MyNewCollection<T>(collectionName);
            }
        }

        private static List<T>? LoadBinary<T>(string filePath)
        {
#pragma warning disable SYSLIB0011
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                // Приведение типа может вызвать исключение, если файл поврежден или имеет неверный формат
                return formatter.Deserialize(fs) as List<T>;
            }
#pragma warning restore SYSLIB0011
        }

        private static List<T>? LoadJson<T>(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                // Опции полиморфизма должны совпадать с опциями при сохранении
            };
            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(jsonString, options);
        }

        private static List<T>? LoadXml<T>(string filePath)
            where T : Goods
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return serializer.Deserialize(fs) as List<T>;
            }
        }
    }
}