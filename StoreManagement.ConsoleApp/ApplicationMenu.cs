// Файл: StoreManagement.ConsoleApp/ApplicationMenu.cs (Часть 1)
using System;
using System.Collections.Generic;
using System.IO; // Для Path, File
using System.Linq;
using System.Text; // Для Encoding
using StoreManagement.Collections;
using StoreManagement.ConsoleApp.Demos;
using StoreManagement.Domain;
using StoreManagement.Domain.Comparers; // Для SortByNameComparer
using StoreManagement.Services;

namespace StoreManagement.ConsoleApp
{
    /// <summary>
    /// Реализует консольное меню для управления коллекцией товаров (ЛР №16).
    /// </summary>
    public class ApplicationMenu
    {
        private MyNewCollection<Goods> _collection;
        private readonly Journal _journal;
        private const string DefaultDataFileName = "goods_collection"; // Базовое имя файла без расширения
        private const string JournalFileName = "journal.log";
        private const string ReportFileName = "report.txt";

        // --- Конструктор ---
        public ApplicationMenu()
        {
            // Инициализируем коллекцию и журнал
            _collection = new MyNewCollection<Goods>("Основная Коллекция Товаров");
            _journal = new Journal(JournalFileName);

            // Подписываем журнал на события коллекции
            _collection.CollectionCountChanged += _journal.CollectionCountChangedHandler;
            _collection.CollectionReferenceChanged += _journal.CollectionReferenceChangedHandler;

            Console.WriteLine($"Журнал будет записываться в файл: {Path.Combine(AppContext.BaseDirectory, JournalFileName)}");
        }

        // --- Главный метод запуска меню ---
        public void Run()
        {
            bool exit = false;
            while (!exit)
            {
                DisplayMenu();
                string? choice = Console.ReadLine()?.Trim();
                exit = ProcessChoice(choice);

                if (!exit)
                {
                    Console.WriteLine("\nНажмите Enter для продолжения...");
                    Console.ReadLine(); // Пауза перед следующим показом меню
                }
            }
            Console.WriteLine("Завершение работы приложения.");
        }

        // --- Отображение опций меню ---
        private void DisplayMenu()
        {
            Console.Clear(); // Очищаем консоль для нового показа меню
            Console.WriteLine("===== Меню Управления Коллекцией Товаров =====");
            Console.WriteLine($"Текущее количество элементов: {_collection.Count}");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine(" 1. Автоматически сформировать коллекцию");
            Console.WriteLine(" 2. Просмотреть коллекцию");
            Console.WriteLine(" 3. Добавить элемент");
            Console.WriteLine(" 4. Удалить элемент");
            Console.WriteLine(" 5. Редактировать элемент");
            Console.WriteLine(" 6. Поиск элемента по имени");
            Console.WriteLine(" 7. Сортировать коллекцию (по Цене/Имени)");
            Console.WriteLine(" 8. Фильтровать коллекцию (по Типу/Цене)");
            Console.WriteLine(" 9. Выполнить LINQ-запросы (из ЛР14)");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("10. Сохранить коллекцию в файл");
            Console.WriteLine("11. Загрузить коллекцию из файла");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("12. Просмотреть файл журнала");
            Console.WriteLine("13. Сохранить отчет (результаты LINQ) в файл");
            Console.WriteLine("14. Просмотреть файл отчета");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine(" 0. Выход");
            Console.WriteLine("-------------------------------------------");
            Console.Write("Введите номер опции: ");
        }

        // --- Обработка выбора пользователя ---
        private bool ProcessChoice(string? choice)
        {
            switch (choice)
            {
                case "1":
                    AutoGenerateCollection();
                    break;
                case "2":
                    ViewCollection();
                    break;
                case "3":
                    AddItem();
                    break;
                case "4":
                    RemoveItem();
                    break;
                case "5":
                    EditItem();
                    break;
                case "6":
                    SearchItem();
                    break;
                case "7":
                    SortCollection();
                    break;
                case "8":
                    FilterCollection();
                    break;
                case "9":
                    RunLinqQueries();
                    break;
                case "10":
                    SaveCollection();
                    break;
                case "11":
                    LoadCollection();
                    break;
                case "12":
                    ViewJournal();
                    break;
                case "13":
                    SaveReport();
                    break;
                case "14":
                    ViewReport();
                    break;
                case "0":
                    return true; // Сигнал к выходу
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Неверный ввод. Пожалуйста, выберите опцию из меню.");
                    Console.ResetColor();
                    break;
            }
            return false; // Продолжаем работу
        }

