// Файл: StoreManagement.ConsoleApp/Demos/Lab10Demo.cs
using System;
using System.Linq; // Для Count() в запросах RTTI
using StoreManagement.Domain; // Доступ к классам Goods, Product, Toy и т.д.
using StoreManagement.Domain.Comparers; // Доступ к SortByNameComparer
using StoreManagement.Domain.Interfaces; // Доступ к IInit

namespace StoreManagement.ConsoleApp.Demos
{
    /// <summary>
    /// Демонстрация функциональности Лабораторной Работы №10.
    /// </summary>
    public static class Lab10Demo
    {
        // --- Вспомогательный класс для демонстрации IInit ---
        private class Service : IInit
        {
            public string ServiceName { get; set; } = "Неизвестная услуга";
            public decimal HourlyRate { get; set; } = 500;

            public void Init()
            {
                Console.WriteLine("\n--- Инициализация объекта Service ---");
                ServiceName = Goods.ReadString("Введите название услуги: "); // Используем хелпер из Goods
                HourlyRate = Goods.ReadDecimal("Введите почасовую ставку: ");
                Console.WriteLine("--- Инициализация Service завершена ---");
            }

            public void RandomInit()
            {
                string[] services = { "Консультация", "Доставка", "Упаковка", "Сборка" };
                ServiceName = services[Goods.random.Next(services.Length)];
                HourlyRate = (decimal)(Goods.random.NextDouble() * 1000 + 100); // Ставка от 100 до 1100
            }

            public override string ToString()
            {
                return $"Услуга: {ServiceName}, Ставка: {HourlyRate:C}/час";
            }
        }

        // --- Основной метод демонстрации ---
        public static void Run()
        {
            Console.WriteLine("===== Демонстрация ЛР №10: Наследование, RTTI, Интерфейсы =====");

            // --- Часть 1: Наследование и полиморфизм ---
            Console.WriteLine("\n--- Часть 1: Наследование и полиморфизм ---");
            Goods[] inventory = CreateRandomInventory(5); // Создаем массив из 5 случайных товаров
            ShowInventory("Исходный инвентарь (вызов виртуального Show):", inventory);
            ExplainPolymorphism();

            // --- Часть 2: RTTI (is/as) ---
            Console.WriteLine("\n--- Часть 2: RTTI (Запросы к коллекции) ---");
            RunRttiQueries(inventory);

            // --- Часть 3: Интерфейсы ---
            Console.WriteLine("\n--- Часть 3: Интерфейсы (IComparable, IComparer, IInit, ICloneable) ---");
            DemonstrateSorting(inventory);
            DemonstrateBinarySearch(inventory); // Демонстрируем после сортировки по цене
            DemonstrateIInitInterface();
            DemonstrateCloning(inventory[0]); // Клонируем первый элемент для примера

            Console.WriteLine("\n===== Демонстрация ЛР №10 Завершена =====");
        }

        // --- Методы для Части 1 ---

        private static Goods[] CreateRandomInventory(int size)
        {
            Goods[] items = new Goods[size];
            for (int i = 0; i < items.Length; i++)
            {
                int typeChoice = Goods.random.Next(3); // 0: Dairy, 1: Toy, 2: Product (менее вероятно)
                switch (typeChoice)
                {
                    case 0:
                        items[i] = new DairyProduct();
                        break;
                    case 1:
                        items[i] = new Toy();
                        break;
                    case 2:
                    default: // На случай, если захотим добавить еще типы
                        items[i] = new Product(); // Создаем обычный Product
                        break;
                }
                items[i].RandomInit(); // Инициализируем случайными данными
            }
            return items;
        }

        private static void ShowInventory(string title, Goods[] items)
        {
            Console.WriteLine($"\n{title}");
            if (items == null || items.Length == 0)
            {
                Console.WriteLine("Инвентарь пуст.");
                return;
            }
            for (int i = 0; i < items.Length; i++)
            {
                Console.WriteLine($"--- Элемент {i + 1} ---");
                items[i].Show(); // Полиморфный вызов - будет вызван Show() конкретного класса
                // Console.WriteLine(items[i].ToString()); // Альтернатива - ToString() тоже виртуальный
            }
            Console.WriteLine("--------------------");
        }

        private static void ExplainPolymorphism()
        {
            Console.WriteLine("\n* Пояснение полиморфизма:");
            Console.WriteLine("  Когда мы вызываем метод `item.Show()` для элемента массива `Goods[]`,");
            Console.WriteLine("  благодаря тому, что метод `Show()` объявлен как `virtual` в базовом классе `Goods`");
            Console.WriteLine("  и переопределен (`override`) в производных классах (`DairyProduct`, `Toy`, `Product`),");
            Console.WriteLine("  во время выполнения программы определяется фактический тип объекта (`DairyProduct`, `Toy` и т.д.),");
            Console.WriteLine("  и вызывается соответствующая версия метода `Show()`. Это и есть полиморфизм.");
            Console.WriteLine("  Если бы `Show()` не был виртуальным, всегда вызывалась бы реализация из `Goods`.");
        }

