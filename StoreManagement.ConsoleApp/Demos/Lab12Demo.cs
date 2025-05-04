// Файл: StoreManagement.ConsoleApp/Demos/Lab12Demo.cs
using System;
using System.Collections.Generic; // Для List<T> в демо MyCollection
using StoreManagement.Domain;     // Доступ к Goods, Toy, DairyProduct и т.д.
using StoreManagement.Collections; // Доступ к DoublyLinkedList, BinaryTree, HashTable, MyCollection

namespace StoreManagement.ConsoleApp.Demos
{
    /// <summary>
    /// Демонстрация функциональности Лабораторной Работы №12: Пользовательские коллекции.
    /// </summary>
    public static class Lab12Demo
    {
        public static void Run()
        {
            Console.WriteLine("\n===== Демонстрация ЛР №12: Пользовательские коллекции =====");

            // --- Демонстрация DoublyLinkedList<Goods> ---
            DemonstrateDoublyLinkedList();

            // --- Демонстрация BinaryTree<Goods> ---
            DemonstrateBinaryTree();

            // --- Демонстрация HashTable<string, Goods> ---
            DemonstrateHashTable();

            // --- Демонстрация MyCollection<Goods> ---
            DemonstrateMyCollection();

            Console.WriteLine("\n===== Демонстрация ЛР №12 Завершена =====");
        }

        // --- Метод для создания случайного товара ---
        private static Goods CreateRandomGoods()
        {
            Goods newItem;
            int typeChoice = Goods.random.Next(3);
            switch (typeChoice)
            {
                case 0: newItem = new DairyProduct(); break;
                case 1: newItem = new Toy(); break;
                case 2:
                default: newItem = new Product(); break;
            }
            newItem.RandomInit();
            // Гарантируем уникальность имени для хеш-таблицы (упрощенно)
            newItem.Name += $"_{Goods.random.Next(1000, 9999)}";
            return newItem;
        }

        // --- Демонстрация DoublyLinkedList<Goods> ---
        private static void DemonstrateDoublyLinkedList()
        {
            Console.WriteLine("\n--- 1. Демонстрация DoublyLinkedList<Goods> ---");
            DoublyLinkedList<Goods> goodsList = new DoublyLinkedList<Goods>();

            // Заполнение списка
            Console.WriteLine("\n* Заполнение списка 5 случайными товарами:");
            Goods? firstItem = null;
            for (int i = 0; i < 5; i++)
            {
                var newItem = CreateRandomGoods();
                if (i == 0) firstItem = newItem; // Запомним первый добавленный
                goodsList.Add(newItem);
            }
            goodsList.Print();

            // Удаление первого элемента с заданным именем (Вариант 7)
            if (firstItem != null)
            {
                Console.WriteLine($"\n* Удаление первого элемента с именем '{firstItem.Name}' (Вариант 7):");
                bool removed = goodsList.RemoveFirstByName(firstItem.Name);
                Console.WriteLine(removed ? "Элемент успешно удален." : "Элемент с таким именем не найден.");
                goodsList.Print();
            }
            else
            {
                Console.WriteLine("\n* Не удалось получить имя первого элемента для теста удаления.");
            }

            // Попытка удаления несуществующего имени
            string nonExistentName = "НесуществующийТовар123";
            Console.WriteLine($"\n* Попытка удаления элемента с именем '{nonExistentName}':");
            bool removedNonExistent = goodsList.RemoveFirstByName(nonExistentName);
            Console.WriteLine(removedNonExistent ? "Ошибка: удален несуществующий элемент." : "Элемент с таким именем не найден (ожидаемо).");
            goodsList.Print();


            // Очистка списка
            Console.WriteLine("\n* Очистка списка:");
            goodsList.Clear();
            goodsList.Print();
        }