        // --- Вспомогательные методы для ввода данных ---

        private string ReadStringInput(string prompt, bool allowEmpty = false)
        {
            string? input;
            do
            {
                Console.Write($"{prompt}: ");
                input = Console.ReadLine();
                if (!allowEmpty && string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Ввод не может быть пустым.");
                    Console.ResetColor();
                }
                else
                {
                    return input ?? ""; // Возвращаем пустую строку, если ввод null
                }
            } while (true);
        }

        private int ReadIntInput(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            int value;
            while (true)
            {
                Console.Write($"{prompt} ({min}-{max}): ");
                if (int.TryParse(Console.ReadLine(), out value) && value >= min && value <= max)
                {
                    return value;
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Неверный ввод. Введите целое число от {min} до {max}.");
                Console.ResetColor();
            }
        }

        private decimal ReadDecimalInput(string prompt, decimal min = 0.01m, decimal max = decimal.MaxValue)
        {
            decimal value;
            while (true)
            {
                Console.Write($"{prompt} (> {min - 0.01m}): ");
                // Используем Replace для поддержки и точки, и запятой как разделителя
                if (decimal.TryParse(Console.ReadLine()?.Replace('.', ','), out value) && value >= min && value <= max)
                {
                    return value;
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Неверный ввод. Введите положительное число (больше или равно {min}).");
                Console.ResetColor();
            }
        }

        private double ReadDoubleInput(string prompt, double min = 0.0, double max = double.MaxValue)
        {
            double value;
            while (true)
            {
                Console.Write($"{prompt} (>= {min}): ");
                if (double.TryParse(Console.ReadLine()?.Replace('.', ','), out value) && value >= min && value <= max)
                {
                    return value;
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Неверный ввод. Введите число (больше или равно {min}).");
                Console.ResetColor();
            }
        }

        private DateTime ReadDateTimeInput(string prompt)
        {
            DateTime value;
            while (true)
            {
                Console.Write($"{prompt} (гггг-мм-дд): ");
                if (DateTime.TryParse(Console.ReadLine(), out value)) // Пробуем стандартный парсинг
                {
                    // Дополнительная проверка на разумность даты (например, не в далеком прошлом/будущем)
                    if (value.Year > 1900 && value.Year < DateTime.Now.Year + 50)
                    {
                        return value;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Неверный ввод. Год выглядит неправдоподобно.");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Неверный ввод. Введите дату в формате гггг-мм-дд.");
                    Console.ResetColor();
                }
            }
        }

        // --- Реализация методов-обработчиков ---

        private void AutoGenerateCollection()
        {
            Console.WriteLine("\n--- Автоматическое формирование коллекции ---");
            int count = ReadIntInput("Введите количество элементов для генерации", 1, 100);
            _collection.Clear(); // Очищаем старую коллекцию перед генерацией
            Console.WriteLine("Генерация элементов...");
            for (int i = 0; i < count; i++)
            {
                // Используем метод из Lab12Demo для простоты
                // В реальном приложении логика генерации может быть сложнее
                _collection.Add(Lab12Demo.CreateRandomGoods());
            }
            Console.WriteLine($"Сгенерировано {count} элементов.");
            ViewCollection(); // Показываем результат
        }

        private void ViewCollection()
        {
            Console.WriteLine("\n--- Просмотр коллекции ---");
            _collection.Print(_collection.Name); // Используем метод Print из MyCollection
        }

        private void AddItem()
        {
            Console.WriteLine("\n--- Добавление нового элемента ---");
            Console.WriteLine("Выберите тип товара для добавления:");
            Console.WriteLine(" 1. Обычный продукт (Product)");
            Console.WriteLine(" 2. Молочный продукт (DairyProduct)");
            Console.WriteLine(" 3. Игрушка (Toy)");
            Console.Write("Ваш выбор: ");
            string? choice = Console.ReadLine();

            Goods newItem;
            try
            {
                switch (choice)
                {
                    case "1":
                        newItem = new Product();
                        break;
                    case "2":
                        newItem = new DairyProduct();
                        break;
                    case "3":
                        newItem = new Toy();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Неверный выбор типа.");
                        Console.ResetColor();
                        return;
                }
                // Вызываем метод Init() для ввода данных с клавиатуры
                newItem.Init();
                // Добавляем в коллекцию (это вызовет событие и запись в журнал)
                _collection.Add(newItem);
                Console.WriteLine($"Элемент '{newItem.Name}' успешно добавлен.");
            }
            catch (Exception ex) // Ловим ошибки валидации из свойств или ввода
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nОшибка при добавлении элемента: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void RemoveItem()
        {
            Console.WriteLine("\n--- Удаление элемента ---");
            if (_collection.Count == 0)
            {
                Console.WriteLine("Коллекция пуста, удалять нечего.");
                return;
            }

            string nameToRemove = ReadStringInput("Введите точное имя товара для удаления");
            Goods? itemToRemove = null;
            // Ищем элемент по имени (можно использовать LINQ)
            itemToRemove = _collection.FirstOrDefault(item =>
                item.Name.Equals(nameToRemove, StringComparison.OrdinalIgnoreCase));

            if (itemToRemove != null)
            {
                if (_collection.Remove(itemToRemove)) // Remove вызывает событие
                {
                    Console.WriteLine($"Товар '{itemToRemove.Name}' успешно удален.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Не удалось удалить товар '{itemToRemove.Name}'."); // Маловероятно, если нашли
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Товар с именем '{nameToRemove}' не найден.");
                Console.ResetColor();
            }
        }

        private void EditItem()
        {
            Console.WriteLine("\n--- Редактирование элемента ---");
            if (_collection.Count == 0)
            {
                Console.WriteLine("Коллекция пуста, редактировать нечего.");
                return;
            }

            string nameToEdit = ReadStringInput("Введите точное имя товара для редактирования");
            Goods? itemToEdit = _collection.FirstOrDefault(item =>
                item.Name.Equals(nameToEdit, StringComparison.OrdinalIgnoreCase));

            if (itemToEdit != null)
            {
                Console.WriteLine($"Найден товар: {itemToEdit}");
                Console.WriteLine("Введите новые данные (оставьте пустым, чтобы не менять):");

                try
                {
                    // --- Редактирование общих полей Goods ---
                    string newName = ReadStringInput($"Новое имя (текущее: {itemToEdit.Name})", allowEmpty: true);
                    if (!string.IsNullOrWhiteSpace(newName)) itemToEdit.Name = newName; // Валидация в сеттере

                    Console.Write($"Новая цена (текущая: {itemToEdit.Price:C}): ");
                    string priceInput = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(priceInput) && decimal.TryParse(priceInput.Replace('.', ','), out decimal newPrice) && newPrice > 0)
                    {
                        itemToEdit.Price = newPrice;
                    }
                    else if (!string.IsNullOrWhiteSpace(priceInput))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Неверный формат цены, значение не изменено.");
                        Console.ResetColor();
                    }

                    string newManufacturer = ReadStringInput($"Новый производитель (текущий: {itemToEdit.Manufacturer})", allowEmpty: true);
                    if (!string.IsNullOrWhiteSpace(newManufacturer)) itemToEdit.Manufacturer = newManufacturer;

                    // --- Редактирование специфичных полей ---
                    if (itemToEdit is Product product) // Используем pattern matching
                    {
                        Console.Write($"Новый срок годности (гггг-мм-дд) (текущий: {product.ExpirationDate:yyyy-MM-dd}): ");
                        string dateInput = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(dateInput) && DateTime.TryParse(dateInput, out DateTime newDate) && newDate.Date >= DateTime.Today)
                        {
                            product.ExpirationDate = newDate;
                        }
                        else if (!string.IsNullOrWhiteSpace(dateInput))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Неверный формат или дата в прошлом, значение не изменено.");
                            Console.ResetColor();
                        }

                        if (product is DairyProduct dairy) // Вложенная проверка для DairyProduct
                        {
                            Console.Write($"Новая жирность (%) (текущая: {dairy.FatContent:F1}): ");
                            string fatInput = Console.ReadLine() ?? "";
                            if (!string.IsNullOrWhiteSpace(fatInput) && double.TryParse(fatInput.Replace('.', ','), out double newFat) && newFat >= 0)
                            {
                                dairy.FatContent = newFat;
                            }
                            else if (!string.IsNullOrWhiteSpace(fatInput))
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Неверный формат жирности, значение не изменено.");
                                Console.ResetColor();
                            }

                            Console.Write($"Новый объем (л) (текущий: {dairy.Volume:F2}): ");
                            string volInput = Console.ReadLine() ?? "";
                            if (!string.IsNullOrWhiteSpace(volInput) && double.TryParse(volInput.Replace('.', ','), out double newVol) && newVol > 0)
                            {
                                dairy.Volume = newVol;
                            }
                            else if (!string.IsNullOrWhiteSpace(volInput))
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Неверный формат объема, значение не изменено.");
                                Console.ResetColor();
                            }
                        }
                    }
                    else if (itemToEdit is Toy toy)
                    {
                        Console.Write($"Новое возрастное ограничение (лет) (текущее: {toy.AgeRestriction}+): ");
                        string ageInput = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(ageInput) && int.TryParse(ageInput, out int newAge) && newAge >= 0)
                        {
                            toy.AgeRestriction = newAge;
                        }
                        else if (!string.IsNullOrWhiteSpace(ageInput))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Неверный формат возраста, значение не изменено.");
                            Console.ResetColor();
                        }

                        string newMaterial = ReadStringInput($"Новый материал (текущий: {toy.Material})", allowEmpty: true);
                        if (!string.IsNullOrWhiteSpace(newMaterial)) toy.Material = newMaterial;
                    }