        // --- Методы для Части 2 ---

        private static void RunRttiQueries(Goods[] items)
        {
            if (items == null || items.Length == 0)
            {
                Console.WriteLine("Инвентарь пуст, запросы не выполняются.");
                return;
            }

            // Запрос 1: Найти все игрушки (is)
            Console.WriteLine("\nЗапрос 1: Найти все игрушки:");
            int toyCount = 0;
            foreach (var item in items)
            {
                if (item is Toy) // Проверка типа с помощью 'is'
                {
                    Console.WriteLine($"- Найден {item}"); // Используем ToString()
                    toyCount++;
                }
            }
            if (toyCount == 0) Console.WriteLine("- Игрушки не найдены.");

            // Запрос 2: Подсчитать количество молочных продуктов (as)
            Console.WriteLine("\nЗапрос 2: Подсчитать количество молочных продуктов:");
            int dairyCount = 0;
            foreach (var item in items)
            {
                // Пытаемся привести к типу DairyProduct с помощью 'as'
                // Если item не является DairyProduct, результат будет null
                DairyProduct? dairy = item as DairyProduct;
                if (dairy != null)
                {
                    dairyCount++;
                }
            }
            // Альтернативный подсчет с LINQ (требует using System.Linq):
            // int dairyCountLinq = items.Count(item => item is DairyProduct);
            Console.WriteLine($"- Найдено молочных продуктов: {dairyCount}");

            // Запрос 3: Найти товары дороже X (ввод с клавиатуры)
            Console.WriteLine("\nЗапрос 3: Найти товары дороже X:");
            decimal priceThreshold = Goods.ReadDecimal("Введите минимальную цену для поиска: ");
            int foundCount = 0;
            foreach (var item in items)
            {
                // RTTI здесь не нужен, простое сравнение свойства
                if (item.Price > priceThreshold)
                {
                    Console.WriteLine($"- Найден {item}");
                    foundCount++;
                }
            }
            if (foundCount == 0) Console.WriteLine($"- Товары дороже {priceThreshold:C} не найдены.");
        }

        // --- Методы для Части 3 ---

        private static void DemonstrateSorting(Goods[] items)
        {
            if (items == null || items.Length == 0) return;

            Console.WriteLine("\n--- Демонстрация IComparable<Goods> (сортировка по цене) ---");
            Goods[] sortedByPrice = (Goods[])items.Clone(); // Клонируем, чтобы не менять исходный массив
            Array.Sort(sortedByPrice); // Использует Goods.CompareTo(Goods other)
            ShowInventory("Инвентарь, отсортированный по ЦЕНЕ:", sortedByPrice);

            Console.WriteLine("\n--- Демонстрация IComparer<Goods> (сортировка по имени) ---");
            Goods[] sortedByName = (Goods[])items.Clone();
            Array.Sort(sortedByName, new SortByNameComparer()); // Использует наш компаратор
            ShowInventory("Инвентарь, отсортированный по ИМЕНИ:", sortedByName);
        }

        private static void DemonstrateBinarySearch(Goods[] items)
        {
            if (items == null || items.Length < 2)
            {
                Console.WriteLine("\nБинарный поиск требует хотя бы 2 элемента в отсортированном массиве.");
                return;
            }

            Console.WriteLine("\n--- Демонстрация бинарного поиска (в массиве, отсортированном по ЦЕНЕ) ---");
            // Сначала сортируем массив по цене (ключ поиска)
            Goods[] sortedByPrice = (Goods[])items.Clone();
            Array.Sort(sortedByPrice); // Сортировка по умолчанию (по цене)

            // Создаем "фиктивный" товар с ценой для поиска
            // Возьмем цену из середины массива для большей вероятности нахождения
            decimal priceToSearch = sortedByPrice[sortedByPrice.Length / 2].Price;
            // Важно: Для бинарного поиска нужен объект того же типа или совместимого по IComparable
            // Создадим временный объект Toy (или любой другой наследник Goods) только с нужной ценой
            Goods searchKey = new Toy("Поиск", priceToSearch, "N/A", 0, "N/A");

            Console.WriteLine($"Ищем товар с ценой примерно: {priceToSearch:C}");

            int index = Array.BinarySearch(sortedByPrice, searchKey); // Использует CompareTo по цене

            if (index >= 0)
            {
                Console.WriteLine($"Найден элемент с такой же ценой на позиции {index}:");
                sortedByPrice[index].Show();
                // Примечание: Если есть несколько элементов с одинаковой ценой, BinarySearch может вернуть индекс любого из них.
            }
            else
            {
                // Если элемент не найден, BinarySearch возвращает отрицательное число,
                // которое является побитовым дополнением индекса первого элемента,
                // который больше искомого значения.
                int nextLargerIndex = ~index;
                Console.WriteLine($"Товар с ценой {priceToSearch:C} не найден.");
                if (nextLargerIndex < sortedByPrice.Length)
                {
                    Console.WriteLine($"Следующий больший элемент находится на позиции {nextLargerIndex}:");
                    sortedByPrice[nextLargerIndex].Show();
                }
                else
                {
                    Console.WriteLine("Все элементы в массиве меньше искомой цены.");
                }
            }
        }