        // --- Демонстрация BinaryTree<Goods> ---
        private static void DemonstrateBinaryTree()
        {
            Console.WriteLine("\n--- 2. Демонстрация BinaryTree<Goods> ---");

            // Создание идеально сбалансированного дерева
            Console.WriteLine("\n* Создание идеально сбалансированного дерева из 7 элементов:");
            BinaryTree<Goods> goodsTree = new BinaryTree<Goods>();
            goodsTree.BuildBalancedTree(7, CreateRandomGoods); // Используем метод создания как фабрику
            goodsTree.PrintInOrder(); // Вывод в симметричном порядке

            // Подсчет элементов, имя которых начинается с символа (Вариант 7)
            char searchChar = 'М'; // Пример символа
            Console.WriteLine($"\n* Поиск количества элементов, имя которых начинается на '{searchChar}' (Вариант 7):");
            int count = goodsTree.CountNodesWithNameStartingWith(searchChar);
            Console.WriteLine($"Найдено элементов: {count}");

            // Преобразование в дерево поиска
            Console.WriteLine("\n* Преобразование в Бинарное Дерево Поиска (сортировка по цене):");
            goodsTree.ConvertToSearchTree();
            goodsTree.PrintInOrder(); // Вывод BST должен быть отсортирован по цене (IComparable)

            // Очистка дерева
            Console.WriteLine("\n* Очистка дерева:");
            goodsTree.Clear();
            goodsTree.PrintInOrder();
        }

        // --- Демонстрация HashTable<string, Goods> ---
        private static void DemonstrateHashTable()
        {
            Console.WriteLine("\n--- 3. Демонстрация HashTable<string, Goods> (Открытая адресация) ---");
            // Используем имя товара как ключ (убедились, что имена уникальны в CreateRandomGoods)
            HashTable<string, Goods> goodsTable = new HashTable<string, Goods>(4); // Начнем с малой емкости для демонстрации ресайза

            // Добавление элементов (должен произойти ресайз)
            Console.WriteLine("\n* Добавление 5 элементов (ожидается ресайз):");
            Goods? itemToFind = null;
            Goods? itemToRemove = null;
            string keyToRemove = "";

            for (int i = 0; i < 5; i++)
            {
                Goods newItem = CreateRandomGoods();
                Console.WriteLine($"Добавляем: Key='{newItem.Name}', Value='{newItem}'");
                goodsTable.Add(newItem.Name, newItem);
                if (i == 1) itemToFind = newItem; // Запомним для поиска
                if (i == 3) // Запомним для удаления
                {
                    itemToRemove = newItem;
                    keyToRemove = newItem.Name;
                }
                goodsTable.PrintDebug(); // Показываем состояние после каждого добавления
            }

            // Поиск существующего элемента
            if (itemToFind != null)
            {
                Console.WriteLine($"\n* Поиск элемента с ключом '{itemToFind.Name}':");
                if (goodsTable.TryGetValue(itemToFind.Name, out Goods? foundValue))
                {
                    Console.WriteLine($"Найден: {foundValue}");
                    Console.WriteLine($"Совпадает с ожидаемым? {(ReferenceEquals(itemToFind, foundValue) ? "Да" : "Нет (ошибка)")}");
                }
                else
                {
                    Console.WriteLine("Ошибка: элемент не найден.");
                }
            }

            // Удаление элемента
            if (itemToRemove != null)
            {
                Console.WriteLine($"\n* Удаление элемента с ключом '{keyToRemove}':");
                bool removed = goodsTable.Remove(keyToRemove);
                Console.WriteLine(removed ? "Элемент успешно удален (помечен как Deleted)." : "Ошибка: элемент для удаления не найден.");
                goodsTable.PrintDebug(); // Показываем состояние после удаления (ячейка должна быть <Deleted>)

                // Повторный поиск удаленного элемента
                Console.WriteLine($"\n* Повторный поиск удаленного элемента с ключом '{keyToRemove}':");
                if (goodsTable.TryGetValue(keyToRemove, out Goods? removedValue))
                {
                    Console.WriteLine($"Ошибка: Удаленный элемент найден со значением {removedValue}.");
                }
                else
                {
                    Console.WriteLine("Элемент не найден (ожидаемо).");
                }

                // Попытка добавить элемент с тем же ключом после удаления
                Console.WriteLine($"\n* Попытка добавить новый элемент с ключом '{keyToRemove}' после удаления:");
                Goods replacementItem = new Toy($"Новый_{keyToRemove}", 999m, "Замена", 0, "Картон");
                goodsTable.Add(keyToRemove, replacementItem);
                Console.WriteLine("Элемент добавлен (должен был занять <Deleted> ячейку или другую).");
                goodsTable.PrintDebug();
            }

            // Демонстрация поиска несуществующего ключа
            string nonExistentKey = "НесуществующийКлюч999";
            Console.WriteLine($"\n* Поиск несуществующего ключа '{nonExistentKey}':");
            if (goodsTable.TryGetValue(nonExistentKey, out Goods? nonExistentValue))
            {
                Console.WriteLine($"Ошибка: Найден несуществующий элемент со значением {nonExistentValue}.");
            }
            else
            {
                Console.WriteLine("Элемент не найден (ожидаемо).");
            }

            // Очистка таблицы
            Console.WriteLine("\n* Очистка хеш-таблицы:");
            goodsTable.Clear();
            goodsTable.PrintDebug();
        }

