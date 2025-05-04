// Файл: StoreManagement.ConsoleApp/Demos/Lab14Demo.cs
using System;
using System.Collections.Generic;
using System.Diagnostics; // Для Stopwatch
using System.Linq;        // Главное пространство имен для LINQ
using StoreManagement.Domain;     // Доступ к Goods и наследникам
using StoreManagement.Collections; // Доступ к MyNewCollection (для Части 2)
using StoreManagement.Services; // Может понадобиться для методов расширения (Часть 2)

namespace StoreManagement.ConsoleApp.Demos
{
    /// <summary>
    /// Демонстрация функциональности Лабораторной Работы №14: LINQ to Objects.
    /// </summary>
    public static class Lab14Demo
    {
        // --- Общие данные для демонстрации ---
        private static Queue<Dictionary<string, Goods>> _shopQueue = new Queue<Dictionary<string, Goods>>();
        private static MyNewCollection<Goods> _myCollection = new MyNewCollection<Goods>("Моя Коллекция");

        public static void Run()
        {
            Console.WriteLine("\n===== Демонстрация ЛР №14: LINQ to Objects =====");

            // Подготовка данных
            PrepareData();

            // --- Часть 1: LINQ к стандартным коллекциям ---
            Console.WriteLine("\n--- Часть 1: LINQ к стандартным коллекциям (Queue<Dictionary<string, Goods>>) ---");
            DemonstrateLinqToStandardCollections();

            // --- Часть 2: Методы расширения для своей коллекции ---
            Console.WriteLine("\n--- Часть 2: Методы расширения для MyNewCollection<Goods> ---");
            DemonstrateExtensionMethods(); // Раскомментировали вызов

            Console.WriteLine("\n===== Демонстрация ЛР №14 Завершена =====");
        }

        // --- Подготовка тестовых данных ---
        private static void PrepareData()
        {
            Console.WriteLine("\n* Подготовка тестовых данных...");
            _shopQueue.Clear();
            _myCollection.Clear();

            // Отдел 1: Молочный
            var dairyDept = new Dictionary<string, Goods>();
            var milk = new DairyProduct("Молоко Простое", 75.5m, "МолЗавод 1", DateTime.Today.AddDays(7), 3.2, 1.0);
            var cheese = new DairyProduct("Сыр Гауда", 450m, "СырКомбинат", DateTime.Today.AddDays(30), 45.0, 0.25);
            var yogurt = new DairyProduct("Йогурт Вишня", 55m, "МолЗавод 1", DateTime.Today.AddDays(14), 2.5, 0.125);
            dairyDept.Add(milk.Name, milk);
            dairyDept.Add(cheese.Name, cheese);
            dairyDept.Add(yogurt.Name, yogurt);

            // Отдел 2: Игрушки
            var toyDept = new Dictionary<string, Goods>();
            var car = new Toy("Машинка Спорт", 550m, "ИграПром", 3, "Пластик");
            var doll = new Toy("Кукла Анна", 1200m, "Мир Кукол", 5, "Пластик/Текстиль");
            var constructor = new Toy("Конструктор Башня", 2100m, "СтройДеталь", 6, "Пластик");
            toyDept.Add(car.Name, car);
            toyDept.Add(doll.Name, doll);
            toyDept.Add(constructor.Name, constructor);

            // Отдел 3: Смешанный
            var mixedDept = new Dictionary<string, Goods>();
            var bread = new Product("Хлеб Ржаной", 45m, "Пекарь", DateTime.Today.AddDays(3));
            var juice = new Product("Сок Яблоко", 90m, "Сады", DateTime.Today.AddMonths(6));
            var ball = new Toy("Мяч Футбольный", 800m, "СпортИнвест", 0, "Резина");
            // Добавим товар с тем же производителем, что и в молочном отделе
            var kefir = new DairyProduct("Кефир 1%", 65m, "МолЗавод 1", DateTime.Today.AddDays(10), 1.0, 0.5);
            mixedDept.Add(bread.Name, bread);
            mixedDept.Add(juice.Name, juice);
            mixedDept.Add(ball.Name, ball);
            mixedDept.Add(kefir.Name, kefir);

            // Добавляем отделы в очередь магазина
            _shopQueue.Enqueue(dairyDept);
            _shopQueue.Enqueue(toyDept);
            _shopQueue.Enqueue(mixedDept);

            // Заполняем нашу коллекцию для Части 2
            _myCollection.AddRange(dairyDept.Values);
            _myCollection.AddRange(toyDept.Values);
            _myCollection.AddRange(mixedDept.Values);

            Console.WriteLine($"* Данные подготовлены: {_shopQueue.Count} отдела в очереди, {_myCollection.Count} товаров в MyCollection.");
        }