        private static void DemonstrateIInitInterface()
        {
            Console.WriteLine("\n--- Демонстрация интерфейса IInit ---");

            // Создаем массив типа IInit
            IInit[] initializables = new IInit[3];

            // Заполняем объектами из иерархии Goods и новым классом Service
            initializables[0] = new DairyProduct();
            initializables[1] = new Toy();
            initializables[2] = new Service(); // Наш новый класс, не из иерархии Goods

            Console.WriteLine("\n* Вызов RandomInit() для всех объектов через интерфейс IInit:");
            foreach (var item in initializables)
            {
                item.RandomInit();
                Console.WriteLine($"- Инициализирован: {item}"); // Используем ToString()
            }

            Console.WriteLine("\n* Вызов Init() (ручной ввод) для всех объектов через интерфейс IInit:");
            foreach (var item in initializables)
            {
                Console.WriteLine($"\nИнициализация объекта типа: {item.GetType().Name}");
                item.Init();
                Console.WriteLine($"- Результат ручной инициализации: {item}");
            }
        }

        private static void DemonstrateCloning(Goods original)
        {
            if (original == null) return;

            Console.WriteLine("\n--- Демонстрация ICloneable (Clone и ShallowCopy) ---");
            Console.WriteLine("Оригинальный объект:");
            original.Show();

            // Глубокое клонирование (используем Clone())
            Goods? deepClone = null;
            try
            {
                deepClone = original.Clone() as Goods; // Приведение типа после клонирования
                if (deepClone == null) throw new InvalidCastException("Clone() вернул несовместимый тип.");

                Console.WriteLine("\nГлубокая копия (Clone):");
                deepClone.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при глубоком клонировании: {ex.Message}");
            }


            // Поверхностное клонирование (используем ShallowCopy())
            Goods? shallowClone = null;
            try
            {
                shallowClone = original.ShallowCopy() as Goods;
                if (shallowClone == null) throw new InvalidCastException("ShallowCopy() вернул несовместимый тип.");

                Console.WriteLine("\nПоверхностная копия (ShallowCopy):");
                shallowClone.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при поверхностном клонировании: {ex.Message}");
            }

            // Демонстрация разницы (если возможно)
            Console.WriteLine("\nИзменим свойство Name у ОРИГИНАЛА:");
            string oldName = original.Name;
            original.Name = original.Name + " (изменен)";
            original.Price += 10; // Изменим и цену для наглядности

            Console.WriteLine("\nОригинал после изменения:");
            original.Show();

            if (deepClone != null)
            {
                Console.WriteLine("\nГлубокая копия ПОСЛЕ изменения оригинала:");
                deepClone.Show(); // Должна остаться НЕИЗМЕННОЙ
            }
            if (shallowClone != null)
            {
                Console.WriteLine("\nПоверхностная копия ПОСЛЕ изменения оригинала:");
                shallowClone.Show(); // Тоже должна остаться НЕИЗМЕННОЙ, т.к. Name и Price - значимые/неизменяемые типы
            }

            // Возвращаем имя оригинала обратно, чтобы не влиять на другие тесты
            original.Name = oldName;
            original.Price -= 10;


            Console.WriteLine("\n* Пояснение Clone vs ShallowCopy:");
            Console.WriteLine("  - `ShallowCopy` (через `MemberwiseClone`) создает новый объект и копирует значения полей.");
            Console.WriteLine("    Если поле является типом значения (int, decimal, struct DateTime), копируется само значение.");
            Console.WriteLine("    Если поле является ссылочным типом (string, другой класс), копируется ССЫЛКА на тот же объект в памяти.");
            Console.WriteLine("    В нашем случае `string` хоть и ссылочный тип, но он неизменяемый (immutable), поэтому изменение строки в оригинале создает новую строку, не затрагивая копию.");
            Console.WriteLine("  - `Clone` должен создавать 'глубокую' копию. Это значит, что если бы у нас были изменяемые ссылочные поля (например, List<string>),");
            Console.WriteLine("    метод `Clone` должен был бы создать НОВЫЙ экземпляр этого списка и скопировать его содержимое, а не просто скопировать ссылку на старый список.");
            Console.WriteLine("    В текущей реализации классов `Goods`, `Product`, `Toy` и `DairyProduct` все поля либо значимые типы, либо неизменяемый `string`,");
            Console.WriteLine("    поэтому результат `Clone` (реализованного через конструктор копирования) и `ShallowCopy` фактически совпадает.");
            Console.WriteLine("    Разница стала бы очевидной при наличии изменяемых ссылочных полей.");

        }
    }
}