                    Console.WriteLine($"Товар '{itemToEdit.Name}' обновлен.");
                    // Событие ReferenceChanged здесь НЕ генерируется, т.к. мы меняем поля существующего объекта,
                    // а не заменяем сам объект в коллекции по индексу.
                    // Если бы мы хотели это отслеживать, потребовался бы другой механизм (например, INotifyPropertyChanged).
                }
                catch (Exception ex) // Ловим ошибки валидации из сеттеров свойств
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка при редактировании элемента: {ex.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Товар с именем '{nameToEdit}' не найден.");
                Console.ResetColor();
            }
        }

        private void SearchItem()
        {
            Console.WriteLine("\n--- Поиск элемента по имени ---");
            if (_collection.Count == 0)
            {
                Console.WriteLine("Коллекция пуста.");
                return;
            }
            string nameToSearch = ReadStringInput("Введите имя товара для поиска (можно часть имени)");

            // Используем LINQ для поиска (без учета регистра)
            var foundItems = _collection
                             .Where(item => item.Name.Contains(nameToSearch, StringComparison.OrdinalIgnoreCase))
                             .ToList(); // Материализуем результат

            if (foundItems.Any())
            {
                Console.WriteLine($"Найдено {foundItems.Count} товаров:");
                int index = 0;
                foreach (var item in foundItems)
                {
                    Console.WriteLine($" [{index++}]: {item}");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Товары, содержащие '{nameToSearch}' в имени, не найдены.");
                Console.ResetColor();
            }
        }

        private void SortCollection()
        {
            Console.WriteLine("\n--- Сортировка коллекции ---");
            if (_collection.Count < 2)
            {
                Console.WriteLine("В коллекции недостаточно элементов для сортировки.");
                return;
            }

            Console.WriteLine("Выберите критерий сортировки:");
            Console.WriteLine(" 1. По Цене (по возрастанию, стандартно)");
            Console.WriteLine(" 2. По Имени (по возрастанию)");
            Console.Write("Ваш выбор: ");
            string? choice = Console.ReadLine();

            // Так как MyNewCollection не имеет встроенного метода Sort,
            // мы создадим новый отсортированный List и заменим им содержимое коллекции.
            // Это приведет к событиям Remove/Add в журнале!
            List<Goods> sortedList;
            try
            {
                switch (choice)
                {
                    case "1":
                        // Используем стандартный LINQ OrderBy, который использует IComparable<Goods> (по цене)
                        sortedList = _collection.OrderBy(g => g).ToList();
                        Console.WriteLine("Сортировка по цене...");
                        break;
                    case "2":
                        // Используем OrderBy с компаратором SortByNameComparer
                        sortedList = _collection.OrderBy(g => g, new SortByNameComparer()).ToList();
                        Console.WriteLine("Сортировка по имени...");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Неверный выбор критерия.");
                        Console.ResetColor();
                        return;
                }

                // Заменяем содержимое коллекции (генерирует события!)
                string collectionName = _collection.Name; // Сохраняем имя
                _collection.Clear(); // Удалит все старые (события Remove)
                _collection.Name = collectionName; // Восстанавливаем имя
                _collection.AddRange(sortedList); // Добавит все новые (события Add)

                Console.WriteLine("Коллекция отсортирована.");
                ViewCollection();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nОшибка при сортировке: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void FilterCollection()
        {
            Console.WriteLine("\n--- Фильтрация коллекции ---");
            if (_collection.Count == 0)
            {
                Console.WriteLine("Коллекция пуста.");
                return;
            }

            Console.WriteLine("Выберите критерий фильтрации:");
            Console.WriteLine(" 1. По типу товара");
            Console.WriteLine(" 2. По цене (больше или равно)");
            Console.Write("Ваш выбор: ");
            string? choice = Console.ReadLine();

            IEnumerable<Goods> filteredResult = Enumerable.Empty<Goods>(); // Пустой результат по умолчанию

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Выберите тип:");
                        Console.WriteLine("  a. Product (Обычный продукт)");
                        Console.WriteLine("  b. DairyProduct (Молочный продукт)");
                        Console.WriteLine("  c. Toy (Игрушка)");
                        Console.Write(" Ваш выбор типа: ");
                        string? typeChoice = Console.ReadLine()?.ToLower();
                        switch (typeChoice)
                        {
                            case "a": filteredResult = _collection.Where(item => item is Product && item.GetType() == typeof(Product)); break; // Точное совпадение типа
                            case "b": filteredResult = _collection.OfType<DairyProduct>(); break;
                            case "c": filteredResult = _collection.OfType<Toy>(); break;
                            default: Console.WriteLine("Неверный выбор типа."); return;
                        }
                        Console.WriteLine($"Фильтрация по типу '{typeChoice}'...");
                        break;
                    case "2":
                        decimal minPrice = ReadDecimalInput("Введите минимальную цену для фильтрации");
                        filteredResult = _collection.Where(item => item.Price >= minPrice);
                        Console.WriteLine($"Фильтрация по цене >= {minPrice:C}...");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Неверный выбор критерия.");
                        Console.ResetColor();
                        return;
                }

                // Выводим результат фильтрации
                Console.WriteLine("\nРезультаты фильтрации:");
                List<Goods> resultList = filteredResult.ToList(); // Материализуем
                if (resultList.Any())
                {
                    int index = 0;
                    foreach (var item in resultList)
                    {
                        Console.WriteLine($" [{index++}]: {item}");
                    }
                }
                else
                {
                    Console.WriteLine("(Нет элементов, удовлетворяющих условию)");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nОшибка при фильтрации: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void RunLinqQueries()
        {
            Console.WriteLine("\n--- Выполнение LINQ-запросов (из ЛР14) ---");
            if (_collection.Count == 0)
            {
                Console.WriteLine("Коллекция пуста. Запросы не выполняются.");
                return;
            }
            // Просто повторно используем часть логики из Lab14Demo, применяя ее к _collection
            // Это не идеальный подход (дублирование), но для ЛР допустим.
            // В реальном приложении запросы были бы вынесены в отдельный сервис.

            try
            {
                // 1. Самая дорогая игрушка
                var mostExpensiveToy = _collection.OfType<Toy>().OrderByDescending(t => t.Price).FirstOrDefault();
                Console.WriteLine("\n* Самая дорогая игрушка:");
                if (mostExpensiveToy != null) Console.WriteLine($" - {mostExpensiveToy}"); else Console.WriteLine(" - Игрушки не найдены.");

                // 2. Количество молочных продуктов
                int dairyCount = _collection.OfType<DairyProduct>().Count();
                Console.WriteLine($"\n* Количество молочных продуктов: {dairyCount}");

                // 3. Средняя цена всех товаров
                decimal avgPrice = _collection.Any() ? _collection.Average(g => g.Price) : 0m;
                Console.WriteLine($"\n* Средняя цена всех товаров: {avgPrice:C}");

                // 4. Группировка по производителю
                Console.WriteLine("\n* Группировка по производителю:");
                var grouped = _collection.GroupBy(g => g.Manufacturer);
                foreach (var group in grouped)
                {
                    Console.WriteLine($" - {group.Key}: {group.Count()} шт.");
                }
                Console.WriteLine("\nЗапросы выполнены.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nОшибка при выполнении LINQ-запросов: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void SaveCollection()
        {
            Console.WriteLine("\n--- Сохранение коллекции в файл ---");
            Console.WriteLine("Выберите формат сохранения:");
            Console.WriteLine(" 1. Binary");
            Console.WriteLine(" 2. JSON");
            Console.WriteLine(" 3. XML");
            Console.Write("Ваш выбор: ");
            string? choice = Console.ReadLine();
            SerializationFormat format;
            string fileExtension;

            switch (choice)
            {
                case "1": format = SerializationFormat.Binary; fileExtension = ".bin"; break;
                case "2": format = SerializationFormat.Json; fileExtension = ".json"; break;
                case "3": format = SerializationFormat.Xml; fileExtension = ".xml"; break;
                default: Console.WriteLine("Неверный выбор формата."); return;
            }

            string filePath = Path.Combine(AppContext.BaseDirectory, DefaultDataFileName + fileExtension);
            Console.WriteLine($"Попытка сохранения в файл: {filePath}");
            PersistenceService.SaveCollection(_collection, filePath, format);
        }

        private void LoadCollection()
        {
            Console.WriteLine("\n--- Загрузка коллекции из файла ---");
            Console.WriteLine("Выберите формат загрузки:");
            Console.WriteLine(" 1. Binary");
            Console.WriteLine(" 2. JSON");
            Console.WriteLine(" 3. XML");
            Console.Write("Ваш выбор: ");
            string? choice = Console.ReadLine();
            SerializationFormat format;
            string fileExtension;

            switch (choice)
            {
                case "1": format = SerializationFormat.Binary; fileExtension = ".bin"; break;
                case "2": format = SerializationFormat.Json; fileExtension = ".json"; break;
                case "3": format = SerializationFormat.Xml; fileExtension = ".xml"; break;
                default: Console.WriteLine("Неверный выбор формата."); return;
            }

            string filePath = Path.Combine(AppContext.BaseDirectory, DefaultDataFileName + fileExtension);
            Console.WriteLine($"Попытка загрузки из файла: {filePath}");

            // Отписываем старую коллекцию от журнала перед заменой
            _collection.CollectionCountChanged -= _journal.CollectionCountChangedHandler;
            _collection.CollectionReferenceChanged -= _journal.CollectionReferenceChangedHandler;

            // Загружаем новую коллекцию
            _collection = PersistenceService.LoadCollection<Goods>(_collection.Name, filePath, format); // Используем старое имя

            // Подписываем новую коллекцию на журнал
            _collection.CollectionCountChanged += _journal.CollectionCountChangedHandler;
            _collection.CollectionReferenceChanged += _journal.CollectionReferenceChangedHandler;

            Console.WriteLine("Загрузка завершена (или создана пустая коллекция, если файл не найден/ошибка).");
            ViewCollection();
        }

        private void ViewJournal()
        {
            Console.WriteLine("\n--- Просмотр файла журнала ---");
            string logContent = _journal.ReadLogFile();
            Console.WriteLine($"Содержимое файла '{JournalFileName}':");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine(logContent);
            Console.WriteLine("-------------------------------------------");
        }

        private void SaveReport()
        {
            Console.WriteLine("\n--- Сохранение отчета (результаты LINQ) в файл ---");
            if (_collection.Count == 0)
            {
                Console.WriteLine("Коллекция пуста. Отчет не будет содержать данных.");
                // return; // Можно выйти, а можно создать пустой отчет
            }

            string reportFilePath = Path.Combine(AppContext.BaseDirectory, ReportFileName);
            StringBuilder reportContent = new StringBuilder();

            reportContent.AppendLine($"===== Отчет по Коллекции '{_collection.Name}' от {DateTime.Now} =====");
            reportContent.AppendLine($"Всего элементов: {_collection.Count}");
            reportContent.AppendLine("-------------------------------------------");

            // Добавляем результаты тех же LINQ-запросов, что и в RunLinqQueries
            try
            {
                // 1. Самая дорогая игрушка
                var mostExpensiveToy = _collection.OfType<Toy>().OrderByDescending(t => t.Price).FirstOrDefault();
                reportContent.AppendLine("\n* Самая дорогая игрушка:");
                if (mostExpensiveToy != null) reportContent.AppendLine($" - {mostExpensiveToy}"); else reportContent.AppendLine(" - Игрушки не найдены.");

                // 2. Количество молочных продуктов
                int dairyCount = _collection.OfType<DairyProduct>().Count();
                reportContent.AppendLine($"\n* Количество молочных продуктов: {dairyCount}");

                // 3. Средняя цена всех товаров
                decimal avgPrice = _collection.Any() ? _collection.Average(g => g.Price) : 0m;
                reportContent.AppendLine($"\n* Средняя цена всех товаров: {avgPrice:C}");

                // 4. Группировка по производителю
                reportContent.AppendLine("\n* Группировка по производителю:");
                var grouped = _collection.GroupBy(g => g.Manufacturer);
                foreach (var group in grouped)
                {
                    reportContent.AppendLine($" - {group.Key}: {group.Count()} шт.");
                }
                reportContent.AppendLine("-------------------------------------------");
                reportContent.AppendLine("===== Конец отчета =====");

                // Записываем в файл
                File.WriteAllText(reportFilePath, reportContent.ToString(), Encoding.UTF8);
                Console.WriteLine($"Отчет успешно сохранен в файл: {reportFilePath}");

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nОшибка при формировании или сохранении отчета: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void ViewReport()
        {
            Console.WriteLine("\n--- Просмотр файла отчета ---");
            string reportFilePath = Path.Combine(AppContext.BaseDirectory, ReportFileName);
            try
            {
                if (!File.Exists(reportFilePath))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Файл отчета '{reportFilePath}' не найден. Сначала сохраните отчет (опция 13).");
                    Console.ResetColor();
                    return;
                }
                string reportContent = File.ReadAllText(reportFilePath, Encoding.UTF8);
                Console.WriteLine($"Содержимое файла '{ReportFileName}':");
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine(reportContent);
                Console.WriteLine("-------------------------------------------");
            }
            catch (IOException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ОШИБКА ЧТЕНИЯ ОТЧЕТА] Не удалось прочитать файл '{reportFilePath}': {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ОШИБКА ЧТЕНИЯ ОТЧЕТА] Непредвиденная ошибка при чтении файла: {ex.Message}");
                Console.ResetColor();
            }
        }

    } // Конец класса ApplicationMenu
} // Конец namespace