        // --- Демонстрация MyCollection<Goods> ---
        private static void DemonstrateMyCollection()
        {
            Console.WriteLine("\n--- 4. Демонстрация MyCollection<Goods> (на базе DoublyLinkedList) ---");

            // Создание из IEnumerable
            Console.WriteLine("\n* Создание коллекции из списка случайных товаров:");
            List<Goods> initialItems = new List<Goods>();
            for (int i = 0; i < 4; ++i) initialItems.Add(CreateRandomGoods());
            MyCollection<Goods> myCollection = new MyCollection<Goods>(initialItems);
            myCollection.Print("Начальная коллекция");

            // Добавление элемента
            Console.WriteLine("\n* Добавление нового элемента:");
            Goods itemToAdd = new DairyProduct("Йогурт Чудо", 80m, "Вимм-Билль-Данн", DateTime.Today.AddDays(14), 3.2, 0.125);
            myCollection.Add(itemToAdd);
            myCollection.Print("Коллекция после добавления");

            // Проверка наличия
            Console.WriteLine($"\n* Проверка наличия '{itemToAdd.Name}': {myCollection.Contains(itemToAdd)}");
            Goods nonExistentItem = new Toy("Несуществующая Игрушка", 1m, "N/A", 0, "N/A");
            Console.WriteLine($"* Проверка наличия '{nonExistentItem.Name}': {myCollection.Contains(nonExistentItem)}");

            // Удаление элемента
            Console.WriteLine($"\n* Удаление элемента '{itemToAdd.Name}':");
            bool removed = myCollection.Remove(itemToAdd);
            Console.WriteLine(removed ? "Удалено успешно." : "Элемент не найден.");
            myCollection.Print("Коллекция после удаления");

            // Удаление по индексу (RemoveAt)
            if (myCollection.Count > 1)
            {
                int indexToRemove = 1;
                Console.WriteLine($"\n* Удаление элемента по индексу {indexToRemove}:");
                try
                {
                    Goods itemAtIndex = myCollection[indexToRemove]; // Получим элемент для информации
                    Console.WriteLine($"(Удаляемый элемент: {itemAtIndex})");
                    bool removedAt = myCollection.RemoveAt(indexToRemove);
                    Console.WriteLine(removedAt ? "Удалено успешно." : "Не удалось удалить по индексу.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при удалении по индексу: {ex.Message}");
                }
                myCollection.Print($"Коллекция после удаления по индексу {indexToRemove}");
            }


            // Клонирование
            Console.WriteLine("\n* Демонстрация клонирования:");
            MyCollection<Goods> shallowCopy = myCollection.ShallowCopy();
            MyCollection<Goods>? deepCopy = null;
            try
            {
                deepCopy = myCollection.Clone(); // Использует ICloneable у Goods
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка глубокого клонирования: {ex.Message}");
            }

            // Изменение элемента в оригинальной коллекции
            if (myCollection.Count > 0)
            {
                Console.WriteLine("\n* Изменение цены первого элемента в ОРИГИНАЛЬНОЙ коллекции:");
                Goods firstOriginal = myCollection[0]; // Получаем ссылку
                decimal oldPrice = firstOriginal.Price;
                firstOriginal.Price += 1000; // Меняем цену

                myCollection.Print("Оригинал после изменения");
                shallowCopy.Print("Поверхностная копия после изменения оригинала (цена должна измениться)");
                if (deepCopy != null) deepCopy.Print("Глубокая копия после изменения оригинала (цена НЕ должна измениться)");

                // Возвращаем цену обратно
                firstOriginal.Price = oldPrice;
            }
            else
            {
                Console.WriteLine("Коллекция пуста, демонстрация изменения при клонировании невозможна.");
            }


            // Очистка
            Console.WriteLine("\n* Очистка коллекции:");
            myCollection.Clear();
            myCollection.Print("Коллекция после очистки");
        }
    }
}