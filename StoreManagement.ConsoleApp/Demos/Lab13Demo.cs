// Файл: StoreManagement.ConsoleApp/Demos/Lab13Demo.cs
using System;
using StoreManagement.Domain;     // Доступ к Goods, Toy, DairyProduct и т.д.
using StoreManagement.Collections; // Доступ к MyNewCollection
using StoreManagement.Services;    // Доступ к Journal

namespace StoreManagement.ConsoleApp.Demos
{
    /// <summary>
    /// Демонстрация функциональности Лабораторной Работы №13: События в коллекциях.
    /// </summary>
    public static class Lab13Demo
    {
        public static void Run()
        {
            Console.WriteLine("\n===== Демонстрация ЛР №13: События в коллекциях =====");

            // 1. Создание двух коллекций MyNewCollection<Goods>
            Console.WriteLine("\n--- Создание коллекций mc1 и mc2 ---");
            MyNewCollection<Goods> mc1 = new MyNewCollection<Goods>("Коллекция №1");
            MyNewCollection<Goods> mc2 = new MyNewCollection<Goods>("Коллекция №2");

            // 2. Создание двух объектов Journal
            Console.WriteLine("\n--- Создание журналов j1 и j2 ---");
            Journal j1 = new Journal();
            Journal j2 = new Journal();

            // 3. Подписка j1 на ОБА события от mc1
            Console.WriteLine("\n--- Подписка j1 на события от mc1 ---");
            mc1.CollectionCountChanged += j1.CollectionCountChangedHandler;       // Подписка на изменение количества
            mc1.CollectionReferenceChanged += j1.CollectionReferenceChangedHandler; // Подписка на изменение ссылки

            // 4. Подписка j2 ТОЛЬКО на CollectionReferenceChanged от ОБЕИХ коллекций
            Console.WriteLine("\n--- Подписка j2 на CollectionReferenceChanged от mc1 и mc2 ---");
            mc1.CollectionReferenceChanged += j2.CollectionReferenceChangedHandler; // j2 слушает mc1
            mc2.CollectionReferenceChanged += j2.CollectionReferenceChangedHandler; // j2 слушает mc2
                                                                                    // mc2.CollectionCountChanged НЕ подписываем на j2

            Console.WriteLine("\n--- Начальное состояние журналов ---");
            j1.PrintJournal("Журнал 1 (слушает все от mc1)");
            j2.PrintJournal("Журнал 2 (слушает ReferenceChanged от mc1 и mc2)");


            // 5. Выполнение операций с коллекциями для генерации событий
            Console.WriteLine("\n--- Выполнение операций с коллекциями ---");

            // Добавление элементов
            Console.WriteLine("\n* Добавление элементов в mc1:");
            mc1.Add(Lab12Demo.CreateRandomGoods()); // Используем метод из Lab12Demo для генерации
            mc1.Add(Lab12Demo.CreateRandomGoods());

            Console.WriteLine("\n* Добавление элементов в mc2:");
            mc2.Add(Lab12Demo.CreateRandomGoods()); // Это событие НЕ должно попасть в j2
            mc2.Add(Lab12Demo.CreateRandomGoods()); // И это

            Console.WriteLine("\n--- Состояние журналов после добавлений ---");
            j1.PrintJournal("Журнал 1"); // Должны быть записи Add от mc1
            j2.PrintJournal("Журнал 2"); // Должен быть пуст

            mc1.Print("Коллекция mc1");
            mc2.Print("Коллекция mc2");

            // Удаление элементов
            if (mc1.Count > 0)
            {
                Console.WriteLine("\n* Удаление элемента из mc1 (по индексу 0):");
                mc1.RemoveAt(0); // Генерирует CollectionCountChanged
            }
            if (mc2.Count > 0)
            {
                Console.WriteLine("\n* Удаление элемента из mc2 (по значению):");
                Goods itemInMc2 = mc2[0]; // Берем первый элемент для удаления
                mc2.Remove(itemInMc2); // Генерирует CollectionCountChanged, но j2 не слушает
            }

            Console.WriteLine("\n--- Состояние журналов после удалений ---");
            j1.PrintJournal("Журнал 1"); // Должна быть запись Remove от mc1
            j2.PrintJournal("Журнал 2"); // Должен быть все еще пуст

            mc1.Print("Коллекция mc1");
            mc2.Print("Коллекция mc2");

            // Изменение элементов через индексатор (сеттер)
            if (mc1.Count > 0)
            {
                Console.WriteLine("\n* Изменение элемента в mc1 (индекс 0):");
                Goods replacement1 = Lab12Demo.CreateRandomGoods();
                replacement1.Name = "ЗАМЕНА_В_MC1";
                mc1[0] = replacement1; // Генерирует CollectionReferenceChanged
                                       // ВНИМАНИЕ: Фактически элемент не заменится из-за ограничений базового списка!
                                       // Но событие должно сгенерироваться.
            }
            if (mc2.Count > 0)
            {
                Console.WriteLine("\n* Изменение элемента в mc2 (индекс 0):");
                Goods replacement2 = Lab12Demo.CreateRandomGoods();
                replacement2.Name = "ЗАМЕНА_В_MC2";
                mc2[0] = replacement2; // Генерирует CollectionReferenceChanged
            }

            Console.WriteLine("\n--- Финальное состояние журналов ---");
            j1.PrintJournal("Журнал 1"); // Должна быть запись ReferenceChanged от mc1
            j2.PrintJournal("Журнал 2"); // Должны быть записи ReferenceChanged от mc1 и mc2

            mc1.Print("Коллекция mc1 (элемент [0] не должен был измениться!)");
            mc2.Print("Коллекция mc2 (элемент [0] не должен был измениться!)");


            Console.WriteLine("\n===== Демонстрация ЛР №13 Завершена =====");
        }
    }
}