        // --- Демонстрация LINQ к стандартным коллекциям ---
        private static void DemonstrateLinqToStandardCollections()
        {
            // --- Flattening: Получаем единый список всех товаров из всех отделов ---
            // Это часто полезно для последующих запросов ко всему магазину
            var allGoods = _shopQueue.SelectMany(dept => dept.Values).ToList();
            Console.WriteLine($"\n--- Общий список всех товаров ({allGoods.Count} шт.) ---");
            // foreach (var item in allGoods) Console.WriteLine(item);


            // --- 1. Выборка (Where) ---
            Console.WriteLine("\n--- 1. Выборка (Where) ---");

            // Запрос 1.1: Найти все молочные продукты с жирностью > 3% (синтаксис запросов)
            Console.WriteLine("\n* Молочные продукты с жирностью > 3% (синтаксис запросов):");
            var highFatDairyQuery = from item in allGoods
                                    where item is DairyProduct dairy && dairy.FatContent > 3.0
                                    select item;
            PrintQueryResult(highFatDairyQuery);

            // Запрос 1.2: Найти все игрушки дороже 1000 (методы расширения)
            Console.WriteLine("\n* Игрушки дороже 1000 (методы расширения):");
            decimal priceLimit = 1000m;
            var expensiveToysQuery = allGoods
                                     .OfType<Toy>() // Фильтруем по типу Toy
                                     .Where(toy => toy.Price > priceLimit);
            PrintQueryResult(expensiveToysQuery);


            // --- 2. Операции над множествами (Union, Except, Intersect) ---
            Console.WriteLine("\n--- 2. Операции над множествами ---");
            // Сравним ассортимент первых двух отделов
            var dept1 = _shopQueue.ElementAtOrDefault(0)?.Values ?? Enumerable.Empty<Goods>();
            var dept2 = _shopQueue.ElementAtOrDefault(1)?.Values ?? Enumerable.Empty<Goods>();

            // Запрос 2.1: Все уникальные товары из первых двух отделов (Union)
            // Union сам убирает дубликаты, если Equals/GetHashCode реализованы правильно
            Console.WriteLine("\n* Уникальные товары из отделов 1 и 2 (Union):");
            var unionQuery = dept1.Union(dept2); // По умолчанию использует Equals
            PrintQueryResult(unionQuery);

            // Запрос 2.2: Товары, которые есть в отделе 1, но нет в отделе 3 (Except)
            var dept3 = _shopQueue.ElementAtOrDefault(2)?.Values ?? Enumerable.Empty<Goods>();
            Console.WriteLine("\n* Товары, которые есть в отделе 1, но нет в отделе 3 (Except):");
            var exceptQuery = dept1.Except(dept3);
            PrintQueryResult(exceptQuery);

            // Запрос 2.3: Товары, которые есть и в отделе 1, и в отделе 3 (Intersect)
            // (В нашем примере таких нет, но запрос рабочий)
            Console.WriteLine("\n* Товары, которые есть и в отделе 1, и в отделе 3 (Intersect):");
            var intersectQuery = dept1.Intersect(dept3);
            PrintQueryResult(intersectQuery);


            // --- 3. Агрегирование (Sum, Max, Min, Average, Count) ---
            Console.WriteLine("\n--- 3. Агрегирование ---");

            // Запрос 3.1: Найти самую дешевую игрушку во всем магазине (Min)
            Console.WriteLine("\n* Самая дешевая игрушка:");
            var cheapestToyPrice = allGoods.OfType<Toy>().Min(toy => (decimal?)toy.Price) ?? 0m; // (decimal?) для обработки пустой коллекции
            // Можно найти и сам товар:
            var cheapestToy = allGoods.OfType<Toy>()
                                      .OrderBy(toy => toy.Price)
                                      .FirstOrDefault();
            if (cheapestToy != null)
                Console.WriteLine($"- {cheapestToy} (Цена: {cheapestToy.Price:C})");
            else
                Console.WriteLine("- Игрушки не найдены.");

            // Запрос 3.2: Средняя цена всех товаров в отделе 3 (Average)
            Console.WriteLine("\n* Средняя цена товаров в отделе 3:");
            decimal avgPriceDept3 = dept3.Average(item => (decimal?)item.Price) ?? 0m;
            Console.WriteLine($"- {avgPriceDept3:C}");

            // Запрос 3.3: Общая стоимость всех молочных продуктов (Sum)
            Console.WriteLine("\n* Общая стоимость всех молочных продуктов:");
            decimal totalDairyCost = allGoods.OfType<DairyProduct>().Sum(dairy => dairy.Price);
            Console.WriteLine($"- {totalDairyCost:C}");

            // Запрос 3.4: Количество товаров с ценой от 50 до 100 (Count)
            Console.WriteLine("\n* Количество товаров с ценой от 50 до 100:");
            int countInRange = allGoods.Count(item => item.Price >= 50m && item.Price <= 100m);
            Console.WriteLine($"- {countInRange} шт.");


            // --- 4. Группировка (Group by) ---
            Console.WriteLine("\n--- 4. Группировка ---");

            // Запрос 4.1: Сгруппировать все товары по типу (синтаксис запросов)
            Console.WriteLine("\n* Группировка всех товаров по типу:");
            var groupedByTypeQuery = from item in allGoods
                                     group item by item.GetType().Name into typeGroup // Группируем по имени типа
                                     select new // Создаем анонимный тип для вывода
                                     {
                                         TypeName = typeGroup.Key,
                                         Count = typeGroup.Count(),
                                         Items = typeGroup.ToList() // Сохраняем элементы группы
                                     };
            foreach (var group in groupedByTypeQuery)
            {
                Console.WriteLine($"- Тип: {group.TypeName} (Количество: {group.Count})");
                // foreach(var item in group.Items) Console.WriteLine($"    - {item.Name}"); // Раскомментировать для вывода элементов
            }

            // Запрос 4.2: Сгруппировать товары в отделе 3 по производителю (методы расширения)
            Console.WriteLine("\n* Группировка товаров в отделе 3 по производителю:");
            var groupedByManufacturerQuery = dept3
                                            .GroupBy(item => item.Manufacturer) // Группируем по производителю
                                            .Select(mGroup => new {
                                                Manufacturer = mGroup.Key,
                                                Count = mGroup.Count()
                                            });
            foreach (var group in groupedByManufacturerQuery)
            {
                Console.WriteLine($"- Производитель: {group.Manufacturer} (Количество: {group.Count})");
            }


            // --- 5. Проекция (Select / Select new) ---
            Console.WriteLine("\n--- 5. Проекция ---");

            // Запрос 5.1: Создать список только названий всех товаров (Select)
            Console.WriteLine("\n* Список названий всех товаров:");
            var namesQuery = allGoods.Select(item => item.Name);
            foreach (var name in namesQuery.Take(5)) Console.WriteLine($"- {name}"); // Выведем первые 5
            if (namesQuery.Count() > 5) Console.WriteLine("- ...");

            // Запрос 5.2: Создать список объектов с названием и ценой для товаров с истекающим сроком годности (Select new)
            Console.WriteLine("\n* Названия и цены товаров с истекающим сроком годности (< 10 дней):");
            var expiringSoonQuery = allGoods
                                    .OfType<Product>() // Только продукты имеют срок годности
                                    .Where(p => p.ExpirationDate < DateTime.Today.AddDays(10))
                                    .Select(p => new { p.Name, p.Price, p.ExpirationDate }); // Анонимный тип
            foreach (var item in expiringSoonQuery)
            {
                Console.WriteLine($"- {item.Name}, Цена: {item.Price:C}, Годен до: {item.ExpirationDate:yyyy-MM-dd}");
            }
            if (!expiringSoonQuery.Any()) Console.WriteLine("- Товары с истекающим сроком не найдены.");


            // --- 6. Соединение (Join) ---
            Console.WriteLine("\n--- 6. Соединение (Join) ---");
            // Для Join нужен второй набор данных. Создадим список производителей.
            var manufacturers = new List<(int Id, string Name, string City)>
            {
                (1, "МолЗавод 1", "Город А"),
                (2, "СырКомбинат", "Город Б"),
                (3, "ИграПром", "Город В"),
                (4, "Мир Кукол", "Город Г"),
                (5, "СтройДеталь", "Город А"),
                (6, "Пекарь", "Город Б"),
                (7, "Сады", "Город Д"),
                (8, "СпортИнвест", "Город В")
                // Заметьте: "Вимм-Билль-Данн" отсутствует в этом списке
            };
            // Добавим ManufacturerId к нашим товарам (в реальном приложении это было бы поле)
            // Здесь имитируем через поиск по имени
            var goodsWithId = allGoods.Select(g => new {
                Good = g,
                ManufacturerId = manufacturers.FirstOrDefault(m => m.Name == g.Manufacturer).Id // 0 если не найден
            }).ToList();

            Console.WriteLine("\n* Соединение товаров с информацией о производителе (по имени):");
            var joinQuery = from gw in goodsWithId
                            join m in manufacturers on gw.ManufacturerId equals m.Id // Условие соединения
                            where gw.ManufacturerId != 0 // Убираем ненайденных
                            select new
                            {
                                gw.Good.Name,
                                ManufacturerName = m.Name,
                                ManufacturerCity = m.City
                            };

            foreach (var item in joinQuery)
            {
                Console.WriteLine($"- Товар: {item.Name}, Производитель: {item.ManufacturerName}, Город: {item.ManufacturerCity}");
            }


            // --- 7. Сравнение производительности LINQ и цикла ---
            Console.WriteLine("\n--- 7. Сравнение производительности ---");
            // Запрос: Найти все игрушки дороже 1000 (как в 1.2)
            Console.WriteLine($"* Поиск игрушек дороже {priceLimit:C}");

            Stopwatch stopwatch = new Stopwatch();

            // LINQ версия
            stopwatch.Start();
            var linqResult = allGoods.OfType<Toy>().Where(toy => toy.Price > priceLimit).ToList(); // ToList() для материализации
            stopwatch.Stop();
            Console.WriteLine($"- LINQ: Найдено {linqResult.Count} шт., Время: {stopwatch.Elapsed.TotalMilliseconds:F4} мс");

            // Версия с циклом foreach
            stopwatch.Restart();
            List<Toy> loopResult = new List<Toy>();
            foreach (var item in allGoods)
            {
                if (item is Toy toy && toy.Price > priceLimit)
                {
                    loopResult.Add(toy);
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"- Цикл foreach: Найдено {loopResult.Count} шт., Время: {stopwatch.Elapsed.TotalMilliseconds:F4} мс");

            Console.WriteLine("* Вывод: Производительность может варьироваться. LINQ удобнее, но может иметь небольшие накладные расходы. Для простых операций разница часто незначительна.");

        }

        // --- Вспомогательный метод для вывода результатов запроса ---
        private static void PrintQueryResult<T>(IEnumerable<T> queryResult, int maxItems = 5)
        {
            if (queryResult == null)
            {
                Console.WriteLine("  Результат запроса: null");
                return;
            }

            var itemsToShow = queryResult.Take(maxItems).ToList();
            int totalCount = queryResult.Count(); // Повторный перебор, если queryResult - это 'ленивый' запрос

            if (totalCount == 0)
            {
                Console.WriteLine("  Результат запроса: (пусто)");
                return;
            }

            Console.WriteLine($"  Результат запроса ({totalCount} шт.):");
            foreach (var item in itemsToShow)
            {
                Console.WriteLine($"  - {item}");
            }
            if (totalCount > maxItems)
            {
                Console.WriteLine("  - ...");
            }
        }

        // --- Часть 2: Методы расширения ---
        private static void DemonstrateExtensionMethods()
        {
            Console.WriteLine("\n* Демонстрация методов расширения для MyNewCollection:");

            if (!_myCollection.Any())
            {
                Console.WriteLine("MyCollection пуста, демонстрация невозможна.");
                return;
            }

            // Используем методы расширения, реализованные в MyCollectionExtensions

            Console.WriteLine("\n* Фильтрация MyCollection (Только игрушки):");
            // Используем наш метод расширения Filter
            var toysInMyCollection = _myCollection.Filter(item => item is Toy);
            PrintQueryResult(toysInMyCollection); // Используем тот же хелпер для вывода

            Console.WriteLine("\n* Агрегация MyCollection (средняя цена всех товаров):");
            // Используем наш метод расширения AggregateAverage
            decimal avgPriceMyCollection = _myCollection.AggregateAverage(item => item.Price);
            Console.WriteLine($" - Средняя цена: {avgPriceMyCollection:C}");

            Console.WriteLine("\n* Сортировка MyCollection (по имени):");
            // Используем наш метод расширения SortBy
            var sortedMyCollection = _myCollection.SortBy(item => item.Name);
            PrintQueryResult(sortedMyCollection);

            Console.WriteLine("\n* Группировка MyCollection (по производителю):");
            // Используем наш метод расширения GroupByCriteria
            var groupedMyCollection = _myCollection.GroupByCriteria(item => item.Manufacturer);
            foreach (var group in groupedMyCollection)
            {
                Console.WriteLine($"- Производитель: {group.Key} (Количество: {group.Count()})");
                // foreach(var item in group) Console.WriteLine($"    - {item.Name}"); // Раскомментировать для вывода элементов
            }
        }
    }
}
